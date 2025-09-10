using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Conversion
{
   public class ByteArrayConverter
   {
      private readonly JsonSerializerOptions _options = new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };

      public ByteArrayConverter()
      {
      }

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
            return convertFromByteArrayUnsafe<T>(byteArray);
         }
      }

      /// <summary>
      ///    Provides compatibility for saved projects. Must be removed after .NET8
      /// </summary>
      private T[] convertFromByteArrayUnsafe<T>(byte[] byteArray)
      {
         var formatter = new BinaryFormatter();
         using (var mem = new MemoryStream(byteArray))
         {
            return (T[])formatter.Deserialize(mem);
         }
      }
   }
}