using System;
using System.Collections.Specialized;

namespace OSPSuite.Utility.Collections
{
   public interface INotifyCache<TKey, TValue> : ICache<TKey, TValue>, INotifyCollectionChanged
   {
   }

   public class NotifyCache<TKey, TValue> : Cache<TKey, TValue>, INotifyCache<TKey, TValue>
   {
      public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

      public NotifyCache()
      {
      }

      public NotifyCache(Func<TValue, TKey> getKey) : base(getKey)
      {
      }

      public NotifyCache(Func<TKey, TValue> onMissingKey) : base(onMissingKey)
      {
      }

      public NotifyCache(Func<TValue, TKey> getKey, Func<TKey, TValue> onMissingKey) : base(getKey, onMissingKey)
      {
      }

      public override void Add(TKey key, TValue value)
      {
         base.Add(key, value);
         raiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
      }

      public override void Remove(TKey key)
      {
         if (!Contains(key))
            return;

         var value = this[key];
         base.Remove(key);

         raiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
      }

      public override void Clear()
      {
         base.Clear();
         raiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
      }

      private void raiseCollectionChangedEvent(NotifyCollectionChangedEventArgs eventArgs)
      {
         CollectionChanged(this, eventArgs);
      }

      public override TValue this[TKey key]
      {
         set
         {
            if (Contains(key))
            {
               var oldValue = base[key];
               base[key] = value;
               raiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue));
            }
            else
            {
               Add(key, value);
            }
         }
      }
   }
}