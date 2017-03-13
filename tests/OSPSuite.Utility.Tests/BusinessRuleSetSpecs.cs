using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Validation;

namespace OSPSuite.Utility.Tests
{
   public abstract class BusinessRuleSetSpecs : ContextSpecification<IBusinessRuleSet>
   {
      protected ObjectWithRules _objectWithRules;

      protected override void Context()
      {
         sut = ObjectWithRules.AllRules.Default;
         _objectWithRules = new ObjectWithRules(sut);
      }
   }

   public class When_initialized_with_a_set_of_rules_and_asked_if_it_empty : ContextSpecification<IBusinessRuleSet>
   {
      private bool result;

      [Observation]
      public void shoul_return_false()
      {
         result.ShouldBeFalse();
      }

      protected override void Because()
      {
         result = sut.IsEmpty;
      }

      protected override void Context()
      {
         sut = ObjectWithRules.AllRules.Default;
      }
   }

   public class When_retrieving_the_number_of_rules_in_the_set : ContextSpecification<IBusinessRuleSet>
   {
      private int result;

      [Observation]
      public void shoul_return_the_accurate_number_of_rules()
      {
         result.ShouldBeEqualTo(2);
      }

      protected override void Because()
      {
         result = sut.Count;
      }

      protected override void Context()
      {
         sut = ObjectWithRules.AllRules.Default;
      }
   }

   public class When_initialized_with_an_empty_set_of_rules_and_asked_if_it_empty : ContextSpecification<IBusinessRuleSet>
   {
      private bool result;

      [Observation]
      public void shoul_return_true()
      {
         result.ShouldBeTrue();
      }

      protected override void Because()
      {
         result = sut.IsEmpty;
      }

      protected override void Context()
      {
         sut = new BusinessRuleSet();
      }
   }

   public class When_adding_an_existing_rule_to_the_set : BusinessRuleSetSpecs
   {
      private int numberOrRules;

      [Observation]
      public void should_not_change_the_set_of_rules()
      {
         sut.Count.ShouldBeEqualTo(numberOrRules);
      }

      protected override void Because()
      {
         sut.Add(ObjectWithRules.AllRules.GreaterThanZero);
      }

      protected override void Context()
      {
         base.Context();
         numberOrRules = sut.Count;
      }
   }

   public class When_adding_a_range_of_rules_to_the_set : BusinessRuleSetSpecs
   {
      private int numberOrRules;

      [Observation]
      public void should_add_the_rules_to_the_existing_set_of_rules()
      {
         sut.Count.ShouldBeEqualTo(numberOrRules + 2);
         sut.Contains(ObjectWithRules.AllRules.SmallerThanTwoThousand).ShouldBeTrue();
         sut.Contains(ObjectWithRules.AllRules.SmallerThanOneThousand).ShouldBeTrue();
      }

      protected override void Because()
      {
         sut.AddRange(new[] {ObjectWithRules.AllRules.SmallerThanTwoThousand, ObjectWithRules.AllRules.SmallerThanOneThousand});
      }

      protected override void Context()
      {
         base.Context();
         numberOrRules = sut.Count;
      }
   }

   public class When_adding_a_new_rule_to_the_set : BusinessRuleSetSpecs
   {
      private int numberOrRules;

      [Observation]
      public void should_add_the_rule_to_the_list_of_rules()
      {
         sut.Count.ShouldBeEqualTo(numberOrRules + 1);
         sut.Contains(ObjectWithRules.AllRules.SmallerThanOneThousand).ShouldBeTrue();
      }

      protected override void Because()
      {
         sut.Add(ObjectWithRules.AllRules.SmallerThanOneThousand);
      }

      protected override void Context()
      {
         base.Context();
         numberOrRules = sut.Count;
      }
   }

   public class When_adding_a_rule_for_a_derived_type : ContextSpecification<IBusinessRuleSet>
   {
      [Observation]
      public void should_add_the_rule_to_the_business_rule_set()
      {
         sut.Contains(DerivedObjectWithRules.OtherRules.AnotherRuleToAdd).ShouldBeTrue();
      }

