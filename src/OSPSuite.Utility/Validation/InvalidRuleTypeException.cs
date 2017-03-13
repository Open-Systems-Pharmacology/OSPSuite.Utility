using System;

namespace OSPSuite.Utility.Validation
{
   public class InvalidRuleTypeException : Exception
   {
      public const string InvalidRuleMessageFormat = "Rule '{0}' cannot be used with object of type '{1}'";

      public InvalidRuleTypeException(IBusinessRule businessRule, Type objecTypeForWhichTheRuleIsNotValid)
         : base(string.Format(InvalidRuleMessageFormat, businessRule.Name, objecTypeForWhichTheRuleIsNotValid))
      {
      }
   }
}