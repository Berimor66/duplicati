#region Disclaimer / License
// Copyright (C) 2011, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using Duplicati.Library.Interface;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Reader;
using SharpCompress.Writer;


/*
 
 *  Current Problems. TODO: Size and FlushBuffer size need to worked out correctly otherwise Duplicati has no idea how big the output is.
 
 * 
 
1) The filename should not be a read-write property IMO. 
	- HUH?
2) I am a bit worried that you keep all compressed streams in memory, why not write them to the archive as you go? Optionally when they are closed/disposed?
	- Done
3) The size is used to determine when to create a new volume. Apart from the size of streams, there is also a "Central Header" which is a list if filenames and offsets. It takes 46 + 24 + len(filename in encoding) pr. entry. It is fairly important that the size calculation is correct as some providers have a strict max-filesize. The current implementation does not capture this overhead 100% correctly, so I have added a 1200 bytes grace margin.
	- TODO - I think this is working now, but need to check with team to see if I still need to add headers as the size calculation seems to be correct due to FileStream length
4) I could have sworn directories were used :). But that was perhaps in an earlier version. I originally used the ICompression interface for debugging, so I could switch to a folder instead of a Zip file for output. But lets remove those methods from the interface if it is not used.
	- Done
5) The "missing date" should be either EPOCH or "01/01/0000 00:00:00", Duplicati uses the timestamp to determine if the file should be checked for changes, setting it to "Now", means that the file is probably older than "Now" and will not be checked. I think I have used "new DateTime(0)" somewhere.
	- Done (Using DateTime.Min)
6) You use a bit of .Net 4.0 syntax, this means that we cannot port the changes directly to the 1.3.x branch as that is .Net 2.0. Not sure this is a problem though.
	- TODO: Ignore or solve? Need to ask which part
7) The lookup of filenames should calculate the "normalized" version of the name outside the Linq query, otherwise it is translated for each compare.
	- TODO
8) I prefer if there is both a license.txt and download.txt file in each of the 3rd party folders, just to make sure we have actually considered license implications. But as they are not distributed, maybe that is not really important.
	- TODO
9) I checked the SharpCompress source, and they use UTF-8 all over, so we are good on that.
	- TODO  
10) Create a custom exception
    - TODO
 */



//ZIP Implementation using SharpCompress
//Please note, duplicati does not require both Read & Write access at the same time
//and so this has not been implemented
namespace Duplicati.Library.Compression
{
    /// <summary>
    /// An abstraction of a zip archive as a FileArchive.
    /// Due to a very unfortunate Zip implementation, the archive is either read or write, never both
    /// </summary>
    public class FileArchiveZip : ICompression
    {
        /// <summary>
        /// The commandline option for toggling the compression level
        /// </summary>
        private const string COMPRESSION_LEVEL_OPTION = "compression-level";

        /// <summary>
        /// The default compression level
        /// </summary>
        private const int DEFAULT_COMPRESSION_LEVEL = 9;

        /// <summary>
        /// When creating a new file in the zip file, the memory stream is  kept
        /// here until the file is written (in the dispose)
        /// </summary>
        private List<CompressionEntity> ZipStreams { get; set; }

        private string FileName { get; set; }

        /// <summary>
        /// This property indicates that this current instance should write to a file
        /// </summary>
        private bool IsWriting { get; set; }

        private IWriter ZipWriter { get; set; }

        private FileStream FileWriter { get; set; }

        /// <summary>
        /// Default constructor, used to read file extension and supported commands
        /// </summary>
        public FileArchiveZip() { }

        /// <summary>
        /// Constructs a new zip instance.
        /// If the file exists and has a non-zero length we read it,
        /// otherwise we create a new archive.
        /// </summary>
        /// <param name="file">The name of the file to read or write</param>
        /// <param name="options">The options passed on the commandline</param>
        public FileArchiveZip(string file, Dictionary<string, string> options)
        {
            if (string.IsNullOrEmpty(file.Trim()))
                throw new Exception("NULL filename is not valid");

            FileName = file;
            ZipStreams = new List<CompressionEntity>();

            IsWriting = !File.Exists(file);

            //TODO: Set compression level, maybe on saving?
            //if (options.TryGetValue(COMPRESSION_LEVEL_OPTION, out cplvl) && int.TryParse(cplvl, out tmplvl))
            //    compressionLevel = Math.Max(Math.Min(9, tmplvl), 0);

            if (File.Exists(file) && new FileInfo(file).Length > 0)
            {
                using (var stream = File.OpenRead(file))
                using (var reader = ReaderFactory.Open(stream))
                    ReadZipIntoMemory(reader);
            }
        }

