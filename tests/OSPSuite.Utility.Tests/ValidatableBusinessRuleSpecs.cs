using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_ValidatableBusinessRule : ContextSpecification<IBusinessRule>
   {
      protected ObjectWithRules _objectToValidate;

      protected override void Context()
      {
         _objectToValidate = new ObjectWithRules();
         sut = new ValidatableBusinessRule(_objectToValidate);
      }
   }

   public class When_checking_a_rule_is_a_rule_for_a_given_property : concern_for_ValidatableBusinessRule
   {
      [Observation]
      public void should_return_true_if_one_of_the_underlying_rules_has_been_defined_for_that_property_otherwise_false()
      {
         sut.IsRuleFor("Name").ShouldBeTrue();
         sut.IsRuleFor("Description").ShouldBeFalse();
      }
   }

   public class When_validating_a_value_for_a_wrapper_object : concern_for_ValidatableBusinessRule
   {
      private ObjectWithRulesDTO _objectWithRulesDTO;

      protected override void Context()
      {
         base.Context();
         _objectWithRulesDTO = new ObjectWithRulesDTO(_objectToValidate);
      }

      [Observation]
      public void should_delegate_to_the_underlying_rules_and_return_the_corresponding_validation()
      {
         _objectWithRulesDTO.Validate(item => item.Value, 10).IsEmpty.ShouldBeTrue();
         _objectWithRulesDTO.Validate(item => item.Value, -2).IsEmpty.ShouldBeFalse();
         _objectWithRulesDTO.Validate(item => item.Name, string.Empty).IsEmpty.ShouldBeFalse();
      }

      [Observation]
      public void should_retrieve_the_accurate_error_message()
      {
         _objectWithRulesDTO.Validate(item => item.Value, -2).Message.ShouldBeEqualTo(ObjectWithRules.AllRules.GreaterThanZero.Description);
      }
   }
}