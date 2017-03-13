using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_validatable_extensions : StaticContextSpecification
   {
      protected IValidatable _objectToValidate;
      protected IBusinessRuleSet _rules;

      protected override void Context()
      {
         _objectToValidate = A.Fake<IValidatable>();
         _rules = A.Fake<IBusinessRuleSet>();
         A.CallTo(() => _objectToValidate.Rules).Returns(_rules);
      }
   }

   public class When_validating_an_object : concern_for_validatable_extensions
   {
      [Observation]
      public void should_ask_the_object_to_retrieve_the_list_of_broken_rules()
      {
         A.CallTo(() => _rules.BrokenBy(_objectToValidate)).MustHaveHappened();
      }

      protected override void Because()
      {
         _objectToValidate.Validate();
      }
   }

   public class When_validating_an_object_for_a_specific_property_name : concern_for_validatable_extensions
   {
      private readonly string _propertyName = "toto";

      [Observation]
      public void should_ask_the_object_to_retrieve_the_list_of_broken_rules()
      {
         A.CallTo(() => _rules.BrokenBy(_objectToValidate, _propertyName)).MustHaveHappened();
      }

      protected override void Because()
      {
         _objectToValidate.Validate(_propertyName);
      }
   }

   public class When_validating_an_object_for_a_specific_property_name_and_a_specific_value : concern_for_validatable_extensions
   {
      private readonly string _propertyName = "toto";
      private readonly double _value = 5;

      [Observation]
      public void should_ask_the_object_to_retrieve_the_list_of_broken_rules()
      {
         A.CallTo(() => _rules.BrokenBy(_objectToValidate, _propertyName, _value)).MustHaveHappened();
      }

      protected override void Because()
      {
         _objectToValidate.Validate(_propertyName, _value);
      }
   }

   public abstract class concern_for_validatable_extensions_with_a_real_object : StaticContextSpecification
   {
      protected ObjectWithRules _objectToValidate;

      protected override void Context()
      {
         _objectToValidate = new ObjectWithRules();
      }
   }

   public class When_validating_a_valid_object_for_a_specific_property : concern_for_validatable_extensions_with_a_real_object
   {
      private IBusinessRuleSet _brokenRules;

      [Observation]
      public void should_return_an_empty_set_of_broken_rules()
      {
         _brokenRules.Count.ShouldBeEqualTo(0);
      }

      protected override void Because()
      {
         _brokenRules = _objectToValidate.Validate(item => item.Description);
      }
   }

   public class When_validating_an_object_for_a_specific_property_and_a_specific_valid_value : concern_for_validatable_extensions_with_a_real_object
   {
      private IBusinessRuleSet _brokenRules;

      [Observation]
      public void should_return_an_empty_set_of_broken_rules()
      {
         _brokenRules.Count.ShouldBeEqualTo(0);
      }

      protected override void Because()
      {
         _brokenRules = _objectToValidate.Validate(item => item.Value, 3);
      }
   }

   public class When_validating_an_object_for_a_specific_property_and_a_specific_invalid_value : concern_for_validatable_extensions_with_a_real_object
   {
      private IBusinessRuleSet _brokenRules;

      [Observation]
      public void should_return_an_non_empty_set_of_broken_rules()
      {
         _brokenRules.Count.ShouldNotBeEqualTo(0);
      }

      protected override void Because()
      {
         _brokenRules = _objectToValidate.Validate(item => item.Value, -5);
      }
   }

   public class When_validating_an_invalid_object_for_a_specific_property : concern_for_validatable_extensions_with_a_real_object
   {
      private IBusinessRuleSet _brokenRules;

      [Observation]
      public void should_return_an_non_empty_set_of_broken_rules()
      {
         _brokenRules.Count.ShouldNotBeEqualTo(0);
      }

      protected override void Context()
      {
         base.Context();
         _objectToValidate.Value = -15;
      }

      protected override void Because()
      {
         _brokenRules = _objectToValidate.Validate(item => item.Value);
      }
   }

   public class When_asking_if_a_valid_object_is_valid : concern_for_validatable_extensions_with_a_real_object
   {
      protected override void Context()
      {
         base.Context();
         _objectToValidate.Name = "toto";
      }

      [Observation]
      public void should_return_true()
      {
         _objectToValidate.IsValid().ShouldBeTrue();
      }
   }

   public class When_asking_if_a_invalid_object_is_valid : concern_for_validatable_extensions_with_a_real_object
   {
      protected override void Because()
      {
         base.Because();
         _objectToValidate.Value = -15;
      }

      [Observation]
      public void should_return_false()
      {
         _objectToValidate.IsValid().ShouldBeFalse();
      }
   }
}