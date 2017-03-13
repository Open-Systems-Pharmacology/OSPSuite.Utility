using System;

namespace OSPSuite.Utility.Reflection
{
   public static class TypeInspector
   {
      public static bool IsIntegerType(Type type)
      {
         return IsSignedIntegerType(type) || IsUnsignedIntegerType(type);
      }

      public static bool IsDoubleType(Type type)
      {
         if (type == typeof(double))
            return true;
         if (type == typeof(float))
            return true;
         if (type == typeof(decimal))
            return true;
         if (type == typeof(double?))
            return true;
         if (type == typeof(float?))
            return true;
         if (type == typeof(decimal?))
            return true;

         return false;
      }

      public static bool IsNumericType(Type type)
      {
         return IsIntegerType(type) || IsDoubleType(type);
      }

      public static bool IsUnsignedIntegerType(Type type)
      {
         if (type == typeof(uint))
            return true;
         if (type == typeof(ulong))
            return true;
         if (type == typeof(ushort))
            return true;
         if (type == typeof(byte))
            return true;
         if (type == typeof(uint?))
            return true;
         if (type == typeof(ulong?))
            return true;
         if (type == typeof(ushort?))
            return true;
         if (type == typeof(byte?))
            return true;

         return false;
      }

      public static bool IsSignedIntegerType(Type type)
      {
         if (type == typeof(int))
            return true;
         if (type == typeof(long))
            return true;
         if (type == typeof(short))
            return true;
         if (type == typeof(sbyte))
            return true;
         if (type == typeof(int?))
            return true;
         if (type == typeof(long?))
            return true;
         if (type == typeof(short?))
            return true;
         if (type == typeof(sbyte?))
            return true;

         return false;
      }
   }
}