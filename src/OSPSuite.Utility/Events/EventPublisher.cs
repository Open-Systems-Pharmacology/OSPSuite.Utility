using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Events
{
   public class EventPublisher : IEventPublisher
   {
      private readonly SynchronizationContext _context;
      private readonly IExceptionManager _exceptionManager;
      private readonly IList<WeakRef<IListener>> _listeners;

      public EventPublisher(SynchronizationContext context, IExceptionManager exceptionManager)
      {
         _listeners = new List<WeakRef<IListener>>();
         _context = context;
         _exceptionManager = exceptionManager;
      }

      public void PublishEvent<T>(T eventToPublish)
      {
         List<WeakRef<IListener>> listeners;
         lock (_listeners)
         {
            //Before publishing, prunes the list of listener and remove dead references
            pruneReferences();

            // Snapshot the listeners while holding the lock so the dispatch loop below
            // cannot race with a concurrent Add/Remove/prune mutating the same list
            listeners = _listeners.ToList();
         }

         // When publishing from the thread that owns the context (typically the UI thread),
         // dispatch synchronously with Send so handlers run inline and any state they touch is
         // updated before PublishEvent returns. When publishing from another thread, use the
         // non-blocking Post so the background thread is not blocked waiting on the UI thread.
         var publishingFromContextThread = SynchronizationContext.Current == _context;

         // Determine if a Listener handles the message of type T by trying to cast it.
         // Dispatch happens outside the lock so handlers never run while the lock is held.
         foreach (var listener in listeners)
         {
            var receiver = listener.Target as IListener<T>;
            if (receiver == null) continue;

            // We are using SynchronizationContext to handle moving processing
            // from a background thread to the UI thread without having
            // to worry about it in the View or Presenter.
            SendOrPostCallback dispatch = state => _exceptionManager.Execute(() => receiver.Handle(eventToPublish));
            if (publishingFromContextThread)
               _context.Send(dispatch, null);
            else
               _context.Post(dispatch, null);
         }
      }

      public void AddListener(IListener listenerToAdd)
      {
         doWithinLock(() =>
         {
            if (containsListener(listenerToAdd)) return;
            _listeners.Add(new WeakRef<IListener>(listenerToAdd));
         });
      }

      public void RemoveListener(IListener listenerToRemove)
      {
         doWithinLock(() =>
         {
            var listenerRef = _listeners.FirstOrDefault(item => item.Target == listenerToRemove);
            if (listenerRef != null)
            {
               _listeners.Remove(listenerRef);
            }
         });
      }

      public bool Contains(IListener listener)
      {
         lock (_listeners)
         {
            return containsListener(listener);
         }
      }

      private bool containsListener(IListener listener)
      {
         var listenerRef = _listeners.FirstOrDefault(item => item.Target == listener);
         return listenerRef != null;
      }

      private void doWithinLock(Action action)
      {
         lock (_listeners)
         {
            action();
         }
      }

      // Caller must hold the lock on _listeners (PublishEvent does).
      private void pruneReferences()
      {
         for (var i = _listeners.Count - 1; i >= 0; i--)
         {
            if (_listeners[i].Target == null)
            {
               _listeners.RemoveAt(i);
            }
         }
      }
   }
}