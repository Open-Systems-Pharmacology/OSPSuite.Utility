using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Visitor;

namespace OSPSuite.Utility.Extensions
{
   public static class EnumerableExtensions
   {
      public static IEnumerable<T> ThatSatisfy<T>(this IEnumerable<T> itemsToPeekInto, Predicate<T> criteriaToSatisfy)
      {
         return itemsToPeekInto.Where(item => item.Satisfies(criteriaToSatisfy));
      }

      public static bool ContainsItem<T>(this IEnumerable<T> itemsToPeekInto, T itemToFind)
      {
         var items = new List<T>(itemsToPeekInto);
         return items.Contains(itemToFind);
      }

      public static IEnumerable<T> All<T>(this IEnumerable<T> items)
      {
         foreach (var item in items)
         {
            yield return item;
         }
      }

      public static void Each<T>(this IEnumerable<T> list, Action<T> action)
      {
         foreach (var item in list)
         {
            action(item);
         }
      }

      public static void Each<T>(this IReadOnlyList<T> list, Action<T, int> action)
      {
         for (int i = 0; i < list.Count; i++)
         {
            action(list[i], i);
         }
      }

      public static void VisitAllItemsWith<T>(this IEnumerable<T> items, IVisitor<T> visitor)
      {
         items.Each(visitor.Visit);
      }

      public static void DisposeAll<T>(this IEnumerable<T> disposables) where T : IDisposable
      {
         disposables.Each(item => item.Dispose());
      }

      public static IReadOnlyList<TOutput> MapAllUsing<TInput, TOutput>(this IEnumerable<TInput> itemsToMap, IMapper<TInput, TOutput> mapper)
      {
         //no implementation using yield return here since mapper generally creates news objects and we do not want to have
         //to deal with different references in calling code
         return itemsToMap.Select(mapper.MapFrom).ToList();
      }

      public static INotifyList<T> ToRichList<T>(this IEnumerable<T> enumerable)
      {
         return new NotifyList<T>(enumerable);
      }

      public static BindingList<TObject> ToBindingList<TObject>(this IEnumerable<TObject> enumerable)
      {
         return new BindingList<TObject>(enumerable.ToList());
      }

      public static string ToString<T>(this IEnumerable<T> enumerable, string separator)
      {
         return enumerable.ToString(separator, string.Empty);
      }

      public static string ToString<T>(this IEnumerable<T> enumerable, string separator, string encloser)
      {
         var list = enumerable.ToList();
         if (list.Count == 0)
         {
            return string.Empty;
         }
         var sb = new StringBuilder(string.Format("{0}{1}{0}", encloser, list[0]));

         for (int count = 1; count <= list.Count - 1; count++)
         {
            sb.Append(string.Format("{2}{0}{1}{0}", encloser, list[count], separator));
         }

         return sb.ToString();
      }
   }
}