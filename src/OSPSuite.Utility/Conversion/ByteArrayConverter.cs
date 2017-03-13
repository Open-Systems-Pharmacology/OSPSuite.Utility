using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Conversion
{
   public class ByteArrayConverter
   {
      public byte[] ConvertToByteArray<T>(T[] arrayToConvert)
      {
         if (typeof(T).IsAnImplementationOf<byte>())
            return arrayToConvert as byte[];

         var formatter = new BinaryFormatter();
         using (var mem = new MemoryStream())
         {
            formatter.Serialize(mem, arrayToConvert);
            return mem.ToArray();
         }
      }

      public T[] ConvertFromByteArray<T>(byte[] byteArray)
      {
         var formatter = new BinaryFormatter();
         using (var mem = new MemoryStream(byteArray))
         {
            return (T[]) formatter.Deserialize(mem);
         }
      }
   }
}