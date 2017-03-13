using System;
using System.Collections.Generic;
using System.Linq;

namespace OSPSuite.Utility
{
   /// <summary>
   ///    Encapsulates some helpful functions for enumerable type
   /// </summary>
   public static class EnumHelper
   {
      /// <summary>
      ///    Returns an enumeration containing all values defined in the enumeration
      /// </summary>
      /// <typeparam name="TEnum">Type of the enumeration for which the values should be returned</typeparam>
      public static IEnumerable<TEnum> AllValuesFor<TEnum>()
      {
         return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
      }

      /// <summary>
      ///    Returns the enum value corresponding to the string given as parameter (not case sensivite)
      /// </summary>
      /// <typeparam name="TEnum">Type of the enumeration for which the value should be returned</typeparam>
      public static TEnum ParseValue<TEnum>(string enumStringToParser)
      {
         return (TEnum) Enum.Parse(typeof(TEnum), enumStringToParser, true);
      }
   }
}