using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Duplicati.Library.Compression.Tests.ZipArchives
{
    public enum ZipFiles
    {
        Test1 = 0
    }

    class ZipRouter
    {
        private const string ResourceNamespace = "Duplicati.Library.Compression.Tests.ZipArchives.{0}";

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

        private static string GetFileName(ZipFiles target)
        {
            switch(target)
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
