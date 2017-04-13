using System;

namespace OSPSuite.Utility.Extensions
{
   public static class StringExtensions
   {
      [Obsolete]
      public static string FormatWith(this string formattedString, params object[] arguments)
      {
         return string.Format(formattedString, arguments);
      }

      public static bool IsNullOrEmpty(this string stringToCheck)
      {
         return string.IsNullOrEmpty(stringToCheck);
      }

      public static float[] ToFloatArray(this string byteString)
      {
         return byteString.ToByteArray().ToFloatArray();
      }

      public static double[] ToDoubleArray(this string byteString)
      {
         return byteString.ToByteArray().ToDoubleArray();
      }

      public static double?[] ToNullableDoubleArray(this string byteString)
      {
         return byteString.ToByteArray().ToNullableDoubleArray();
      }

      public static float?[] ToNullableFloatArray(this string byteString)
      {
         return byteString.ToByteArray().ToNullableFloatArray();
      }

      public static int[] ToIntegerArray(this string byteString)
      {
         return byteString.ToByteArray().ToIntegerArray();
      }

      public static string[] ToStringArray(this string byteString)
      {
         return byteString.ToByteArray().ToStringArray();
      }

      public static byte[] ToByteArray(this string byteString)
      {
         return Convert.FromBase64String(byteString);
      }
   }
}