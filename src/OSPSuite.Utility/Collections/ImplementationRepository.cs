using System.Collections.Generic;
using OSPSuite.Utility.Container;

namespace OSPSuite.Utility.Collections
{
   public class ImplementationRepository<T> : IRepository<T>
   {
      private readonly IContainer _container;

      public ImplementationRepository(IContainer container)
      {
         _container = container;
      }

      public IEnumerable<T> All()
      {
         return _container.ResolveAll<T>();
      }
   }
}