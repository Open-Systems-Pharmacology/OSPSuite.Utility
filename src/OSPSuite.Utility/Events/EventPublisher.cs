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
         //Before publishing, prunes the list of listener and remove dead references
         pruneReferences();

         // Determine if a Listener handles the message of type T
         // by trying to cast it
         foreach (var listener in _listeners.ToList())
         {
            var receiver = listener.Target as IListener<T>;
            if (receiver == null) continue;

            // We are using SyncronizationContext to handle moving processing
            // from a background thread to the UI thread without having 
            // to worry about it in the View or Presenter
            _context.Send(state => _exceptionManager.Execute(() => receiver.Handle(eventToPublish)), null);
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

      private void pruneReferences()
      {
         doWithinLock(() =>
         {
            for (var i = _listeners.Count - 1; i >= 0; i--)
            {
               if (_listeners[i].Target == null)
               {
                  _listeners.RemoveAt(i);
               }
            }
         });
      }
   }
}