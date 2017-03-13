using System;
using System.Collections.Generic;
using System.Reflection;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Extensions
{
   public static class TypeExtensions
   {
      public static bool IsInteger(this Type type)
      {
         return TypeInspector.IsIntegerType(type);
      }

      public static bool IsUnsignedInteger(this Type type)
      {
         return TypeInspector.IsUnsignedIntegerType(type);
      }

      public static bool IsSignedInteger(this Type type)
      {
         return TypeInspector.IsSignedIntegerType(type);
      }

      public static bool IsDouble(this Type type)
      {
         return TypeInspector.IsDoubleType(type);
      }

      public static bool IsNumeric(this Type type)
      {
         return TypeInspector.IsNumericType(type);
      }

      /// <summary>
      ///    Returns true if the
      ///    <para>type</para>
      ///    is a nullable type otherwise false
      /// </summary>
      public static bool IsNullableType(this Type type)
      {
         return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
      }

      /// <summary>
      ///    Rturns true if the type
      ///    <para>type</para>
      ///    is an implementation of the generic type
      ///    <para>T</para>
      ///    otherwise false
      /// </summary>
      public static bool IsAnImplementationOf<T>(this Type type)
      {
         return type.IsAnImplementationOf(typeof(T));
      }

      /// <summary>
      ///    Returns true if the type
      ///    <para>type</para>
      ///    is an implementation of the generic type
      ///    <para>genericType</para>
      ///    otherwise false
      /// </summary>
      public static bool IsAnImplementationOf(this Type type, Type genericType)
      {
         return genericType.IsAssignableFrom(type);
      }

      /// <summary>
      ///    Creates an instance of the specified
      ///    <para>type</para>
      ///    using the parameter less constructor if it exists
      ///    and return an object casted to T
      /// </summary>
      /// <typeparam name="T">The object type to return</typeparam>
      /// <param name="type">The <see cref="System.Type" /> for which an instance should be created</param>
      /// <exception cref="MissingParameterLessConstructorException">
      ///    Thrown when there is no parameter less constructor defined for <see cref="System.Type" />.
      /// </exception>
      public static T CreateInstance<T>(this Type type)
      {
         return type.CreateInstance().DowncastTo<T>();
      }

      /// <summary>
      ///    Creates an instance of the specified
      ///    <para>type</para>
      ///    using the parameter less constructor if it exists
      ///    and return the created object
      /// </summary>
      /// <param name="type">The <see cref="System.Type" /> for which an instance should be created</param>
      /// <exception cref="MissingParameterLessConstructorException">
      ///    Thrown when there is no parameter less constructor defined for <see cref="System.Type" />.
      /// </exception>
      public static object CreateInstance(this Type type)
      {
         var constructor = ReflectionHelper.GetDefaultConstructor(type);

         if (constructor == null)
            throw new MissingParameterLessConstructorException(type);

         return constructor.Invoke(null);
      }

      /// <summary>
      ///    Creates an instance of the specified
      ///    <para>type</para>
      ///    using the constructor matching the number of parameters
      ///    provided in the
      ///    <param>constructorParameters</param>
      ///    and return the created object casted to T
      /// </summary>
      /// <typeparam name="T">The object type to return</typeparam>
      /// <param name="type">The <see cref="System.Type" /> for which an instance should be created</param>
      /// <param name="constructorParameters">a params array of parmaeters used in the constructor</param>
      /// <exception cref="MissingConstructorWithParametersException">
      ///    Thrown when there is constructor defined for <see cref="System.Type" /> with the accurate number of parameters
      /// </exception>
      public static T CreateInstanceUsing<T>(this Type type, params object[] constructorParameters)
      {
         return type.CreateInstanceUsing(constructorParameters).DowncastTo<T>();
      }

      /// <summary>
      ///    Creates an instance of the specified
      ///    <para>type</para>
      ///    using the constructor matching the number of parameters
      ///    provided in the
      ///    <param>constructorParameters</param>
      ///    and return the created object.
      /// </summary>
      /// <param name="type">The <see cref="System.Type" /> for which an instance should be created</param>
      /// <param name="constructorParameters">a params array of parmaeters used in the constructor</param>
      /// <exception cref="MissingConstructorWithParametersException">
      ///    Thrown when there is constructor defined for <see cref="System.Type" /> with the accurate number of parameters
      /// </exception>
      public static object CreateInstanceUsing(this Type type, params object[] constructorParameters)
      {
         var constructor = ReflectionHelper.GetConstructor(type, constructorParameters);

         if (constructor == null)
            throw new MissingConstructorWithParametersException(type, constructorParameters);

         return constructor.Invoke(constructorParameters);
      }

      /// <summary>
      ///    Determines if the <see cref="System.Type" /> is a non creatable class.
      /// </summary>
      /// <param name="type">The <see cref="System.Type" /> to check.</param>
      /// <returns><see langword="true" /> if the <see cref="System.Type" /> is an Abstract Class or an Interface.</returns>
      public static bool IsAbstractClass(this Type type)
      {
         return (type.IsAbstract || type.IsInterface);
      }

      /// <summary>
      ///    Returns all instance fields define for the type and its base type
      /// </summary>
      /// <param name="type"> The <see cref="System.Type" /> to inspect for all the fields</param>
      /// <returns>
      ///    An  Eumeration of <see cref="FieldInfo" /> for each field avaialble in the given <see cref="System.Type" />
      ///    hierarchy.
      /// </returns>
      public static IEnumerable<FieldInfo> AllFields(this Type type)
      {
         return ReflectionHelper.AllFieldsFor(type);
      }

      /// <summary>
      ///    Returns all instance properties define for the type and its base type
      /// </summary>
      /// <param name="type"> The <see cref="System.Type" /> to inspect for all the properties</param>
      /// <returns>
      ///    An  Eumeration of <see cref="PropertyInfo" /> for each properties avaialble in the given <see cref="System.Type" />
      ///    hierarchy.
      /// </returns>
      public static IEnumerable<PropertyInfo> AllProperties(this Type type)
      {
         return ReflectionHelper.AllPropertiesFor(type);
      }

      /// <summary>
      ///    Returns the assembly name of the assembly where the given type is defined
      /// </summary>
      /// <param name="type">Type for which the assembly name is required</param>
      /// <returns>The assembly name. This is not the assembly full name as we only return the name of the assembly</returns>
      public static string AssemblyName(this Type type)
      {
         return ReflectionHelper.AssemblyNameFor(type);
      }
   }
}