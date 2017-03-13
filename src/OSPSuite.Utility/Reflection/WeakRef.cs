using System;

namespace OSPSuite.Utility.Reflection
{
   public class WeakRef<T> where T : class
   {
      private readonly WeakReference<T> _wr;

      public WeakRef(T objectToReference)
      {
         _wr = new WeakReference<T>(objectToReference);
      }

      public T Target
      {
         get
         {
            T target;
            _wr.TryGetTarget(out target);
            return target;
         }
      }
   }
}