using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Extensions
{
   public static class ConversionExtensions
   {
      /// <summary>
      ///    Casts
      ///    <para>objectToCast</para>
      ///    to the specified generic type
      ///    <para>T</para>
      /// </summary>
      /// <returns>the object casted to T</returns>
      public static T DowncastTo<T>(this object objectToCast)
      {
         return (T) objectToCast;
      }

      /// <summary>
      ///    Tries to convert the
      ///    <para>itemToConvert</para>
      ///    to the specified generic type
      ///    <para>T</para>
      ///    using if possible the convertible approach.
      /// </summary>
      /// <returns>the object converted to T</returns>
      public static T ConvertedTo<T>(this object itemToConvert)
      {
         //we convert from a string to a numeric type
         if (typeof(T).IsNumeric() && itemToConvert.IsAnImplementationOf<string>())
         {
            var stringToConvert = itemToConvert.ToString().Replace(",", ".");
            if (typeof(T).IsNullableType())
            {
               var converter = new NullableConverter(typeof(T));
               if (string.IsNullOrEmpty(stringToConvert))
                  return default(T);

               var value = Convert.ChangeType(stringToConvert, converter.UnderlyingType, NumberFormatInfo.InvariantInfo);
               return converter.ConvertFrom(value).DowncastTo<T>();
            }

            return (T) Convert.ChangeType(stringToConvert, typeof(T), NumberFormatInfo.InvariantInfo);
         }

         if (typeof(T).IsNullableType())
            return new NullableConverter(typeof(T)).ConvertFrom(itemToConvert).DowncastTo<T>();

         if (itemToConvert.IsAnImplementationOf<IConvertible>())
            return (T) Convert.ChangeType(itemToConvert, typeof(T), NumberFormatInfo.InvariantInfo);

         //last chance
         return itemToConvert.DowncastTo<T>();
      }

      /// <summary>
      ///    Returns true if the object
      ///    <para>item</para>
      ///    is an implementation of the generic type
      ///    <para>T</para>
      ///    otherwise false
      /// </summary>
      public static bool IsAnImplementationOf<T>(this object item)
      {
         return item is T;
      }

      /// <summary>
      ///    Returns true if the object
      ///    <para>item</para>
      ///    is an implementation of the type
      ///    <para>type</para>
      ///    otherwise false
      /// </summary>
      public static bool IsAnImplementationOf(this object item, Type type)
      {
         return item != null && item.GetType().IsAnImplementationOf(type);
      }

      /// <summary>
      ///    Returns an enumeration of TypeForGenericType for the given object and the generic interface interfaceType
      /// </summary>
      /// <param name="instance">Object for which the search should be performed</param>
      /// <param name="interfaceType">Generic interface type that we would like to parse in the insance hierarchy</param>
      /// <example>
      ///    MyObject : IOneInterface[double], AnotherImplemtation
      ///    The call myObject.GetDeclaredTypesForGeneric(typeof(IOneInterface[double])) will return a
      ///    TypeForGeneric(double,IOneInterface[double]),
      /// </example>
      public static IEnumerable<TypeForGenericType> GetDeclaredTypesForGeneric(this object instance, Type interfaceType)
      {
         return instance.GetType().GetDeclaredTypesForGeneric(interfaceType);
      }

      public static IEnumerable<TypeForGenericType> GetDeclaredTypesForGeneric<T>(this object instance)
      {
         var interfaceType = typeof(T);
         return instance.GetType().GetDeclaredTypesForGeneric(interfaceType);
      }

      public static IEnumerable<TypeForGenericType> GetDeclaredTypesForGeneric<T>(this object instance, T interfaceType)
      {
         return instance.GetDeclaredTypesForGeneric<T>();
      }

      public static IEnumerable<TypeForGenericType> GetDeclaredTypesForGeneric<T>(this Type type)
      {
         var interfaceType = typeof(T);
         return type.GetDeclaredTypesForGeneric(interfaceType);
      }

      public static IEnumerable<TypeForGenericType> GetDeclaredTypesForGeneric<T>(this Type type, T interfaceType)
      {
         return type.GetDeclaredTypesForGeneric(typeof(T));
      }

      public static IEnumerable<TypeForGenericType> GetDeclaredTypesForGeneric(this Type type, Type interfaceType)
      {
         foreach (var generic in type.getGenericInterfacesFor(interfaceType))
         {
            foreach (var arg in generic.GetGenericArguments())
            {
               yield return new TypeForGenericType(arg, generic);
            }
         }
      }

      /// <summary>
      ///    Returns all generic implementation implemented by the given type
      /// </summary>
      /// <example>
      ///    MyObject : IOneInterface[], AnotherImplemtation
      ///    The call GetGenericInterfaceFor(typeof(MyObject), typeof(IOneInterface[])) will return  IOneInterface[]
      /// </example>
      private static IEnumerable<Type> getGenericInterfacesFor(this Type type, Type interfaceType)
      {
         var candidates = type.GetInterfaces();
         foreach (var candidate in candidates)
         {
            if (!candidate.IsGenericType)
            {
               continue;
            }

            var definition = candidate.GetGenericTypeDefinition();
            if (definition == interfaceType)
            {
               yield return candidate;
            }
         }
      }
   }
}