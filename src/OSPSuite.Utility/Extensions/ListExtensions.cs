using System.Collections.Generic;

namespace OSPSuite.Utility.Extensions
{
   public static class ListExtensions
   {
      /// <summary>
      ///    Returns true if all elements in the first list are equal to the elements in the second list (and in the same order)
      ///    otherwise false
      /// </summary>
      public static bool ListEquals<T>(this IList<T> list1, IList<T> list2)
      {
         if (list1.Count != list2.Count)
            return false;

         for (int i = 0; i < list1.Count; i++)
         {
            if (!Equals(list1[i], list2[i]))
               return false;
         }

         return true;
      }
   }
}