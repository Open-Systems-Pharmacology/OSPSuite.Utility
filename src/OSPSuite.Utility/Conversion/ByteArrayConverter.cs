using System;
using System.Formats.Nrbf;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Conversion
{
   public class ByteArrayConverter
   {
      private readonly JsonSerializerOptions _options = new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };

      public byte[] ConvertToByteArray<T>(T[] arrayToConvert)
      {
         if (typeof(T).IsAnImplementationOf<byte>())
            return arrayToConvert as byte[];

         return JsonSerializer.SerializeToUtf8Bytes(arrayToConvert, _options);
      }

      public T[] ConvertFromByteArray<T>(byte[] byteArray)
      {
         try
         {
            return JsonSerializer.Deserialize<T[]>(byteArray, _options);
         }
         catch (JsonException)
         {
            return convertFromLegacyNrbfPayload<T>(byteArray);
         }
      }

      // Reads payloads written by BinaryFormatter before the JSON migration. Uses
      // NrbfDecoder so the path works on .NET 9+ without
      // EnableUnsafeBinaryFormatterSerialization or the unsupported compat package.
      // See: https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-migration-guide/read-nrbf-payloads
      private static T[] convertFromLegacyNrbfPayload<T>(byte[] byteArray)
      {
         using var stream = new MemoryStream(byteArray);
         var record = NrbfDecoder.Decode(stream);

         if (record is SZArrayRecord<T> primitiveArray)
            return primitiveArray.GetArray();

         throw new InvalidOperationException(
            $"Unsupported legacy NRBF payload for element type '{typeof(T).FullName}'.");
      }
   }
}
