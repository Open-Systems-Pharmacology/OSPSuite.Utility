using System;
using System.Text;

namespace OSPSuite.Utility.Compression
{
   public interface IStringCompression
   {
      string Compress(string stringToCompress);
      string Decompress(string stringToDecompress);
   }

   public class StringCompression : IStringCompression
   {
      private readonly ICompression _compression;

      public StringCompression(ICompression compression)
      {
         _compression = compression;
      }

      public string Compress(string stringToCompress)
      {
         byte[] input = Encoding.UTF8.GetBytes(stringToCompress);
         var compressesInput = _compression.Compress(input);
         return Convert.ToBase64String(compressesInput);
      }

      public string Decompress(string stringToDecompress)
      {
         var encodedDataAsBytes = Convert.FromBase64String(stringToDecompress);
         var decompressData = _compression.Decompress(encodedDataAsBytes);
         return Encoding.UTF8.GetString(decompressData);
      }
   }
}