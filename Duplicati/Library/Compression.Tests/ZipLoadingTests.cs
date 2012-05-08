using System;
using System.Collections;
using System.Collections.Generic;
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
            public abstract class base_given : SpecsFor<FileArchiveZip>
            {
                protected override void InitializeClassUnderTest()
                {
                    SUT = new FileArchiveZip(File, Commands);
                }

                protected virtual string File { get { return string.Empty; } }

                protected virtual Dictionary<string, string> Commands
                {
                    get
                    {
                        return new Dictionary<string, string>();
                    }
                }

                public override void TearDown()
                {
                    System.IO.File.Delete(File);
                }
            }

            public abstract class test_zip_file_one : base_given
            {
                protected override string File
                {
                    get
                    {
                        return ZipRouter.GetZipLocation(ZipFiles.Test1);
                    }
                }

                protected IDictionary<string, Files> ExpectedFiles
                {
                    get
                    {
                        return new Dictionary<string, Files>
                        {
                            {"Tests/File1_Picture.jpg", new Files{Hash = "54-C2-F1-A1-EB-6F-12-D6-81-A5-C7-07-84-21-A5-50-0C-EE-02-AD", Size = 620888}},
                            {"Tests/File2_Picture.png", new Files{Hash = "B3-90-38-87-D3-CC-0E-89-39-BD-AD-7A-0E-E8-79-19-82-D7-84-86", Size = 112861}},
                            {"Tests/File3_Code.cs", new Files{Hash = "C2-C3-75-CF-7B-00-CF-14-6D-18-D3-4D-57-CF-FB-0B-C9-B5-F1-64", Size = 10969}},
                            {"Tests/File4_Code.cs", new Files{Hash = "79-6D-0F-6F-95-16-DD-97-EE-FE-39-0F-09-AB-1A-1A-B7-8C-F2-1E", Size = 2921}}
                        };
                    }
                }

                protected class Files
                {
                    public long Size { get; set; }
                    public string Hash { get; set; }
                }
            }

        }

        [TestFixture]
        public class when_testing_test_file_one : given.test_zip_file_one
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
                    SUT.OpenRead(file).Length.ShouldEqual(ExpectedFiles[file].Size);
                }
            }

            [Test]
            public void then_files_should_have_the_correct_hashes()
            {
                foreach (var file in SUT.ListFiles(string.Empty))
                {
                    using (var cryptoProvider = new SHA1CryptoServiceProvider())
                    {
                        var hash = BitConverter.ToString(cryptoProvider.ComputeHash(SUT.OpenRead(file)));

                        hash.ShouldEqual(ExpectedFiles[file].Hash);
                    }
                }
            }
        }
    }
}
