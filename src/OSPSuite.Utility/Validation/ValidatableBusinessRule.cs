using System;
using System.Collections.Generic;
using System.Linq;

namespace OSPSuite.Utility.Validation
{
   public class ValidatableBusinessRule : IBusinessRule
   {
      private readonly IValidatable _objectToValidate;
      private readonly IEnumerable<IBusinessRule> _underlyingRules;
      private string _propertyNameToValidate;

      public string Name { get; private set; }
      public string Description { get; private set; }

      public ValidatableBusinessRule(IValidatable objectToValidate)
      {
         _objectToValidate = objectToValidate;
         _underlyingRules = objectToValidate.Rules.All();
         Name = "ValidatableBusinessRule";
      }

      public bool IsRuleFor(string propertyName)
      {
         var isRuleForProperty = _underlyingRules.Any(rule => rule.IsRuleFor(propertyName));
         _propertyNameToValidate = isRuleForProperty ? propertyName : null;
         return isRuleForProperty;
      }

      public bool IsSatisfiedBy(object item)
      {
         return anySubRulesBrokenForCriteria(rule => rule.IsSatisfiedBy(_objectToValidate) == false);
      }

      public bool IsSatisfiedBy(object item, object value)
      {
         return anySubRulesBrokenForCriteria(rule => rule.IsSatisfiedBy(_objectToValidate, value) == false);
      }

      private bool anySubRulesBrokenForCriteria(Func<IBusinessRule, bool> specification)
      {
         var rulesToValidates = _propertyNameToValidate == null
            ? _underlyingRules
            : _underlyingRules.Where(rule => rule.IsRuleFor(_propertyNameToValidate));

         var brokenRules = new BusinessRuleSet(rulesToValidates.Where(specification));
         Description = brokenRules.Message;
         _propertyNameToValidate = null;
         return brokenRules.IsEmpty;
      }
   }
}