using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Reflection
{
   public static class ReflectionHelper
   {
      public const BindingFlags AnyVisibilityInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      public static PropertyInfo PropertyFor<TOBject, TProperty>(Expression<Func<TOBject, TProperty>> expressionToInspect)
      {
         MemberExpression memberExpression = GetMemberExpression(expressionToInspect);
         var propertyInfo = memberExpression.Member as PropertyInfo;
         if (propertyInfo == null) throw new ArgumentException("Only property access are allowed", "expressionToInspect");
         return propertyInfo;
      }

      public static MemberExpression GetMemberExpression<TDelegate>(Expression<TDelegate> expression)
      {
         return GetMemberExpression(expression, true);
      }

      public static MemberExpression GetMemberExpression<TDelegate>(Expression<TDelegate> expression, bool enforceCheck)
      {
         MemberExpression memberExpression = null;
         if (expression.Body.NodeType == ExpressionType.Convert)
         {
            var body = (UnaryExpression) expression.Body;
            memberExpression = body.Operand as MemberExpression;
         }
         else if (expression.Body.NodeType == ExpressionType.MemberAccess)
         {
            memberExpression = expression.Body as MemberExpression;
         }

         if (enforceCheck && memberExpression == null)
         {
            throw new ArgumentException("Not a member access", "expression");
         }

         return memberExpression;
      }

      public static MethodInfo MethodFor<T>(Expression<Func<T, object>> expression)
      {
         var methodCall = (MethodCallExpression) expression.Body;
         return methodCall.Method;
      }

      public static MethodInfo MethodFor<T, TResult>(Expression<Func<T, TResult>> expression)
      {
         var methodCall = (MethodCallExpression) expression.Body;
         return methodCall.Method;
      }

      public static MethodInfo MethodFor<T, U, V>(Expression<Func<T, U, V>> expression)
      {
         var methodCall = (MethodCallExpression) expression.Body;
         return methodCall.Method;
      }

      public static MethodInfo MethodFor<T>(Expression<Action<T>> expression)
      {
         var methodCall = (MethodCallExpression) expression.Body;
         return methodCall.Method;
      }

      public static MethodInfo MethodFor(Expression<Func<object>> expression)
      {
         var methodCall = (MethodCallExpression) expression.Body;
         return methodCall.Method;
      }

      /// <summary>
      ///    Gets the default no arg constructor for the <see cref="System.Type" />.
      /// </summary>
      /// <param name="type">The <see cref="System.Type" /> to find the constructor for.</param>
      /// <returns>
      ///    The <see cref="ConstructorInfo" /> for the no argument constructor, or <see langword="null" /> if the
      ///    <c>type</c> is an abstract class.
      /// </returns>
      /// <exception cref="MissingParameterLessConstructorException">
      ///    Thrown when there is a problem calling the method GetConstructor on <see cref="System.Type" />.
      /// </exception>
      public static ConstructorInfo GetDefaultConstructor(Type type)
      {
         if (type.IsAbstractClass())
            return null;

         try
         {
            ConstructorInfo constructor = type.GetConstructor(AnyVisibilityInstance, null, CallingConventions.HasThis, Type.EmptyTypes, null);
            return constructor;
         }
         catch (Exception e)
         {
            throw new MissingParameterLessConstructorException(type, e);
         }
      }

      /// <summary>
      ///    Finds the constructor that takes the parameters.
      /// </summary>
      /// <param name="type">The <see cref="System.Type" /> to find the constructor in.</param>
      /// <param name="constructorParameters">The objects used als parameter for the constructor to retrieve.</param>
      /// <returns>
      ///    An <see cref="ConstructorInfo" /> that can be used to create the type with
      ///    the specified parameters. Return null if not found
      /// </returns>
      public static ConstructorInfo GetConstructor(Type type, params object[] constructorParameters)
      {
         ConstructorInfo[] candidates = type.GetConstructors(AnyVisibilityInstance);

         foreach (ConstructorInfo constructor in candidates)
         {
            ParameterInfo[] parameters = constructor.GetParameters();

            if (parameters.Length == constructorParameters.Length)
            {
               bool found = true;

               for (int j = 0; j < parameters.Length; j++)
               {
                  bool ok = parameters[j].ParameterType.IsInstanceOfType(constructorParameters[j]);

                  if (!ok)
                  {
                     found = false;
                     break;
                  }
               }

               if (found)
               {
                  return constructor;
               }
            }
         }

         return null;
      }

      /// <summary>
      ///    Return all instance fields define for the type and its base type
      /// </summary>
      /// <param name="type"> The <see cref="System.Type" /> to inspect for all the fields</param>
      /// <returns>
      ///    An  Eumeration of <see cref="FieldInfo" /> for each field avaialble in the given <see cref="System.Type" />
      ///    hierarchy.
      /// </returns>
      public static IEnumerable<FieldInfo> AllFieldsFor(Type type)
      {
         var allFields = new List<FieldInfo>();
         findFields(allFields, type);
         return allFields;
      }

      public static IEnumerable<PropertyInfo> AllPropertiesFor(Type type)
      {
         var allProperties = new List<PropertyInfo>();
         findPoperties(allProperties, type);
         return allProperties;
      }

      private static void findPoperties(IList<PropertyInfo> properties, Type type)
      {
         findTypeElements(properties, t => t.GetProperties(AnyVisibilityInstance), type);
      }

      private static void findFields(IList<FieldInfo> fields, Type type)
      {
         findTypeElements(fields, t => t.GetFields(AnyVisibilityInstance), type);
      }

      private static void findTypeElements<TElementType>(IList<TElementType> elementTypes, Func<Type, IEnumerable<TElementType>> allElementsFor, Type type) where TElementType : MemberInfo
      {
         foreach (var element in allElementsFor(type))
         {
            // Ignore inherited fields.
            if (element.DeclaringType == type)
               elementTypes.Add(element);
         }

         var baseType = type.BaseType;
         if (baseType != null)
            findTypeElements(elementTypes, allElementsFor, baseType);
      }

      public static string AssemblyNameFor(Type type)
      {
         var assembluFullName = type.Assembly.FullName;
         if (String.IsNullOrEmpty(assembluFullName)) return assembluFullName;

         var position = assembluFullName.IndexOf(", Version=", StringComparison.Ordinal);
         if (position < 0) return assembluFullName;
         return assembluFullName.Substring(0, position);
      }

      public static IEnumerable<Type> GetConcreteTypesFromAssemblyImplementing<T>(Assembly assembly, bool onlyExportedTypes)
      {
         return GetConcreteTypesFromAssemblyImplementing(assembly, typeof(T), onlyExportedTypes);
      }

      public static IEnumerable<Type> GetConcreteTypesFromAssemblyImplementing(Assembly assembly, Type typeToImplement, bool onlyExportedTypes)
      {
         return from type in GetConcreteTypesFromAssembly(assembly, onlyExportedTypes)
            where type.IsAnImplementationOf(typeToImplement)
            select type;
      }

      public static IEnumerable<Type> GetConcreteTypesFromAssembly(Assembly assembly, bool onlyExportedTypes)
      {
         var typesToScann = onlyExportedTypes ? assembly.GetExportedTypes() : assembly.GetTypes();

         return from type in typesToScann
            where !type.IsGenericType
            where !type.IsAbstractClass()
            select type;
      }
   }
}