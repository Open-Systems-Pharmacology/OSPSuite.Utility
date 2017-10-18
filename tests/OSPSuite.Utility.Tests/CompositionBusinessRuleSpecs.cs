using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;

namespace OSPSuite.Utility.Tests
{
   public class When_checking_the_validity_of_a_validatable_object_using_the_composition_business_rule : StaticContextSpecification
   {
      private WrapperObject _wrapperObject;

      protected override void Context()
      {
         _wrapperObject = new WrapperObject();
      }

      [Observation]
      public void should_return_a_valid_state_if_the_composed_property_is_valid()
      {
         _wrapperObject.MyValue = 4.5;
         _wrapperObject.IsValid().ShouldBeTrue();
         _wrapperObject.Validate(x => x.MyValue, 3.5).IsEmpty.ShouldBeTrue();
      }

      [Observation]
      public void should_return_an_invalid_state_if_the_composed_property_is_invalid()
      {
         _wrapperObject.MyValue = 3000;
         _wrapperObject.IsValid().ShouldBeFalse();
         _wrapperObject.Validate(x => x.MyValue,5000).IsEmpty.ShouldBeFalse();
      }
   }

   internal class WrapperObject : IValidatable
   {
      private readonly ObjectWithRules _parameter;
      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();

      public double MyValue
      {
         get => _parameter.Value;
         set => _parameter.Value = value;
      }

      public WrapperObject()
      {
         _parameter = new ObjectWithRules(ObjectWithRules.AllRules.All);
         Rules.Add(new CompositionBusinessRule<ObjectWithRules, double>(_parameter, x => x.Value, nameof(MyValue)));
      }
   }
}