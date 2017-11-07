using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility
{
   /// <summary>
   ///    Generic inteface representing a builder repository (i.e. a repository retrieving dynamically the most probable
   ///    implementation for a given type
   /// </summary>
   public interface IBuilderRepository<out TBuilder> : IStartable
   {
      TBuilder BuilderFor(Type type);
      TBuilder BuilderFor(object objectToBuild);
   }

   public abstract class BuilderRepository<TBuilder> : IBuilderRepository<TBuilder> where TBuilder : class, ISpecification<Type>
   {
      private readonly IContainer _container;
      private readonly Type _genericBuilderType;
      private readonly ITypeSimplifier _typeSimplifier;
      private List<TBuilder> _allBuilders;
      private readonly ICache<Type, TBuilder> _allBuilderCache = new Cache<Type, TBuilder>();
      private bool _isInitialized;

      protected BuilderRepository(IContainer container, Type genericBuilderType)
      {
         _container = container;
         _genericBuilderType = genericBuilderType;
         _typeSimplifier = new TypeSimplifier();
      }

      public TBuilder BuilderFor(Type type)
      {
         Start();
         if (_allBuilderCache.Contains(type))
            return _allBuilderCache[type];

         var allBuilderForType = _allBuilders.Where(b => b.IsSatisfiedBy(type)).ToList();

         //No Builder found. Return null
         if (allBuilderForType.Count == 0)
            return null;

         if (allBuilderForType.Count == 1)
            _allBuilderCache.Add(type, allBuilderForType[0]);
         else
         {
            //more than one implementation? try to find the one that matches the best the given type
            var allGenericTypes = allBuilderForType.SelectMany(t => t.GetDeclaredTypesForGeneric(_genericBuilderType)).ToList();

            //one builder implements two ITEXBuilder<> does not make sens
            if (allGenericTypes.Count != allBuilderForType.Count)
               throw new InvalidOperationException($"Cannot resolve the Builder for type '{type}'. It seems that one builder implements more than one generic interface");

            //finds the one that matches the most the implementation
            var simplifiedImplementation = _typeSimplifier.Simplify(allGenericTypes).ToList();
            if (simplifiedImplementation.Count == 1)
               _allBuilderCache.Add(type, allBuilderForType.ElementAt(allGenericTypes.IndexOf(simplifiedImplementation[0])));
            else
               return null;
         }

         return _allBuilderCache[type];
      }

      public TBuilder BuilderFor(object objectToBuild)
      {
         return BuilderFor(objectToBuild.GetType());
      }

      public void Start()
      {
         if (_isInitialized) return;
         _allBuilders = _container.ResolveAll<TBuilder>().ToList();
         _isInitialized = true;
      }
   }
}
