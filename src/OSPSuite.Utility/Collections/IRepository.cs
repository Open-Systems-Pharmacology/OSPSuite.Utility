using System.Collections.Generic;

namespace OSPSuite.Utility.Collections
{
   public interface IRepository<out T>
   {
      IEnumerable<T> All();
   }
}