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
      private int _initializingThreadId;
      private ExceptionDispatchInfo _startFailure;

      protected StartableRepository()
      {
         _initialized = false;
      }

      /// <summary>
      ///    Starts the repository once. The first caller runs <see cref="DoStart" /> followed by
      ///    <see cref="PerformPostStartProcessing" />; concurrent callers block until both have completed and therefore never
      ///    observe a partially started repository. Subsequent calls return without locking.
      /// </summary>
      public void Start()
      {
         if (_initialized) return;
         lock (_locker)
         {
            if (_initialized) return;

            //a reentrant call on the initializing thread (e.g. PerformPostStartProcessing using All) must not run DoStart again.
            //Only that thread is let through: every other caller waits on the lock until the repository is fully started.
            if (_initializingThreadId == Environment.CurrentManagedThreadId) return;

            //DoStart may have partially filled the repository before throwing. Running it again would append
            //to that partial state and duplicate entries, so the first failure is final and is rethrown.
            _startFailure?.Throw();

            _initializingThreadId = Environment.CurrentManagedThreadId;
            try
            {
               DoStart();
               PerformPostStartProcessing();
            }
            catch (Exception e)
            {
               _startFailure = ExceptionDispatchInfo.Capture(e);
               throw;
            }
            finally
            {
               _initializingThreadId = 0;
            }

            _initialized = true;
         }
      }

      /// <summary>
      ///    Action that can only be done once the repository has been intialized.
      ///    <para>
      ///       Implementations typically build the lookup caches that the repository's own accessors read, so it runs while
      ///       <see cref="Start" /> still holds the start lock: the repository is only published as started once this method
      ///       returned. Reentrant use of the repository from this method is supported, but it must not block on another
      ///       thread that accesses the same repository.
      ///    </para>
      /// </summary>
      protected virtual void PerformPostStartProcessing()
      {
         /*  Override when required */
      }

      protected abstract void DoStart();
      public abstract IEnumerable<T> All();
   }
}