      protected override void Because()
      {
         sut.Add(DerivedObjectWithRules.OtherRules.AnotherRuleToAdd);
      }

      protected override void Context()
      {
         sut = ObjectWithRules.AllRules.Default;
      }
   }

   public class When_retrieving_the_subset_of_rules_broken_by_an_item : BusinessRuleSetSpecs
   {
      private IBusinessRuleSet result;

      [Observation]
      public void should_return_the_subset_of_rules_indeed_broken_by_the_item()
      {
         result.Count.ShouldBeEqualTo(2);
         result.Contains(ObjectWithRules.AllRules.GreaterThanZero).ShouldBeFalse();
         result.Contains(ObjectWithRules.AllRules.Name).ShouldBeTrue();
         result.Contains(ObjectWithRules.AllRules.SmallerThanOneThousand).ShouldBeTrue();
      }

      protected override void Because()
      {
         result = sut.BrokenBy(_objectWithRules);
      }

      protected override void Context()
      {
         base.Context();
         sut.Add(ObjectWithRules.AllRules.SmallerThanOneThousand);
         _objectWithRules.Name = string.Empty;
         _objectWithRules.Value = 1500;
      }
   }

   public class When_asking_if_a_valid_value_for_a_property_would_break_the_rules_for_an_object_with_an_invalid_value_for_that_property : BusinessRuleSetSpecs
   {
      private IBusinessRuleSet result;
      private double validValue;

      [Observation]
      public void should_return_an_empty_subset()
      {
         result.IsEmpty.ShouldBeTrue();
      }

      protected override void Because()
      {
         result = sut.BrokenBy(_objectWithRules, "Value", validValue);
      }

      protected override void Context()
      {
         sut = new BusinessRuleSet(
            ObjectWithRules.AllRules.GreaterThanZero,
            ObjectWithRules.AllRules.SmallerThanOneThousand);
         _objectWithRules = new ObjectWithRules(sut) {Name = string.Empty, Value = 1500};
         validValue = 70;
      }
   }

   public class When_asking_if_an_invalid_value_for_a_property_would_break_the_rules : BusinessRuleSetSpecs
   {
      private IBusinessRuleSet result;
      private double invalidValue;

      [Observation]
      public void should_return_the_subset_of_rules_indeed_broken_by_the_item()
      {
         result.Count.ShouldBeEqualTo(1);
         result.Contains(ObjectWithRules.AllRules.SmallerThanOneThousand).ShouldBeTrue();
      }

      protected override void Because()
      {
         result = sut.BrokenBy(_objectWithRules, "Value", invalidValue);
      }

      protected override void Context()
      {
         sut = new BusinessRuleSet(
            ObjectWithRules.AllRules.GreaterThanZero,
            ObjectWithRules.AllRules.SmallerThanOneThousand);
         _objectWithRules = new ObjectWithRules(sut) {Name = string.Empty, Value = 500};
         invalidValue = 1100;
      }
   }