        private void ReadZipIntoMemory(IReader reader)
        {
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    var entity = new CompressionEntity(reader.Entry);
                    SetupEntityEvents(entity);

                    ZipStreams.Add(entity);
                }
            }
        }

        private void SetupEntityEvents(CompressionEntity entity)
        {
            entity.StreamDisposing += EntityStreamStreamDisposing;
            entity.RequiresStream += EntityRequiresStream;
        }

        private MemoryStream EntityRequiresStream(CompressionEntity sender)
        {
            var rtnStream = new MemoryStream();

            if (!IsWriting)
            {
                if (File.Exists(FileName) && new FileInfo(FileName).Length > 0)
                {
                    using (var stream = File.OpenRead(FileName))
                    using (var reader = ReaderFactory.Open(stream))
                    {
                        Stream foundStream = null;

                        while (reader.MoveToNextEntry())
                        {
                            if (reader.Entry.FilePath != sender.FilePath) continue;

                            foundStream = reader.OpenEntryStream();
                            break;
                        }


                        if (foundStream != null)
                        {
                            using (foundStream)
                                Utility.Utility.CopyStream(foundStream, rtnStream);
                        }
                    }
                }
            }

            return rtnStream;
        }

        private void EntityStreamStreamDisposing(CompressionEntity sender)
        {
            if (!IsWriting) return;

            if (FileWriter == null)
            {
                FileWriter = File.OpenWrite(FileName);

                //TODO: This can be replaced easily with LZMA
                ZipWriter = WriterFactory.Open(FileWriter, ArchiveType.Zip, CompressionType.Deflate);
            }

            sender.ResetStream();
            ZipWriter.Write(sender.FilePath, sender.CompressionStream, sender.LastModified);
        }

        /// <summary>
        /// Converts a zip file path to the format used by the local filesystem.
        /// Internally all files are stored in the archive use / as directory separator.
        /// </summary>
        /// <param name="path">The path to convert in internal format</param>
        /// <returns>The path in filesystem format</returns>
        private static string PathToOsFilesystem(string path)
        {
            return Path.DirectorySeparatorChar != '/' ? path.Replace('/', Path.DirectorySeparatorChar) : path;
        }

        /// <summary>
        /// Converts a file system path to the internal format.
        /// Internally all files are stored in the archive use / as directory separator.
        /// </summary>
        /// <param name="path">The path to convert in filesystem format</param>
        /// <returns>The path in the internal format</returns>
        private static string PathToZipFilesystem(string path)
        {
            return Path.DirectorySeparatorChar != '/' ? path.Replace(Path.DirectorySeparatorChar, '/') : path;
        }

        #region IFileArchive Members
        /// <summary>
        /// Gets the filename extension used by the compression module
        /// </summary>
        public string FilenameExtension { get { return "zip"; } }
        /// <summary>
        /// Gets a friendly name for the compression module
        /// </summary>
        public string DisplayName { get { return Strings.FileArchiveZip.DisplayName; } }
        /// <summary>
        /// Gets a description of the compression module
        /// </summary>
        public string Description { get { return Strings.FileArchiveZip.Description; } }

        /// <summary>
        /// Gets a list of commands supported by the compression module
        /// </summary>
        public IList<ICommandLineArgument> SupportedCommands
        {
            get
            {
                return new List<ICommandLineArgument>(new ICommandLineArgument[] {
                new CommandLineArgument(COMPRESSION_LEVEL_OPTION, CommandLineArgument.ArgumentType.Enumeration, Strings.FileArchiveZip.CompressionlevelShort, Strings.FileArchiveZip.CompressionlevelLong, DEFAULT_COMPRESSION_LEVEL.ToString(), null, new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
            });
            }
        }

        private IEnumerable<string> FilterEntries(string prefix)
        {
            prefix = PathToZipFilesystem(prefix);

            return
                ZipStreams
                    .Where(x => prefix == null || x.FilePath.StartsWith(prefix))
                            .Select(x => PathToOsFilesystem(x.FilePath));
        }

        /// <summary>
        /// Returns a list of files matching the given prefix
        /// </summary>
        /// <param name="prefix">The prefix to match</param>
        /// <returns>A list of files matching the prefix</returns>
        public string[] ListFiles(string prefix)
        {
            return FilterEntries(prefix).ToArray();
        }

        /// <summary>
        /// Returns a list of entries matching the given prefix
        /// </summary>
        /// <param name="prefix">The prefix to match</param>
        /// <returns>A list of entries matching the prefix</returns>
        public string[] ListEntries(string prefix)
        {
            return FilterEntries(prefix).ToArray();
        }

        /// <summary>
        /// Reads all bytes from a file and returns it as an array
        /// </summary>
        /// <param name="file">The name of the file to read</param>
        /// <returns>The contents of the file as a byte array</returns>
        public byte[] ReadAllBytes(string file)
        {
            using (var ms = new MemoryStream())
            using (var s = OpenRead(file))
            {
                Utility.Utility.CopyStream(s, ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Reads all lines from a text file, and returns them
        /// </summary>
        /// <param name="file">The name of the file to read</param>
        /// <returns>The lines read from the file</returns>
        public string[] ReadAllLines(string file)
        {
            var lines = new List<string>();
            using (var sr = new StreamReader(OpenRead(file)))
                while (!sr.EndOfStream)
                    lines.Add(sr.ReadLine());

            return lines.ToArray();
        }

        /// <summary>
        /// Opens an file for reading
        /// </summary>
        /// <param name="file">The name of the file to open</param>
        /// <returns>A stream with the file contents</returns>
        public Stream OpenRead(string file)
        {
            var ze = GetEntry(file);

            return ze == null ? null : ze.CompressionStream;
        }

        /// <summary>
        /// Opens a file for writing
        /// </summary>
        /// <param name="file">The name of the file to write</param>
        /// <returns>A stream that can be updated with file contents</returns>
        public Stream OpenWrite(string file)
        {
            return CreateFile(file);
        }

        /// <summary>
        /// Writes all bytes from the array into the file
        /// </summary>
        /// <param name="file">The name of the file</param>
        /// <param name="data">The data contents of the file</param>
        public void WriteAllBytes(string file, byte[] data)
        {
            using (var s = CreateFile(file))
                s.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes the given lines into the file
        /// </summary>
        /// <param name="file">The name of the file</param>
        /// <param name="data">The lines to write</param>
        public void WriteAllLines(string file, string[] data)
        {
            using (var sw = new StreamWriter(CreateFile(file)))
                foreach (var s in data)
                    sw.WriteLine(s);
        }

        /// <summary>
        /// Internal function that returns a ZipEntry for a filename, or null if no such file exists
        /// </summary>
        /// <param name="file">The name of the file to find</param>
        /// <returns>The ZipEntry for the file or null if no such file was found</returns>
        private CompressionEntity GetEntry(string file)
        {
            var found = ZipStreams.FirstOrDefault(x => x.FilePath == PathToZipFilesystem(file));

            if (found == null)
                found = ZipStreams.FirstOrDefault(x => x.FilePath == PathToOsFilesystem(file));

            return found;
        }

        /// <summary>
        /// Deletes a file from the archive
        /// </summary>
        /// <param name="file">The name of the file to delete</param>
        public void DeleteFile(string file)
        {
            var zipEntry = ZipStreams.First(x => x.FilePath == file);
            if (zipEntry != null)
                ZipStreams.Remove(zipEntry);
        }

        /// <summary>
        /// Creates a file in the archive and returns a writeable stream
        /// </summary>
        /// <param name="file">The name of the file to create</param>
        /// <returns>A writeable stream for the file contents</returns>
        public Stream CreateFile(string file)
        {
            return CreateFile(file, DateTime.Now);
        }

        /// <summary>
        /// Creates a file in the archive and returns a writeable stream
        /// </summary>
        /// <param name="file">The name of the file to create</param>
        /// <param name="lastWrite">The time the file was last written</param>
        /// <returns>A writeable stream for the file contents</returns>
        public Stream CreateFile(string file, DateTime lastWrite)
        {
            if (FileExists(file))
                DeleteFile(file);

            //TODO: Unicode saving of file names
            //Encode filenames as unicode, we do this for all files, to avoid codepage issues
            //ze.Flags |= (int)ICSharpCode.SharpZipLib.Zip.GeneralBitFlags.UnicodeText;

            IsWriting = true;

            return GetFileStream(file, lastWrite);
        }

        private Stream GetFileStream(string file, DateTime lastWrite)
        {
            var entry = GetEntry(file);
            if (entry != null)
                return entry.CompressionStream;

            var newEntry = new CompressionEntity(PathToZipFilesystem(file), lastWrite);
            SetupEntityEvents(newEntry);

            ZipStreams.Add(newEntry);
            return newEntry.CompressionStream;
        }

        /// <summary>
        /// Returns a value that indicates if the file exists
        /// </summary>
        /// <param name="file">The name of the file to test existence for</param>
        /// <returns>True if the file exists, false otherwise</returns>
        public bool FileExists(string file)
        {
            return GetEntry(file) != null;
        }

        /// <summary>
        /// Gets the current size of the archive
        /// </summary>
        public long Size
        {
            get
            {
                return FileWriter == null? 0 : FileWriter.Length;
            }
        }

        /// <summary>
        /// The size of the current unflushed buffer
        /// </summary>
        public long FlushBufferSize
        { 
            get
            {
                return ZipStreams.Sum(x => x.UnFlushedSize);
            } 
        }


        /// <summary>
        /// Gets the last write time for a file
        /// </summary>
        /// <param name="file">The name of the file to query</param>
        /// <returns>The last write time for the file</returns>
        public DateTime GetLastWriteTime(string file)
        {
            var entry = GetEntry(file);
            if (entry != null)
                return entry.LastModified; //TODO: Is this ok?

            throw new Exception(string.Format(Strings.FileArchiveZip.FileNotFoundError, file));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (IsWriting)
            {
                if (ZipWriter != null)
                    ZipWriter.Dispose();

                if (FileWriter != null)
                    FileWriter.Dispose();
            }

            if (ZipStreams != null)
                foreach (var entity in ZipStreams)
                    entity.Dispose();
        }

        #endregion

    }
}
