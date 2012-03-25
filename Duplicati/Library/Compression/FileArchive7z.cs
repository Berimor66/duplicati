#region Disclaimer / License
/*  SevenZipSharp is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SevenZipSharp is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with SevenZipSharp.  If not, see <http://www.gnu.org/licenses/>. */
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Duplicati.Library.Interface;
using SevenZip;

namespace Duplicati.Library.Compression
{
    /// <summary>
    /// Using the freely available library "SevenZip" found at
    /// http://sevenzipsharp.codeplex.com
    /// </summary>
    public class FileArchive7z : ICompression
    {
        private bool m_saveChanges;
        private string m_fileName;
        private SevenZipExtractor m_7zExtractor;
        private SevenZipCompressor m_7zCompressor;
        private string m_passkey;

        //Where all the files and streams are stored
        private List<SevenZipEntry> m_fileStore { get; set; }

        /// <summary>
        /// Default constructor, used to read file extension and supported commands
        /// </summary>
        public FileArchive7z()
        {
        }

        /// <summary>
        /// Constructs a new 7z
        /// </summary>
        /// <param name="file">The name of the file to read or write</param>
        /// <param name="options">The options passed on the commandline</param>
        public FileArchive7z(string file, Dictionary<string, string> options)
        {
            //Detect x64 operating system and load the corrent DLL
            if (IntPtr.Size == 8)
                SevenZipExtractor.SetLibraryPath(@"7z64.dll");
            else
                SevenZipExtractor.SetLibraryPath(@"7z.dll");

            m_fileName = file;
            m_fileStore = new List<SevenZipEntry>();
            m_saveChanges = false;

            //Used for encryption
            if (options.ContainsKey("encryption-module") && options["encryption-module"] == FilenameExtension)
                m_passkey = options["passphrase"];

            if (!System.IO.File.Exists(file) || new System.IO.FileInfo(file).Length == 0)
                System.IO.File.Create(file).Close();
            else
            { //Only setup the extractor if there is something to extract
                if(string.IsNullOrEmpty(m_passkey))
                    m_7zExtractor = new SevenZipExtractor(file);
                else
                    m_7zExtractor = new SevenZipExtractor(file, m_passkey);

                m_read7z();
            }
        }

        #region SevenZipStuff
        //Read in files and store a stream to that file (initially empty)
        private void m_read7z()
        {
            if (m_7zExtractor != null)
                foreach (ArchiveFileInfo ze in m_7zExtractor.ArchiveFileData)
                    m_fileStore.Add(new SevenZipEntry(ze));
        }

        //Get a stream for a specific index and setup the 
        private Stream m_getStream(string fileName)
        {
            var ze = m_getEntry(fileName);

            if (ze != null)
            {
                if (ze.BaseStream.Length == 0)
                {
                    if (m_7zExtractor != null)
                    {
                        m_7zExtractor.ExtractFile(ze.Index, ze.BaseStream);
                        ze.BaseStream.Position = 0;
                    }
                }

                return ze.BaseStream;
            }

            return null;
        }

        private SevenZipEntry m_getEntry(string file)
        {
            //Checks for both type of path formats as like the ZIP
            var tmpEntry = m_getEntrySearch(m_pathFromFilesystem(file));

            if (tmpEntry == null)
                tmpEntry = m_getEntrySearch(m_pathToFilesystem(file));

            return tmpEntry;
        }

        private SevenZipEntry m_getEntrySearch(string file)
        {
            foreach (var ze in m_fileStore)
                if (ze.FileName.ToLower() == file.ToLower())
                    return ze;

            return null;
        }

        #endregion

        #region MiscFunctions
        /// <summary>
        /// Converts a zip file path to the format used by the local filesystem.
        /// Internally all files are stored in the archive use / as directory separator.
        /// </summary>
        /// <param name="path">The path to convert in internal format</param>
        /// <returns>The path in filesystem format</returns>
        private string m_pathToFilesystem(string path)
        {
            if (System.IO.Path.DirectorySeparatorChar != '/')
                return path.Replace('/', System.IO.Path.DirectorySeparatorChar);
            else
                return path;
        }

        /// <summary>
        /// Converts a file system path to the internal format.
        /// Internally all files are stored in the archive use / as directory separator.
        /// </summary>
        /// <param name="path">The path to convert in filesystem format</param>
        /// <returns>The path in the internal format</returns>
        private string m_pathFromFilesystem(string path)
        {
            if (System.IO.Path.DirectorySeparatorChar != '/')
                return path.Replace(System.IO.Path.DirectorySeparatorChar, '/');
            else
                return path;
        }

