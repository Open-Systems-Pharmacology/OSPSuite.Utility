using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public class when_changing_the_value_of_correctly_bound_property : ContextSpecification<IPropertyBinder<IAnInterface, string>>
   {
      private IAnInterface target;
      private const string firstName = "Michael";

      [Observation]
      public void should_update_the_value_of_the_property_int_the_target()
      {
         target.FirstName.ShouldBeEqualTo(firstName);
      }

      protected override void Context()
      {
         target = new AnImplementation {FirstName = "toto"};
         var property = typeof(IAnInterface).GetProperty("FirstName");
         sut = new PropertyBinder<IAnInterface, string>(property);
      }

      protected override void Because()
      {
         sut.SetValue(target, firstName);
      }
   }

   public class when_retrieving_the_current_value_of_a_bound_property : ContextSpecification<IPropertyBinder<IAnInterface, string>>
   {
      private IAnInterface target;
      private string result;

      [Observation]
      public void should_return_the_correct_value()
      {
         result.ShouldBeEqualTo(target.FirstName);
      }

      protected override void Context()
      {
         target = new AnImplementation {FirstName = "toto"};
         var property = typeof(IAnInterface).GetProperty("FirstName");
         sut = new PropertyBinder<IAnInterface, string>(property);
      }

      protected override void Because()
      {
         result = sut.GetValue(target);
      }
   }

   public class when_retrieving_the_name_of_a_bound_property : ContextSpecification<IPropertyBinder<IAnInterface, string>>
   {
      private string result;

      [Observation]
      public void should_return_the_correct_name()
      {
         result.ShouldBeEqualTo("FirstName");
      }

      protected override void Context()
      {
         var property = typeof(IAnInterface).GetProperty("FirstName");
         sut = new PropertyBinder<IAnInterface, string>(property);
      }

      protected override void Because()
      {
         result = sut.PropertyName;
      }
   }

   public class when_asking_if_a_readonly_property_can_be_overwritten : ContextSpecification<IPropertyBinder<IAnInterface, string>>
   {
      private bool result;
      private IAnInterface target;

      [Observation]
      public void should_return_false()
      {
         result.ShouldBeFalse();
      }

      protected override void Context()
      {
         target = new AnImplementation {FirstName = "toto"};
         var property = typeof(IAnInterface).GetProperty("ReadOnlyProp");
         sut = new PropertyBinder<IAnInterface, string>(property);
      }

      protected override void Because()
      {
         result = sut.CanSetValueTo(target);
      }
   }

   public class When_being_released : ContextSpecification<IPropertyBinder<IAnInterface, string>>
   {
      private IAnInterface _target;
      private WeakReference _wr;

      protected override void Context()
      {
         _target = new AnImplementation {FirstName = "toto"};
         var property = typeof(IAnInterface).GetProperty("FirstName");
         sut = new PropertyBinder<IAnInterface, string>(property);
         sut.SetValue(_target, "tutu");
         _wr = new WeakReference(_target);
      }

      protected override void Because()
      {
         _target = null;
         sut = null;
         GC.Collect();
      }

      [Observation]
      public void should_not_hold_any_references_to_the_target()
      {
         _wr.IsAlive.ShouldBeFalse();
      }
   }

   public class when_setting_a_value_for_a_readonly_property : ContextSpecification<IPropertyBinder<IAnInterface, string>>
   {
      private IAnInterface target;
      private string originalValue;

      [Observation]
      public void should_not_change_the_original_value()
      {
         target.ReadOnlyProp.ShouldBeEqualTo(originalValue);
      }

      protected override void Context()
      {
         target = new AnImplementation {FirstName = "toto"};
         var property = typeof(IAnInterface).GetProperty("ReadOnlyProp");
         sut = new PropertyBinder<IAnInterface, string>(property);
         originalValue = target.ReadOnlyProp;
      }

      protected override void Because()
      {
         sut.SetValue(target, "tata");
      }
   }
}