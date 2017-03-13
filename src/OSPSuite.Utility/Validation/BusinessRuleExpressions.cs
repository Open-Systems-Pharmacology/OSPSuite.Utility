using System;
using System.Linq.Expressions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Validation
{
   public static class CreateRule
   {
      public static BusinessRuleExpression<TObject> For<TObject>()
      {
         return new BusinessRuleExpression<TObject>();
      }
   }

   public class BusinessRuleExpression<TObject>
   {
      public BusinessRuleExpression<TObject, TProperty> Property<TProperty>(Expression<Func<TObject, TProperty>> propertyToCheck)
      {
         var propertyBinder = new PropertyBinder<TObject, TProperty>(propertyToCheck.PropertyInfo());
         return new BusinessRuleExpression<TObject, TProperty>(propertyBinder);
      }
   }

   public class BusinessRuleExpression<TObject, TProperty>
   {
      private readonly IPropertyBinder<TObject, TProperty> _propertyBinder;
      private Func<TObject, TProperty, bool> _predicateToMatch;

      public BusinessRuleExpression(PropertyBinder<TObject, TProperty> propertyBinder)
      {
         _propertyBinder = propertyBinder;
      }

      public BusinessRuleExpression<TObject, TProperty> WithRule(Func<TObject, TProperty, bool> predicateToMatch)
      {
         _predicateToMatch = predicateToMatch;
         return this;
      }

      public BusinessRule<TObject, TProperty> WithError(string errorDescription)
      {
         return WithError((o, v) => errorDescription);
      }

      public BusinessRule<TObject, TProperty> WithError(Func<TObject, TProperty, string> errorDescription)
      {
         return new BusinessRule<TObject, TProperty>(_propertyBinder, errorDescription, _predicateToMatch);
      }
   }
}