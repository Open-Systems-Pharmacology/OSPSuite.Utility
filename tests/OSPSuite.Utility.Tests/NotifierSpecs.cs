using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_Notifier : ContextSpecification<MyObject>
   {
      protected bool _propertyNotified;
      protected bool _changed;
      protected string _propertyName;
      protected string _defaultDescription = "Default Description";
      protected string _defaultName = "Default Name";

      protected override void Context()
      {
         sut = new MyObject
         {
            Description = _defaultDescription,
            Name = _defaultName
         };

         sut.PropertyChanged += (o, e) =>
         {
            _propertyNotified = true;
            _propertyName = e.PropertyName;
         };
         sut.Changed += o => _changed = true;
      }
   }

   public class When_a_property_is_set_for_which_the_on_property_changed_was_defined_dynamically : concern_for_Notifier
   {
      protected override void Because()
      {
         sut.Name = "toto";
      }

      [Observation]
      public void should_notify_the_listener()
      {
         _propertyNotified.ShouldBeTrue();
      }

      [Observation]
      public void should_notify_the_gloabal_listener()
      {
         _changed.ShouldBeTrue();
      }

      [Observation]
      public void the_name_of_the_property_changed_should_have_been_set()
      {
         _propertyName.ShouldBeEqualTo("Name");
      }
   }

   public class When_a_property_is_set_for_which_the_on_property_changed_was_defined_statically : concern_for_Notifier
   {
      protected override void Because()
      {
         sut.Description = "toto";
      }

      [Observation]
      public void should_notify_the_listener()
      {
         _propertyNotified.ShouldBeTrue();
      }

      [Observation]
      public void the_name_of_the_property_changed_should_have_been_set()
      {
         _propertyName.ShouldBeEqualTo("Description");
      }
   }

   public class When_setting_the_same_property_twice : concern_for_Notifier
   {
      protected override void Because()
      {
         sut.Description = _defaultDescription;
      }
      [Observation]
      public void should_not_raise_the_property_notified()
      {
         _propertyNotified.ShouldBeFalse();
      }
   }

   public class MyObject : Notifier
   {
      private string _name;

      public string Name
      {
         get => _name;
         set => SetProperty(ref _name, value, () => Name);
      }

      private string _description;

      public string Description
      {
         get => _description;
         set => SetProperty(ref _description, value);
      }
   }
}