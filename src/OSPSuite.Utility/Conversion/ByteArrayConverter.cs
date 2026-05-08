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

         // Nullable<U>[] is stored by BinaryFormatter as an SZArray of objects
         // whose elements are either null (for a null Nullable<U>) or a boxed U
         // (for a non-null Nullable<U>). NrbfDecoder surfaces this as
         // SZArrayRecord<SerializationRecord> with PrimitiveTypeRecord<U> entries.
         if (Nullable.GetUnderlyingType(typeof(T)) is not null
             && record is SZArrayRecord<SerializationRecord> objectArray)
         {
            var elements = objectArray.GetArray();
            var result = new T[elements.Length];
            for (var i = 0; i < elements.Length; i++)
            {
               var element = elements[i];
               if (element is null)
                  continue;

               if (element is PrimitiveTypeRecord primitive)
               {
                  result[i] = (T)primitive.Value;
                  continue;
               }

               throw new InvalidOperationException(
                  $"Unsupported NRBF element record '{element.GetType().Name}' inside Nullable<{typeof(T).GenericTypeArguments[0].Name}> array.");
            }

            return result;
         }

         throw new InvalidOperationException(
            $"Unsupported legacy NRBF payload for element type '{typeof(T).FullName}'.");
      }
   }
}
