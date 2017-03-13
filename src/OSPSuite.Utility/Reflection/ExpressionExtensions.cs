using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OSPSuite.Utility.Reflection
{
   public static class ExpressionExtensions
   {
      public static PropertyInfo PropertyInfo<TParameter, TValue>(this Expression<Func<TParameter, TValue>> expression)
      {
         return new ExpressionInspector<TParameter>().PropertyFor(expression);
      }

      public static string Name<T>(this Expression<T> expression)
      {
         return new ExpressionInspector<T>().Name(expression);
      }
   }
}