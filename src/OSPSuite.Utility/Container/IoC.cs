using System;

namespace OSPSuite.Utility.Container
{
   public static class IoC
   {
      public static IContainer Container { get; private set; }

      public static void InitializeWith(IContainer container)
      {
         Container = container;
      }

      public static TImplementationOfInterface Resolve<TImplementationOfInterface>()
      {
         try
         {
            return Container.Resolve<TImplementationOfInterface>();
         }
         catch (Exception e)
         {
            throw new InterfaceResolutionException(typeof(TImplementationOfInterface), e);
         }
      }

      public static TImplementationOfInterface Resolve<TImplementationOfInterface>(string key)
      {
         try
         {
            return Container.Resolve<TImplementationOfInterface>(key);
         }
         catch (Exception e)
         {
            throw new InterfaceResolutionException(typeof(TImplementationOfInterface), key, e);
         }
      }

      public static void RegisterImplementationOf<T>(T component) where T : class
      {
         Container.RegisterImplementationOf(component);
      }

      public static void RegisterImplementationOf<T>(T component, string key) where T : class
      {
         Container.RegisterImplementationOf(component, key);
      }
   }
}