using System;
using System.Linq.Expressions;
using System.Reflection;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Reflection
{
   public interface IExpressionInspector<TObject>
   {
      PropertyInfo PropertyFor<TProperty>(Expression<Func<TObject, TProperty>> expressionToInspect);
      MethodInfo MethodFor<T, U>(Expression<Func<T, U>> expression);
      MethodInfo MethodFor<T, U, V>(Expression<Func<T, U, V>> expression);
      MethodInfo MethodFor<T>(Expression<Action<T>> expression);
      MemberExpression GetMemberExpression<TProperty>(Expression<Func<TObject, TProperty>> expression);
      string Name(Expression<TObject> expression);
   }

   public class ExpressionInspector<TObject> : IExpressionInspector<TObject>
   {
      public PropertyInfo PropertyFor<TProperty>(Expression<Func<TObject, TProperty>> expressionToInspect)
      {
         return ReflectionHelper.PropertyFor(expressionToInspect);
      }

      public MethodInfo MethodFor<T, U>(Expression<Func<T, U>> expression)
      {
         return ReflectionHelper.MethodFor(expression);
      }

      public MethodInfo MethodFor<T, U, V>(Expression<Func<T, U, V>> expression)
      {
         return ReflectionHelper.MethodFor(expression);
      }

      public MethodInfo MethodFor<T>(Expression<Action<T>> expression)
      {
         return ReflectionHelper.MethodFor(expression);
      }

      public MemberExpression GetMemberExpression<TProperty>(Expression<Func<TObject, TProperty>> expression)
      {
         return ReflectionHelper.GetMemberExpression(expression);
      }

      public string Name(Expression<TObject> expression)
      {
         if (expression.Body.NodeType == ExpressionType.MemberAccess)
            return expression.Body.DowncastTo<MemberExpression>().Member.Name;
         if (expression.Body.NodeType == ExpressionType.Parameter)
            return expression.Body.DowncastTo<ParameterExpression>().Name;
         if (expression.Body.NodeType == ExpressionType.Call)
            return expression.Body.DowncastTo<MethodCallExpression>().Method.Name;

         throw new ArgumentException($"Cannot find name for {expression.Body.NodeType}");
      }
   }
}