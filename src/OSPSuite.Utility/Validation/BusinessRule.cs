using System;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Validation
{
   public interface IBusinessRule
   {
      /// <summary>
      ///    Name of the property for which the business rule was defined
      /// </summary>
      string Name { get; }

      /// <summary>
      ///    Return true if this rule is a business rule for the given property, otherwise false
      /// </summary>
      bool IsRuleFor(string propertyName);

      /// <summary>
      ///    Return business rule description (e.g. error message or notification)
      /// </summary>
      string Description { get; }

      /// <summary>
      ///    Return true if the business rule is satisfied for the given item otherwise false
      /// </summary>
      /// <param name="item">
      ///    object for which the rule should be verified.
      ///    in that case the property value itstelf will be used to validate the object.
      /// </param>
      bool IsSatisfiedBy(object item);

      /// <summary>
      ///    Return true if the business rule is satisfied for the given item and for the given value otherwise false
      /// </summary>
      /// <param name="item">object for which the rule should be verified</param>
      /// <param name="value">value that should be used to validate the object</param>
      /// <returns></returns>
      bool IsSatisfiedBy(object item, object value);
   }

   public class BusinessRule<TObject, TProperty> : IBusinessRule
   {
      private readonly IPropertyBinder<TObject, TProperty> _propertyBinder;
      private readonly Func<TObject, TProperty, string> _description;
      private readonly Func<TObject, TProperty, bool> _matchPredicate;
      public string Description { get; private set; }

      public BusinessRule(IPropertyBinder<TObject, TProperty> propertyBinder, Func<TObject, TProperty, string> description, Func<TObject, TProperty, bool> matchPredicate)
      {
         _propertyBinder = propertyBinder;
         _description = description;
         _matchPredicate = matchPredicate;
      }

      public bool IsRuleFor(string propertyName)
      {
         return Name.Equals(propertyName);
      }

      public bool IsSatisfiedBy(object item)
      {
         var convertedItem = convertObjectToValidate(item);
         return IsSatisfiedBy(convertedItem, _propertyBinder.GetValue(convertedItem));
      }

      public bool IsSatisfiedBy(object item, object value)
      {
         return IsSatisfiedBy(convertObjectToValidate(item), value.ConvertedTo<TProperty>());
      }

      private bool IsSatisfiedBy(TObject item, TProperty value)
      {
         Description = _description(item, value);
         return _matchPredicate(item, value);
      }

      private TObject convertObjectToValidate(object item)
      {
         try
         {
            return item.ConvertedTo<TObject>();
         }
         catch (Exception)
         {
            throw new InvalidRuleTypeException(this, item.GetType());
         }
      }

      public string Name
      {
         get { return _propertyBinder.PropertyName; }
      }
   }
}