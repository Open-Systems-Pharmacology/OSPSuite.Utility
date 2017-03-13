using System.Collections.Generic;

namespace OSPSuite.Utility.Collections
{
   public interface IStartableRepository<T> : IRepository<T>, IStartable
   {
   }

   public abstract class StartableRepository<T> : IStartableRepository<T>
   {
      private bool _initialized;

      protected StartableRepository()
      {
         _initialized = false;
      }

      public void Start()
      {
         //not thread safe!
         if (_initialized) return;
         DoStart();
         _initialized = true;
         PerformPostStartProcessing();
      }

      /// <summary>
      ///    Action that can only be done once the repository has been intialized
      /// </summary>
      protected virtual void PerformPostStartProcessing()
      {
         /*  Override when required */
      }

      protected abstract void DoStart();
      public abstract IEnumerable<T> All();
   }
}