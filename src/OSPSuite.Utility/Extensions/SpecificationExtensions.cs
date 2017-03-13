using System;

namespace OSPSuite.Utility.Extensions
{
   public static class SpecificationExtensions
   {
      public static bool Satisfies<T>(this T itemToValidate, Predicate<T> criteriaToSatisfy)
      {
         return criteriaToSatisfy(itemToValidate);
      }
   }
}