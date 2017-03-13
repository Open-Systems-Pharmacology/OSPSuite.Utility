using System.Collections.Generic;
using OSPSuite.Utility.Collections;

namespace OSPSuite.Utility.Extensions
{
   public static class NotifyCacheExtensions
   {
      public static INotifyCache<TKey, TValue> ToNotifyCache<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
      {
         var notifyCache = new NotifyCache<TKey, TValue>();
         foreach (var keyValue in dictionary)
         {
            notifyCache.Add(keyValue.Key, keyValue.Value);
         }
         return notifyCache;
      }
   }
}