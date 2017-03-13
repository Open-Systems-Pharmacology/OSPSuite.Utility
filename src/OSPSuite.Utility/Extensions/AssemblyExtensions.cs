using System;
using System.Collections.Generic;
using System.Reflection;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Extensions
{
   public static class AssemblyExtensions
   {
      public static IEnumerable<Type> GetAllConcreteTypesImplementing<T>(this Assembly assembly)
      {
         return assembly.GetConcreteTypesImplementing<T>(false);
      }

      public static IEnumerable<Type> GetConcreteTypesImplementing<T>(this Assembly assembly, bool onlyExportedTypes)
      {
         return ReflectionHelper.GetConcreteTypesFromAssemblyImplementing<T>(assembly, onlyExportedTypes);
      }

      public static IEnumerable<Type> GetAllConcreteTypesImplementing(this Assembly assembly, Type typeToImplement)
      {
         return assembly.GetConcreteTypesImplementing(typeToImplement, false);
      }

      public static IEnumerable<Type> GetConcreteTypesImplementing(this Assembly assembly, Type typeToImplement, bool onlyExportedTypes)
      {
         return ReflectionHelper.GetConcreteTypesFromAssemblyImplementing(assembly, typeToImplement, onlyExportedTypes);
      }
   }
}