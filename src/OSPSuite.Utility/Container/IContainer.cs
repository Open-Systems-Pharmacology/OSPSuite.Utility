using System;
using System.Collections.Generic;

namespace OSPSuite.Utility.Container
{
   public enum LifeStyle
   {
      Singleton,
      Transient
   }

   public interface IContainer : IDisposable
   {
      /// <summary>
      ///    Returns a registered implementation for the interface
      /// </summary>
      /// <returns>a registered implementation for the interface</returns>
      TInterface Resolve<TInterface>();

      /// <summary>
      ///    Returns a registered implementation with the given key for the interface
      /// </summary>
      /// <returns>a registered implementation for the interface</returns>
      TInterface Resolve<TInterface>(string key);

      /// <summary>
      ///    Returns a registered implementation of type <paramref name="type" />
      /// </summary>
      object Resolve(Type type);

      /// <summary>
      ///    Returns a registered implementation of type <paramref name="type" /> that also implements
      ///    <typeparamref name="TInterface" />
      /// </summary>
      TInterface Resolve<TInterface>(Type type) where TInterface : class;

      /// <summary>
      ///    Returns all object implementing the same interface that are registered in the container
      /// </summary>
      /// <typeparam name="TInterface">Type to resolve</typeparam>
      /// <returns>all object implementing the same interface that are registered in the container</returns>
      IEnumerable<TInterface> ResolveAll<TInterface>();

      /// <summary>
      ///    Registers an concreate implementation of an interface.
      /// </summary>
      void RegisterImplementationOf<TInterface>(TInterface component) where TInterface : class;

      /// <summary>
      ///    Registers an instance of an interface with the given key
      /// </summary>
      void RegisterImplementationOf<TInterface>(TInterface component, string key) where TInterface : class;

      /// <summary>
      ///    Registers a concrete class for an interface. A new object will be created for each request
      /// </summary>
      void Register<TService, TConcreteType>();

      /// <summary>
      ///    Registers a concrete class for an interface with key. A new object will be created for each request
      /// </summary>
      void Register<TService, TConcreteType>(string key);

      /// <summary>
      ///    Registers a concrete class for an interface with life style
      /// </summary>
      void Register<TService, TConcreteType>(LifeStyle lifeStyle);

      /// <summary>
      ///    Registers a concrete class for multiple interfaces
      /// </summary>
      void Register<TService1, TService2, TConcreteType>(LifeStyle lifeStyle);

      /// <summary>
      ///    Registers a concrete class for multiple interfaces
      /// </summary>
      void Register<TService1, TService2, TService3, TConcreteType>(LifeStyle lifeStyle);

      /// <summary>
      ///    Registers a concrete class for multiple interfaces
      /// </summary>
      void Register<TService1, TService2, TService3, TService4, TConcreteType>(LifeStyle lifeStyle);

      /// <summary>
      ///    Registers a concrete class for for an interface with life style and key
      /// </summary>
      void Register<TService, TConcreteType>(LifeStyle lifeStyle, string key);

      /// <summary>
      ///    Registers a concrete class for an interface. A new object will be created for each request
      /// </summary>
      void Register(Type serviceType, Type concreteType);

      /// <summary>
      ///    Registers a concrete class for an interface with life style
      /// </summary>
      void Register(Type serviceType, Type concreteType, LifeStyle lifeStyle);

      /// <summary>
      ///    Registers a concrete class forfor multiple interfaces with life style
      /// </summary>
      void Register(IReadOnlyCollection<Type> serviceTypes, Type concreteType, LifeStyle lifeStyle);

      /// <summary>
      ///    Registers a concrete class for an interface with life style and key
      /// </summary>
      void Register(Type serviceType, Type concreteType, LifeStyle lifeStyle, string key);

      /// <summary>
      ///    Registers an abstract factory in the underlying container
      /// </summary>
      /// <typeparam name="TFactory"></typeparam>
      void RegisterFactory<TFactory>() where TFactory : class;

      /// <summary>
      ///    Use this method in using() to encapsulate registration of components in the container.
      ///    A significant gain in registration speed should be observed when registering more than 100 components.
      /// </summary>
      /// <returns>A disposable object managing the optimization.</returns>
      /// <remarks>Do not forget to use using() with this method.</remarks>
      IDisposable OptimizeDependencyResolution();
   }
}