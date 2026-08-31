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

      //both fields below are only ever written and read while holding _locker, so unlike _initialized (which is
      //also read on the lock-free fast path) they do not need to be volatile. 0 is a safe sentinel for
      //_initializingThreadId because managed thread ids are always positive.
      private int _initializingThreadId;
      private bool _doStartCompleted;

      /// <summary>
      ///    Starts the repository once. The first caller runs <see cref="DoStart" /> followed by
      ///    <see cref="PerformPostStartProcessing" />; concurrent callers block until both have completed and therefore never
      ///    observe a partially started repository. Subsequent calls return without locking. The guarantee is cross-thread
      ///    only: a reentrant call on the initializing thread out of <see cref="PerformPostStartProcessing" /> returns
      ///    immediately and sees the complete <see cref="DoStart" /> content, but not the state the hook is still building.
      ///    <para>
      ///       A failure in either <see cref="DoStart" /> or <see cref="PerformPostStartProcessing" /> leaves the repository
      ///       cold: the exception propagates to the caller and the next call to <see cref="Start" /> runs both again.
      ///       Implementations that can fail midway should therefore reset partial state on entry of <see cref="DoStart" />
      ///       so that a retry does not duplicate entries.
      ///    </para>
      /// </summary>
      public void Start()
      {
         if (_initialized) return;
         lock (_locker)
         {
            if (_initialized) return;

            if (_initializingThreadId == Environment.CurrentManagedThreadId)
            {
               //reentrant call on the initializing thread. Out of PerformPostStartProcessing (e.g. via All) this is
               //supported and must not run DoStart again: the DoStart content is already complete
               if (_doStartCompleted) return;

               //out of DoStart itself the repository is not filled yet, so returning would silently expose empty
               //content (and running DoStart again would recurse forever, as the original implementation did).
               //The reentrant path may run through other repositories, so it points at the stack trace
               throw new InvalidOperationException($"'{GetType().Name}' was re-entered while its '{nameof(DoStart)}' was still filling it. See the stack trace for the reentrant path.");
            }

            _initializingThreadId = Environment.CurrentManagedThreadId;
            try
            {
               DoStart();
               _doStartCompleted = true;
               PerformPostStartProcessing();

               //published last and only on full success: a failure above leaves the repository cold and the next Start retries
               _initialized = true;
            }
            finally
            {
               _initializingThreadId = 0;
               _doStartCompleted = false;
            }
         }
      }

      /// <summary>
      ///    Action that can only be done once the repository has been initialized.
      ///    <para>
      ///       Implementations typically build the lookup caches that the repository's own accessors read, so it runs while
      ///       <see cref="Start" /> still holds the start lock: the repository is only published as started once this method
      ///       returned. Reentrant use of the repository from this method is supported and sees the complete
      ///       <see cref="DoStart" /> content — but not the caches this method is itself still building, so it must not rely
      ///       on accessors backed by them. It must also not block on another thread that accesses the same repository.
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
