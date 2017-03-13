using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Visitor
{
   public static class VisitorInvoker
   {
      private static readonly ITypeSimplifier _typeSimplifier = new TypeSimplifier();
      private static readonly string _visitMethodName;
      private static readonly ICache<string, MethodInfo> _visitMethodCache = new Cache<string, MethodInfo> {OnMissingKey = x => null};
      private static readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();

      static VisitorInvoker()
      {
         //cache visit name
         Expression<Action<IVisitor<object>>> expression = vis => vis.Visit(new object());
         _visitMethodName = expression.Name();
      }

      /// <summary>
      ///    Try to resolve an implementation of the function Visit of the visitor depending on the type of the objectToVisit
      /// </summary>
      /// <param name="visitor"> visitor that need to be visited if possible </param>
      /// <param name="objectToVisit"> object that the visitor should visit </param>
      public static void InvokeVisit<T>(IVisitor visitor, T objectToVisit)
      {
         if (visitor == null) return;
         if (objectToVisit == null) return;

         var methodInfo = cachedMethodInfoFor(visitor, objectToVisit);
         if (methodInfo != null)
            invokeVisitMethod(visitor, objectToVisit, methodInfo);
         else
         {
            var visitorType = getVisitorType(visitor, objectToVisit);
            invokeVisitForType(visitorType, visitor, objectToVisit);
         }
      }

      private static MethodInfo cachedMethodInfoFor(IVisitor visitor, object objectToVisit)
      {
         var key = getKeyFor(visitor, objectToVisit);
         _cacheLock.EnterReadLock();
         try
         {
            return _visitMethodCache[key];
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      private static string getKeyFor(IVisitor visitor, object objectToVisit)
      {
         return $"{visitor.GetType()}-{objectToVisit.GetType()}";
      }

      private static Type getVisitorType<T>(IVisitor visitor, T objectToVisit)
      {
         //no direct visit available so far.
         var typeOfObjectToVisit = objectToVisit.GetType();

         //return list of all type in IVisitor<Type> from the visitor
         var allVisitableTypes = visitor.GetDeclaredTypesForGeneric(typeof(IVisitor<>));

         //return all possible implementations of our visitor that match the objecToVisit type
         var possibleImplementations = from genericType in allVisitableTypes
            where genericType.DeclaredType.IsAssignableFrom(typeOfObjectToVisit)
            select genericType;

         //return the visitor implementations that best fit the object to visit, if one exists
         return resolveVisitorImplementationType(possibleImplementations.ToList(), visitor, typeOfObjectToVisit);
      }

      private static Type resolveVisitorImplementationType(IList<TypeForGenericType> allPossibleImplementations, IVisitor visitor, Type typeOfObjectToVisit)
      {
         //no matching signatures, return 
         if (allPossibleImplementations.Count == 0)
            return null;

         //one macthing signature that the one
         if (allPossibleImplementations.Count == 1)
            return allPossibleImplementations[0].GenericType;

         //can we find the implementation with our in house conventions?
         string interfaceName = "I" + typeOfObjectToVisit.Name;
         var typeImplementation = allPossibleImplementations.SingleOrDefault(key => key.DeclaredType.Name.Equals(interfaceName));
         if (typeImplementation != null)
            return typeImplementation.GenericType;

         //at least two implementations. We now try to simplify the list of implementation to one (only one hierarchy branch)
         //if the results provide more than one type, we throw an exception

         var simplifiedImplementation = _typeSimplifier.Simplify(allPossibleImplementations).ToList();
         if (simplifiedImplementation.Count == 1)
            return simplifiedImplementation[0].GenericType;

         //at least two implementations. Visit call is ambiguous and cannot be resolved at run time.
         throw new AmbiguousVisitMethodException(simplifiedImplementation, visitor.GetType(), typeOfObjectToVisit);
      }

      private static void invokeVisitForType<T>(Type visitorType, IVisitor visitor, T objectToVisit)
      {
         //no visitor found for given type. 
         if (visitorType == null)
         {
            if (visitor.IsAnImplementationOf<IStrictVisitor>())
               throw new UnableToVisitObjectException(visitor, objectToVisit.GetType());

            //Nothing to do
            return;
         }

         var key = getKeyFor(visitor, objectToVisit);
         var method = visitorType.GetMethod(_visitMethodName);

         _cacheLock.EnterWriteLock();
         try
         {
            if (!_visitMethodCache.Contains(key))
               _visitMethodCache.Add(key, method);
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }

         invokeVisitMethod(visitor, objectToVisit, method);
      }

      private static void invokeVisitMethod<T>(IVisitor visitor, T objectToVisit, MethodInfo method)
      {
         method.Invoke(visitor, new object[] {objectToVisit});
      }
   }
}