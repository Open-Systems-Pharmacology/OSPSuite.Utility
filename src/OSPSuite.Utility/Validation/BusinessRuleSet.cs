using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Validation
{
   public class BusinessRuleSet : IBusinessRuleSet
   {
      private readonly IList<IBusinessRule> _rules;

      public BusinessRuleSet(params IBusinessRule[] rules) : this(new List<IBusinessRule>(rules))
      {
      }

      public BusinessRuleSet(IEnumerable<IBusinessRule> rules) : this(new List<IBusinessRule>(rules))
      {
      }

      public BusinessRuleSet(IList<IBusinessRule> rules)
      {
         _rules = rules;
      }

      public void Add(IBusinessRule ruleToAdd)
      {
         if (Contains(ruleToAdd)) return;
         _rules.Add(ruleToAdd);
      }

      public void AddRange(IEnumerable<IBusinessRule> rulesToAdd)
      {
         rulesToAdd.Each(Add);
      }

      public IEnumerable<IBusinessRule> All()
      {
         return _rules.All();
      }

      public void Clear()
      {
         _rules.Clear();
      }

      public IBusinessRuleSet BrokenBy(object objectToValidate)
      {
         return Where(rule => rule.IsSatisfiedBy(objectToValidate) == false);
      }

      public IBusinessRuleSet BrokenBy(object objectToValidate, string propertyToValidate)
      {
         return Where(rule => rule.IsRuleFor(propertyToValidate)).BrokenBy(objectToValidate);
      }

      public IBusinessRuleSet BrokenBy(object objectToValidate, string propertyToValidate, object valueToValidate)
      {
         return Where(rule => rule.IsRuleFor(propertyToValidate))
            .Where(rule => rule.IsSatisfiedBy(objectToValidate, valueToValidate) == false);
      }

      public IBusinessRuleSet Where(Func<IBusinessRule, bool> predicate)
      {
         var query = from rule in _rules
            where predicate(rule)
            select rule;

         return new BusinessRuleSet(query);
      }

      public string Message => string.Join("\n", Messages.ToArray());

      public IList<string> Messages => new List<string>(from rule in _rules select rule.Description);

      public int Count => _rules.Count;

      public bool Contains(IBusinessRule rule)
      {
         return _rules.Contains(rule);
      }

      public bool IsEmpty => Count == 0;
   }
}