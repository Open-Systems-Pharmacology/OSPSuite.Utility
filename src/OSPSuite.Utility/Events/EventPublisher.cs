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
      private readonly Thread _capturedUIThread;
      private readonly IExceptionManager _exceptionManager;
      private readonly IList<WeakRef<IListener>> _listeners;

      // Assumes the constructing thread is the one the context dispatches to. Use the explicit-thread
      // overload when the context targets a different thread (e.g. one captured for a separate UI thread).
      public EventPublisher(SynchronizationContext context, IExceptionManager exceptionManager)
         : this(context, Thread.CurrentThread, exceptionManager)
      {
      }

      public EventPublisher(SynchronizationContext context, Thread capturedUIThread, IExceptionManager exceptionManager)
      {
         _listeners = new List<WeakRef<IListener>>();
         _context = context;
         _capturedUIThread = capturedUIThread;
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

         // Decide Send vs Post by thread identity, not by context instance: a thread can have more than
         // one SynchronizationContext instance targeting it. On the context thread, Send dispatches inline
         // (handlers// run before this returns); from any other thread, Post so the publishing thread is not blocked.
         var publishingFromUIThread = ReferenceEquals(Thread.CurrentThread, _capturedUIThread);

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
            if (publishingFromUIThread)
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