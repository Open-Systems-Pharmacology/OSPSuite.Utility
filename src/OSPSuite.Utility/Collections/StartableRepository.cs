using System;
using System.Collections.Generic;
using System.Threading;

namespace OSPSuite.Utility.Collections
{
   public interface IStartableRepository<T> : IRepository<T>, IStartable
   {
   }

   public abstract class StartableRepository<T> : IStartableRepository<T>
   {
      private readonly Lock _locker = new Lock();
      private volatile bool _initialized;

      //only ever written and read while holding _locker, so unlike _initialized (which is also read on the
      //lock-free fast path) this field does not need to be volatile
      private int _initializingThreadId;

      protected StartableRepository()
      {
         _initialized = false;
      }

      /// <summary>
      ///    Starts the repository once. The first caller runs <see cref="DoStart" /> followed by
      ///    <see cref="PerformPostStartProcessing" />; concurrent callers block until both have completed and therefore never
      ///    observe a partially started repository. Subsequent calls return without locking.
      ///    <para>
      ///       A <see cref="DoStart" /> failure leaves the repository cold: the exception propagates to the caller and the
      ///       next call to <see cref="Start" /> runs <see cref="DoStart" /> again. Implementations whose
      ///       <see cref="DoStart" /> can fail midway should therefore reset partial state on entry so that a retry does not
      ///       duplicate entries.
      ///    </para>
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

            var doStartSucceeded = false;
            _initializingThreadId = Environment.CurrentManagedThreadId;
            try
            {
               DoStart();
               doStartSucceeded = true;
               PerformPostStartProcessing();
            }
            finally
            {
               _initializingThreadId = 0;

               //once DoStart succeeded the repository counts as started even when the post start processing throws
               //(matching the historical behavior where the flag was set between the two), because re-running DoStart
               //over the already filled content would duplicate entries
               _initialized = doStartSucceeded;
            }
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
