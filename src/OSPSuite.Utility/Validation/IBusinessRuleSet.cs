using System;
using System.Collections.Generic;

namespace OSPSuite.Utility.Validation
{
   public interface IBusinessRuleSet
   {
      /// <summary>
      ///    Return the number of rules in the set
      /// </summary>
      int Count { get; }

      /// <summary>
      ///    Return one string containing the concatenation of all rules messages
      /// </summary>
      string Message { get; }

      /// <summary>
      ///    Return the list of messages of all rules in that set
      /// </summary>
      IList<string> Messages { get; }

      /// <summary>
      ///    Return true if the set is empty otherwise false
      /// </summary>
      bool IsEmpty { get; }

      /// <summary>
      ///    Return a subset of rule that are not validated by the given object
      /// </summary>
      /// <param name="objectToValidate"> object that need to be validated </param>
      IBusinessRuleSet BrokenBy(object objectToValidate);

      /// <summary>
      ///    Return a subset of rule that are not validated by the given object for a specific property
      /// </summary>
      /// <param name="objectToValidate"> object that need to be validated </param>
      /// <param name="propertyToValidate"> property of
      ///    <para>objectToValidate</para>
      ///    that need to be validated
      /// </param>
      IBusinessRuleSet BrokenBy(object objectToValidate, string propertyToValidate);

      /// <summary>
      ///    Return a subset of rule that are not validated by the given object for a specific property and a specific value
      /// </summary>
      /// <param name="objectToValidate"> object that need to be validated </param>
      /// <param name="propertyToValidate"> property of
      ///    <para>objectToValidate</para>
      ///    that need to be validated
      /// </param>
      /// <param name="valueToValidate"> value used to validate the property </param>
      IBusinessRuleSet BrokenBy(object objectToValidate, string propertyToValidate, object valueToValidate);

      /// <summary>
      ///    Return a subset of the given set containing the rules matching the predicate
      /// </summary>
      IBusinessRuleSet Where(Func<IBusinessRule, bool> predicate);

      /// <summary>
      ///    Return true if the set contains the rule otherwise false
      /// </summary>
      bool Contains(IBusinessRule rule);

      /// <summary>
      ///    Add the given rule to the set
      /// </summary>
      void Add(IBusinessRule ruleToAdd);

      /// <summary>
      ///    Add a list of rules to the business set
      /// </summary>
      void AddRange(IEnumerable<IBusinessRule> rulesToAdd);

      /// <summary>
      ///    Return all business rules in that set
      /// </summary>
      /// <returns> </returns>
      IEnumerable<IBusinessRule> All();

      /// <summary>
      ///    Removes all rules defined in the set
      /// </summary>
      void Clear();
   }
}