using System;
using System.Collections;
using System.Collections.Generic;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Collections
{
   public interface ICache<TKey, TValue> : IReadOnlyCollection<TValue>
   {
      /// <summary>
      ///    Adds the value to the cache using the provided getKey method in constructor
      /// </summary>
      /// <param name="value">value to add</param>
      void Add(TValue value);

      /// <summary>
      ///    Adds the value to the cache using the provided key
      /// </summary>
      /// <param name="key">key with which the value will be registered</param>
      /// <param name="value">value to add</param>
      void Add(TKey key, TValue value);

      /// <summary>
      ///    Returns the element by key
      /// </summary>
      /// <param name="key">key of the element to be returned</param>
      /// <returns>the element with the key</returns>
      TValue this[TKey key] { get; set; }

      /// <summary>
      ///    Returns true if an element with the given key exists, otherwise false
      /// </summary>
      bool Contains(TKey key);

      /// <summary>
      ///    Removes an element by key
      /// </summary>
      void Remove(TKey key);

      /// <summary>
      ///    Removes all elements
      /// </summary>
      void Clear();

      /// <summary>
      ///    Allows to iterate over the keys used to cache the items
      /// </summary>
      IEnumerable<TKey> Keys { get; }

      /// <summary>
      ///    Adds a range of <typeparamref name="TValue" /> objects  to the cache using the provided getKey method in constructor
      /// </summary>
      /// <param name="range">range of values to add</param>
      void AddRange(IEnumerable<TValue> range);

      /// <summary>
      ///    Iterates over the cache and returns the Key Value defined for each cache entry.
      /// </summary>
      IEnumerable<KeyValuePair<TKey, TValue>> KeyValues { get; }
   }

   public class Cache<TKey, TValue> : ICache<TKey, TValue>
   {
      private readonly object _locker = new object();

      public Func<TValue, TKey> GetKey { get; set; }
      public Func<TKey, TValue> OnMissingKey { get; set; }

      private readonly IDictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();

      public Cache()
      {
         OnMissingKey = key => { throw new KeyNotFoundException($"Key '{key}' could not be found"); };
      }

      public Cache(Func<TValue, TKey> getKey)
      {
         OnMissingKey = key => { throw new KeyNotFoundException($"Key '{key}' could not be found"); };
         GetKey = getKey;
      }

      public Cache(Func<TKey, TValue> onMissingKey)
      {
         OnMissingKey = onMissingKey;
      }

      public Cache(Func<TValue, TKey> getKey, Func<TKey, TValue> onMissingKey) : this(getKey)
      {
         OnMissingKey = onMissingKey;
      }

      public IEnumerator<TValue> GetEnumerator()
      {
         return _values.Values.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      public virtual void Add(TValue value)
      {
         if (GetKey == null)
            throw new InvalidOperationException("Key delegate was not specified. Please use Add(TKey key, TValue value) instead");

         Add(GetKey(value), value);
      }

      public virtual void AddRange(IEnumerable<TValue> range)
      {
         range.Each(Add);
      }

      public virtual IEnumerable<KeyValuePair<TKey, TValue>> KeyValues
      {
         get { return _values; }
      }

      public virtual void Add(TKey key, TValue value)
      {
         try
         {
            lock (_locker)
            {
               _values.Add(key, value);
            }
         }
         catch (ArgumentNullException)
         {
            throw;
         }
         catch (ArgumentException ex)
         {
            //rethrow message with the value of key as string
            throw new ArgumentException($"An item with the key '{key}' has already been added", ex);
         }
      }

      public virtual TValue this[TKey key]
      {
         get
         {
            // Check first if the value for the requested key 
            // already exists
            if (!Contains(key))
            {
               lock (_locker)
               {
                  if (!Contains(key))
                  {
                     // If the value does not exist, use
                     // the _onMissingKey specified
                     // in the constructor to  fetch the value 
                     return OnMissingKey(key);
                  }
               }
            }

            return _values[key];
         }
         set
         {
            lock (_locker)
            {
               if (Contains(key))
               {
                  _values[key] = value;
               }
               else
               {
                  Add(key, value);
               }
            }
         }
      }

      public virtual bool Contains(TKey key)
      {
         try
         {
            return _values.ContainsKey(key);
         }
         catch (ArgumentNullException)
         {
            return false;
         }
      }

      public virtual void Remove(TKey key)
      {
         lock (_locker)
         {
            if (Contains(key))
            {
               _values.Remove(key);
            }
         }
      }

      public virtual void Clear()
      {
         _values.Clear();
      }

      public virtual IEnumerable<TKey> Keys
      {
         get { return _values.Keys; }
      }

      public int Count
      {
         get { return _values.Count; }
      }
   }
}