using System;
using OSPSuite.Utility.Conversion;

namespace OSPSuite.Utility.Extensions
{
   public static class ArrayExtensions
   {
      public static T[] RedimPreserve<T>(this T[] arrayToRedim, int newDimension)
      {
         if (newDimension <= 0)
            throw new ArgumentException();

         if (arrayToRedim == null)
            return new T[newDimension];

         var tmpArray = new T[newDimension];

         Array.Copy(arrayToRedim, tmpArray, Math.Min(arrayToRedim.Length, tmpArray.Length));

         return tmpArray;
      }

      public static byte[] ToByteArray<T>(this T[] array)
      {
         return new ByteArrayConverter().ConvertToByteArray(array);
      }

      public static string ToByteString<T>(this T[] array)
      {
         return Convert.ToBase64String(array.ToByteArray());
      }

      public static float[] ToFloatArray(this byte[] byteArray)
      {
         return new ByteArrayConverter().ConvertFromByteArray<float>(byteArray);
      }

      public static double[] ToDoubleArray(this byte[] byteArray)
      {
         return new ByteArrayConverter().ConvertFromByteArray<double>(byteArray);
      }

      public static double?[] ToNullableDoubleArray(this byte[] byteArray)
      {
         return new ByteArrayConverter().ConvertFromByteArray<double?>(byteArray);
      }

      public static float?[] ToNullableFloatArray(this byte[] byteArray)
      {
         return new ByteArrayConverter().ConvertFromByteArray<float?>(byteArray);
      }

      public static int[] ToIntegerArray(this byte[] byteArray)
      {
         return new ByteArrayConverter().ConvertFromByteArray<int>(byteArray);
      }

      public static string[] ToStringArray(this byte[] byteArray)
      {
         return new ByteArrayConverter().ConvertFromByteArray<string>(byteArray);
      }
   }
}