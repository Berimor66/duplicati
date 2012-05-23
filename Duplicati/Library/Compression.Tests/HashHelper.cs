using System;
using System.IO;
using System.Security.Cryptography;

namespace Duplicati.Library.Compression.Tests
{
    public static class HashHelper
    {
         public static string GenerateHash(Stream stream)
         {
             using (var cryptoProvider = new SHA1CryptoServiceProvider())
             {
                 return BitConverter.ToString(cryptoProvider.ComputeHash(stream));
             }
         }
    }
}