   public class when_validating_an_object_with_rule_that_are_not_defined_for_that_object : BusinessRuleSetSpecs
   {
      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.BrokenBy(_objectWithRules)).ShouldThrowAn<InvalidRuleTypeException>();
      }

      protected override void Context()
      {
         sut = new BusinessRuleSet(
            ObjectWithRules.AllRules.GreaterThanZero,
            ObjectWithRules.AllRules.SmallerThanOneThousand,
            DerivedObjectWithRules.OtherRules.AnotherRuleToAdd
         );
         _objectWithRules = new ObjectWithRules(sut) {Name = string.Empty, Value = 500};
      }
   }

   public class when_validating_an_object_for_which_a_rule_from_the_base_class_and_a_rule_from_the_derived_class_are_broken_ : BusinessRuleSetSpecs
   {
      private IBusinessRuleSet result;
      private DerivedObjectWithRules _derivedObjectWithRules;

      [Observation]
      public void should_return_the_subset_of_rules_indeed_broken_by_the_item()
      {
         result.Count.ShouldBeEqualTo(2);
         result.Contains(ObjectWithRules.AllRules.SmallerThanOneThousand).ShouldBeTrue();
         result.Contains(DerivedObjectWithRules.OtherRules.AnotherRuleToAdd).ShouldBeTrue();
         result.Contains(ObjectWithRules.AllRules.GreaterThanZero).ShouldBeFalse();
      }

      protected override void Because()
      {
         result = sut.BrokenBy(_derivedObjectWithRules);
      }

      protected override void Context()
      {
         sut = new BusinessRuleSet(
            ObjectWithRules.AllRules.GreaterThanZero,
            ObjectWithRules.AllRules.SmallerThanOneThousand,
            DerivedObjectWithRules.OtherRules.AnotherRuleToAdd
         );
         _derivedObjectWithRules = new DerivedObjectWithRules(sut) {Name = string.Empty, Value = 1100};
      }
   }

   public class ObjectWithRules : IValidatable
   {
      public IBusinessRuleSet Rules { get; private set; }

      public ObjectWithRules()
         : this(AllRules.Default)
      {
      }

      public ObjectWithRules(IBusinessRuleSet rules)
      {
         Rules = rules;
      }

      public string Name { get; set; }
      public string Description { get; set; }
      public double Value { get; set; }

      public static class AllRules
      {
         public static IBusinessRule Name = CreateRule.For<ObjectWithRules>()
            .Property(p => p.Name)
            .WithRule((p, value) => !value.IsNullOrEmpty())
            .WithError("Name is required");

         public static IBusinessRule GreaterThanZero = CreateRule.For<ObjectWithRules>()
            .Property(p => p.Value)
            .WithRule((p, value) => value >= 0)
            .WithError("Value should be greater than zero");

         public static IBusinessRule SmallerThanOneThousand = CreateRule.For<ObjectWithRules>()
            .Property(p => p.Value)
            .WithRule((p, value) => value <= 1000)
            .WithError("Value should be smaller than 1000");

         public static IBusinessRule SmallerThanTwoThousand = CreateRule.For<ObjectWithRules>()
            .Property(p => p.Value)
            .WithRule((p, value) => value <= 2000)
            .WithError("Value should be smaller than 2000");

         public static IBusinessRuleSet Default
         {
            get { return new BusinessRuleSet(Name, GreaterThanZero); }
         }
      }
   }

   public class AnotherObjectWithRules : IValidatable
   {
      public IBusinessRuleSet Rules { get; private set; }

      public AnotherObjectWithRules(IBusinessRuleSet rules)
      {
         Rules = rules;
      }

      public string Name { get; set; }

      public static class AllRules
      {
         public static IBusinessRule Name = CreateRule.For<AnotherObjectWithRules>()
            .Property(p => p.Name)
            .WithRule((p, value) => !value.IsNullOrEmpty())
            .WithError("Name is required");

         public static IBusinessRuleSet Default
         {
            get { return new BusinessRuleSet(Name); }
         }
      }
   }

   public class DerivedObjectWithRules : ObjectWithRules
   {
      public DerivedObjectWithRules(IBusinessRuleSet rules) : base(rules)
      {
      }

      public static class OtherRules
      {
         public static IBusinessRule AnotherRuleToAdd = CreateRule.For<DerivedObjectWithRules>()
            .Property(p => p.Name)
            .WithRule((p, value) => !value.IsNullOrEmpty())
            .WithError("Name is required");
      }
   }

   public class ObjectWithRulesDTO : IValidatable
   {
      private readonly ObjectWithRules _objectWithRules;

      public ObjectWithRulesDTO(ObjectWithRules objectWithRules)
      {
         _objectWithRules = objectWithRules;
         Rules.Add(new ValidatableBusinessRule(_objectWithRules));
      }

      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();

      public double Value { get; set; }
      public string Name { get; set; }
   }
}