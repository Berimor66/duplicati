using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Duplicati.Library.Compression.Tests.ZipArchives
{
    public enum ZipFiles
    {
        Test1 = 0
    }

    public enum RawZipFiles
    {
        Test1 = 0
    }

    class ZipRouter
    {
        private const string ResourceNamespace = "Duplicati.Library.Compression.Tests.ZipArchives.{0}";
        private const string RawResourceNamespace = "Duplicati.Library.Compression.Tests.ZipArchives.RawFiles.{0}";

        public static string GetZipLocation(ZipFiles target)
        {
            var extractionName = GetExtractionLocation() + GetFileName(target);
            var assembly = Assembly.GetExecutingAssembly();

            if (File.Exists(extractionName))
                File.Delete(extractionName);

            using (var resourceStream = assembly.GetManifestResourceStream(string.Format(ResourceNamespace, GetFileName(target))))
            {
                using (var writer = new StreamWriter(extractionName))
                {
                    Utility.Utility.CopyStream(resourceStream, writer.BaseStream);
                }
            }

            return extractionName;
        }

        public static string GetRawFolder(RawZipFiles target)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames().ToList();
            var nameSpace = GetRawNamespace(target);
            var extractionLocation = GetExtractionLocation() + string.Format(@"RawFiles\{0}\", GetRawFileName(target));

            resourceNames = resourceNames.Where(x => x.StartsWith(nameSpace)).ToList();

            if (!Directory.Exists(extractionLocation))
                Directory.CreateDirectory(extractionLocation);

            foreach (var file in resourceNames)
            {
                var extractionName = extractionLocation + file.Substring(nameSpace.Length + 1);

                if (File.Exists(extractionName))
                    File.Delete(extractionName);

                using (var resourceStream = assembly.GetManifestResourceStream(file))
                {
                    using (var writer = new StreamWriter(extractionName))
                    {
                        Utility.Utility.CopyStream(resourceStream, writer.BaseStream);
                    }
                }
            }

            return extractionLocation;
        }

        private static string GetRawNamespace(RawZipFiles target)
        {
            return string.Format(RawResourceNamespace, GetRawFileName(target));
        }

        private static string GetRawFileName(RawZipFiles target)
        {
            switch (target)
            {
                case RawZipFiles.Test1:
                    return "Test1";
                    break;
                default:
                    throw new Exception("Unknown state");
            }
        }

        private static string GetFileName(ZipFiles target)
        {
            switch (target)
            {
                case ZipFiles.Test1:
                    return "Test1.zip";
            }

            throw new Exception("Unknown file reference");
        }

        private static string GetExtractionLocation()
        {
            var currentLocation = string.Format("{0}\\ZipTests\\", Directory.GetCurrentDirectory());

            if (!Directory.Exists(currentLocation))
                Directory.CreateDirectory(currentLocation);

            return currentLocation;
        }
    }
}
