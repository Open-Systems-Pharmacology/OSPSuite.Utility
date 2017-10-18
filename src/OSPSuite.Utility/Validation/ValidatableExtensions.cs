using System;
using System.Linq.Expressions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Validation
{
   public static class ValidatableExtensions
   {
      public static IBusinessRuleSet Validate(this IValidatable itemToValidate)
      {
         return itemToValidate.Rules.BrokenBy(itemToValidate);
      }

      public static IBusinessRuleSet Validate(this IValidatable itemToValidate, string propertyToValidate)
      {
         return itemToValidate.Rules.BrokenBy(itemToValidate, propertyToValidate);
      }

      public static IBusinessRuleSet Validate(this IValidatable itemToValidate, string propertyToValidate, object valueToValidate)
      {
         return itemToValidate.Rules.BrokenBy(itemToValidate, propertyToValidate, valueToValidate);
      }

      public static IBusinessRuleSet Validate<TObjectToValidate, TProperty>(this TObjectToValidate itemToValidate, Expression<Func<TObjectToValidate, TProperty>> propertyToValidate)
         where TObjectToValidate : IValidatable
      {
         return itemToValidate.Validate(propertyToValidate.Name());
      }

      public static IBusinessRuleSet Validate<TObjectToValidate, TProperty>(this TObjectToValidate itemToValidate, Expression<Func<TObjectToValidate, TProperty>> propertyToValidate,
         TProperty valueToValidate) where TObjectToValidate : IValidatable
      {
         return itemToValidate.Validate(propertyToValidate.Name(), valueToValidate);
      }

      public static bool IsValid(this IValidatable itemToValidate)
      {
         return itemToValidate.Validate().IsEmpty;
      }
   }
}