        /// <summary>
        /// Reads all bytes from a file and returns it as an array
        /// </summary>
        /// <param name="file">The name of the file to read</param>
        /// <returns>The contents of the file as a byte array</returns>
        public byte[] ReadAllBytes(string file)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            using (System.IO.Stream s = OpenRead(file))
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
            List<string> lines = new List<string>();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(OpenRead(file)))
                while (!sr.EndOfStream)
                    lines.Add(sr.ReadLine());
            return lines.ToArray();
        }
        #endregion

        #region IFileArchive Members
        /// <summary>
        /// Gets the filename extension used by the compression module
        /// </summary>
        public string FilenameExtension { get { return "7z"; } }
        /// <summary>
        /// Gets a friendly name for the compression module
        /// </summary>
        public string DisplayName { get { return Strings.FileArchive7z.DisplayName; } }
        /// <summary>
        /// Gets a description of the compression module
        /// </summary>
        public string Description { get { return Strings.FileArchive7z.Description; } }

        /// <summary>
        /// Gets a list of commands supported by the compression module
        /// </summary>
        public IList<ICommandLineArgument> SupportedCommands
        {
            get
            {
                return new List<ICommandLineArgument>(new ICommandLineArgument[] {
            });
            }
        }

        /// <summary>
        /// Returns a list of files matching the given prefix
        /// </summary>
        /// <param name="prefix">The prefix to match</param>
        /// <returns>A list of files matching the prefix</returns>
        public string[] ListFiles(string prefix)
        {
            List<string> results = new List<string>();
            foreach (var ze in m_fileStore)
            {
                string name = m_pathToFilesystem(ze.FileName);
                if (prefix == null || name.StartsWith(prefix))
                    if (!ze.IsDirectory)
                        results.Add(name);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Returns a list of folders matching the given prefix
        /// </summary>
        /// <param name="prefix">The prefix to match</param>
        /// <returns>A list of folders matching the prefix</returns>
        public string[] ListDirectories(string prefix)
        {
            List<string> results = new List<string>();
            foreach (var ze in m_fileStore)
            {
                string name = m_pathToFilesystem(ze.FileName);
                if (prefix == null || name.StartsWith(prefix))
                    if (ze.IsDirectory)
                        results.Add(name);
            }

            return results.ToArray();
        }

        /// <summary>
        /// Returns a list of entries matching the given prefix
        /// </summary>
        /// <param name="prefix">The prefix to match</param>
        /// <returns>A list of entries matching the prefix</returns>
        public string[] ListEntries(string prefix)
        {
            List<string> results = new List<string>();
            foreach (var ze in m_fileStore)
            {
                string name = m_pathToFilesystem(ze.FileName);
                if (prefix == null || name.StartsWith(prefix))
                {
                    if (ze.IsDirectory)
                        results.Add(Utility.Utility.AppendDirSeparator(name));
                    else
                        results.Add(name);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Opens an file for reading
        /// </summary>
        /// <param name="file">The name of the file to open</param>
        /// <returns>A stream with the file contents</returns>
        public System.IO.Stream OpenRead(string file)
        {
            return new SevenZipStreamWrapper(m_getStream(file));
        }

        /// <summary>
        /// Opens a file for writing
        /// </summary>
        /// <param name="file">The name of the file to write</param>
        /// <returns>A stream that can be updated with file contents</returns>
        public System.IO.Stream OpenWrite(string file)
        {
            m_saveChanges = true;
            return CreateFile(file);
        }

        /// <summary>
        /// Writes all bytes from the array into the file
        /// </summary>
        /// <param name="file">The name of the file</param>
        /// <param name="data">The data contents of the file</param>
        public void WriteAllBytes(string file, byte[] data)
        {
            using (System.IO.Stream s = CreateFile(file))
                s.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes the given lines into the file
        /// </summary>
        /// <param name="file">The name of the file</param>
        /// <param name="data">The lines to write</param>
        public void WriteAllLines(string file, string[] data)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(CreateFile(file)))
                foreach (string s in data)
                    sw.WriteLine(s);
        }

        /// <summary>
        /// Deletes a file from the archive
        /// </summary>
        /// <param name="file">The name of the file to delete</param>
        public void DeleteFile(string file)
        {
            throw new MissingMethodException(Strings.FileArchive7z.DeleteUnsupportedError);
        }

        /// <summary>
        /// Creates a file in the archive and returns a writeable stream
        /// </summary>
        /// <param name="file">The name of the file to create</param>
        /// <returns>A writeable stream for the file contents</returns>
        public System.IO.Stream CreateFile(string file)
        {
            return CreateFile(file, DateTime.Now);
        }

        /// <summary>
        /// Creates a file in the archive and returns a writeable stream
        /// </summary>
        /// <param name="file">The name of the file to create</param>
        /// <param name="lastWrite">The time the file was last written</param>
        /// <returns>A writeable stream for the file contents</returns>
        public System.IO.Stream CreateFile(string file, DateTime lastWrite)
        {
            var ze = new SevenZipEntry();
            ze.FileName = m_pathFromFilesystem(file);
            ze.LastWriteTime = lastWrite;
            ze.IsDirectory = false;
            m_fileStore.Add(ze);

            m_saveChanges = true;

            return new SevenZipStreamWrapper(m_getStream(ze.FileName));
        }

        /// <summary>
        /// Deletes a folder from the archive
        /// </summary>
        /// <param name="file">The name of the folder to delete</param>
        public void DeleteDirectory(string file)
        {
            throw new MissingMethodException(Strings.FileArchive7z.DeleteUnsupportedError);
        }

        /// <summary>
        /// Adds a folder to the archive
        /// </summary>
        /// <param name="file">The name of the folder to create</param>
        public void AddDirectory(string file)
        {
            m_saveChanges = true;
        }

        /// <summary>
        /// Returns a value that indicates if the file exists
        /// </summary>
        /// <param name="file">The name of the file to test existence for</param>
        /// <returns>True if the file exists, false otherwise</returns>
        public bool FileExists(string file)
        {
            return m_getEntry(file) != null && !m_getEntry(file).IsDirectory;
        }

        /// <summary>
        /// Returns a value that indicates if the folder exists
        /// </summary>
        /// <param name="file">The name of the folder to test existence for</param>
        /// <returns>True if the folder exists, false otherwise</returns>
        public bool DirectoryExists(string file)
        {
            return m_getEntry(file) != null && m_getEntry(file).IsDirectory;
        }

        /// <summary>
        /// Gets the last write time for a file
        /// </summary>
        /// <param name="file">The name of the file to query</param>
        /// <returns>The last write time for the file</returns>
        public DateTime GetLastWriteTime(string file)
        {
            if (m_getEntry(file) != null)
                return m_getEntry(file).LastWriteTime;
            else
                throw new Exception(string.Format(Strings.FileArchive7z.FileNotFoundError, file));
        }

        /// <summary>
        /// Gets the current size of the archive
        /// </summary>
        public long Size
        {
            get
            {
                //Return 0 as the size of the file isn't known until it has been built (see on dispose)
                //To calc an estimated size would cause the overall compression module to become
                //extremely slow as this is called every 1024bytes of writing any file...
                return 0;
            }
        }

        /// <summary>
        /// The size of the current unflushed buffer
        /// </summary>
        public long FlushBufferSize
        {
            get
            {
                long tmpSize = 0;

                foreach (var stm in m_fileStore)
                    tmpSize += stm.BaseStream.Length;
               
                return tmpSize;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (m_saveChanges)
            {
                try
                {
                    var outDictionary = new Dictionary<string, Stream>();

                    foreach (var ze in m_fileStore)
                    {
                        var tmpStream = m_getStream(ze.FileName);
                        
                        if (tmpStream.Length > 0)
                            tmpStream.Position = 0;

                        outDictionary.Add(ze.FileName, tmpStream);
                    }

                    //Get rid of the extractor to ensure no locks exits
                    if (m_7zExtractor != null)
                    {
                        m_7zExtractor.Dispose();
                        m_7zExtractor = null;
                    }

                    if (File.Exists(m_fileName))
                        File.Delete(m_fileName);

                    var sW = new StreamWriter(m_fileName);
                    m_7zCompressor = new SevenZipCompressor();

                    //Compress all of the streams
                    if(string.IsNullOrEmpty(m_passkey))
                        m_7zCompressor.CompressStreamDictionary(outDictionary, sW.BaseStream);
                    else
                        m_7zCompressor.CompressStreamDictionary(outDictionary, sW.BaseStream, m_passkey);

                    sW.Flush();
                    sW.Close();
                    sW.Dispose();
                    sW = null;
                }
                catch (Exception e)
                {
                    //TODO: Possibly add a more detailed error message?
                    throw e;
                }
            }

            if (m_7zCompressor != null)
                m_7zCompressor = null;

            if (m_7zExtractor != null)
            {
                m_7zExtractor.Dispose();
                m_7zExtractor = null;
            }

            //Make sure all streams are disposed of. Stops memory leaks
            foreach (var ze in m_fileStore)
                if(ze.BaseStream.CanRead)
                    ze.BaseStream.Dispose();
        }

        #endregion

        #region PrivateClasses
        /// <summary>
        /// Stream wrapper to prevent closing the base stream when disposing the entry stream
        /// </summary>
        private class SevenZipStreamWrapper : Utility.OverrideableStream
        {
            public SevenZipStreamWrapper(System.IO.Stream stream)
                : base(stream)
            {
            }

            protected override void Dispose(bool disposing)
            {
                m_basestream.Position = 0;
            }
        }

        /// <summary>
        /// SevenZipEntry is a class for simply holding and possibly later manipulating streams
        /// and the information about the stream/file
        /// </summary>
        private class SevenZipEntry
        {
            public string FileName { get; set; }
            public int Index { get; set; }
            public DateTime LastWriteTime { get; set; } //TODO: Work out how to append this to the stream?
            public bool IsDirectory { get; set; }
            public Stream BaseStream;

            public SevenZipEntry()
            {
                BaseStream = new MemoryStream();
            }

            public SevenZipEntry(ArchiveFileInfo info)
            {
                FileName = info.FileName;
                Index = info.Index;
                LastWriteTime = info.LastWriteTime;
                IsDirectory = info.IsDirectory;
                BaseStream = new MemoryStream();
            }

            public Stream OutputStream()
            {
                //TODO: Maybe add some way of add the LastWriteTime??
                return BaseStream;
            }
        }

        #endregion
    }
}
