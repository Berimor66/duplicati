using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Duplicati.Library.Compression.Tests.ZipArchives;
using NUnit.Framework;
using SpecsFor;
using Should;
using System.Linq;
using SpecsFor.ShouldExtensions;


namespace Duplicati.Library.Compression.Tests
{
    class ZipLoadingTests
    {
        public static class given
        {
            public class Files
            {
                public long Size { get; set; }
                public string Hash { get; set; }

                public static IDictionary<string, Files> GetFileInfo(bool includeFolder)
                {
                    var prefix = includeFolder ? @"Tests\" : string.Empty;
                    return new Dictionary<string, Files>
                        {
                            {prefix + "File1_Picture.jpg", new Files{Hash = "54-C2-F1-A1-EB-6F-12-D6-81-A5-C7-07-84-21-A5-50-0C-EE-02-AD", Size = 620888}},
                            {prefix + "File2_Picture.png", new Files{Hash = "B3-90-38-87-D3-CC-0E-89-39-BD-AD-7A-0E-E8-79-19-82-D7-84-86", Size = 112861}},
                            {prefix + "File3_Code.cs", new Files{Hash = "C2-C3-75-CF-7B-00-CF-14-6D-18-D3-4D-57-CF-FB-0B-C9-B5-F1-64", Size = 10969}},
                            {prefix + "File4_Code.cs", new Files{Hash = "79-6D-0F-6F-95-16-DD-97-EE-FE-39-0F-09-AB-1A-1A-B7-8C-F2-1E", Size = 2921}}
                        };
                }
            }

            public abstract class base_given : SpecsFor<FileArchiveZip>
            {
                protected override void InitializeClassUnderTest()
                {
                    SUT = new FileArchiveZip(File, Commands);
                }

                protected virtual string File { get { return String.Empty; } }

                protected virtual Dictionary<string, string> Commands
                {
                    get
                    {
                        return new Dictionary<string, string>();
                    }
                }

                public override void TearDown()
                {
                    if (System.IO.File.Exists(File))
                        System.IO.File.Delete(File);
                }
            }

            public abstract class test_zip_reading_file_one : base_given
            {
                protected override string File
                {
                    get
                    {
                        return ZipRouter.GetZipLocation(ZipFiles.Test1);
                    }
                }

                protected IDictionary<string, Files> ExpectedFiles { get { return Files.GetFileInfo(true); } }
            }

            public abstract class test_zip_writing_file_one : base_given
            {
                protected string Directory { get; set; }

                protected override string File
                {
                    get
                    {
                        Directory = ZipRouter.GetRawFolder(RawZipFiles.Test1);
                        var fileName = Directory + @"Compile\";

                        if (!System.IO.Directory.Exists(Directory))
                            System.IO.Directory.CreateDirectory(Directory);

                        fileName += "Test1.zip";

                        if (System.IO.File.Exists(fileName))
                            System.IO.File.Delete(fileName);

                        return fileName;
                    }
                }

                protected IDictionary<string, Files> ExpectedFiles { get { return Files.GetFileInfo(false); } }

                public override void TearDown()
                {
                    System.IO.Directory.Delete(Directory, true);
                    base.TearDown();
                }
            }
        }

        [TestFixture]
        public class when_testing_test_reading_file_one : given.test_zip_reading_file_one
        {
            [Test]
            public void then_should_contain_four_files()
            {
                SUT.ListFiles(string.Empty).Count().ShouldEqual(4);
            }

            [Test]
            public void then_filenames_should_be_correct()
            {
                SUT.ListFiles(string.Empty).ShouldLookLike(ExpectedFiles.Select(x => x.Key).ToArray());
            }

            [Test]
            public void then_files_should_have_the_correct_file_sizes()
            {
                foreach (var file in SUT.ListFiles(string.Empty))
                {
                    using (var openRead = SUT.OpenRead(file))
                        openRead.Length.ShouldEqual(ExpectedFiles[file].Size);
                }
            }

            [Test]
            public void then_files_should_have_the_correct_hashes()
            {
                foreach (var file in SUT.ListFiles(string.Empty))
                {
                    using (var fileStream = SUT.OpenRead(file))
                    {
                        var hash = HashHelper.GenerateHash(fileStream);

                        hash.ShouldEqual(ExpectedFiles[file].Hash);
                    }
                }
            }
        }

        [TestFixture]
        public class when_testing_compression_of_file_one : given.test_zip_writing_file_one
        {
            [Test]
            public void then_file_to_compress_should_be_correct_before_compression()
            {
                var filesToCompress = System.IO.Directory.GetFiles(Directory);

                filesToCompress.Count().ShouldEqual(4);

                foreach (var fileLocation in filesToCompress)
                {
                    var fileName = fileLocation.Substring(fileLocation.LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                    using (var reader = new StreamReader(fileLocation))
                    {
                        var expected = ExpectedFiles.First(x => x.Key == fileName).Value;
                        reader.BaseStream.Length.ShouldEqual(expected.Size);
                        HashHelper.GenerateHash(reader.BaseStream).ShouldEqual(expected.Hash);
                    }
                }
            }
        }
    }
}
