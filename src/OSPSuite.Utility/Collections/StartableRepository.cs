using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace OSPSuite.Utility.Collections
{
   public interface IStartableRepository<T> : IRepository<T>, IStartable
   {
   }

   public abstract class StartableRepository<T> : IStartableRepository<T>
   {
      private readonly object _locker = new object();
      private volatile bool _initialized;
      private ExceptionDispatchInfo _startFailure;

      protected StartableRepository()
      {
         _initialized = false;
      }

      public void Start()
      {
         if (_initialized) return;
         lock (_locker)
         {
            if (_initialized) return;

            //DoStart may have partially filled the repository before throwing. Running it again would append
            //to that partial state and duplicate entries, so the first failure is final and is rethrown.
            _startFailure?.Throw();

            try
            {
               DoStart();
            }
            catch (Exception e)
            {
               _startFailure = ExceptionDispatchInfo.Capture(e);
               throw;
            }

            //set before the post processing so that a reentrant Start (e.g. PerformPostStartProcessing calling All) does not run DoStart again
            _initialized = true;
            PerformPostStartProcessing();
         }
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
