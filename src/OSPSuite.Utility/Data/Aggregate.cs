using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Data
{
   public abstract class Aggregate
   {
      /// <summary>
      ///    Returns the results of the aggregation function performed on the enumeration <paramref name="objects" />
      /// </summary>
      public abstract object PerformAggregation(IEnumerable<object> objects);

      public abstract Type DataType { get; }
      public string Name { get; set; }
   }

   public abstract class Aggregate<TOutput> : Aggregate
   {
      public override Type DataType
      {
         get { return typeof(TOutput); }
      }
   }

   public class Aggregate<TInput, TOutput> : Aggregate<TOutput>
   {
      /// <summary>
      ///    The typed function performing the aggregation
      /// </summary>
      public Func<IEnumerable<TInput>, TOutput> Aggregation { get; set; }

      public override object PerformAggregation(IEnumerable<object> objects)
      {
         // the aggregation might get an empty object
         if (objects == null)
            return DBNull.Value;

         // the objects might have null values so filter before cast
         var values = objects.Where(valueIsDefined)
            .Select(x => x.DowncastTo<TInput>()).ToList();

         if (values.Any())
            return Aggregation(values);

         return DBNull.Value;
      }

      private static bool valueIsDefined(object x)
      {
         return x != null && x != DBNull.Value;
      }
   }
}