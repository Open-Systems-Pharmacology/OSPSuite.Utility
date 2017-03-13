using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_notifier : ContextSpecification<INotifier>
   {
      protected bool _propertyNotified;
      protected bool _changed;
      protected string _propertyName;

      protected override void Context()
      {
         sut = new MyObject();
         sut.PropertyChanged += (o, e) =>
         {
            _propertyNotified = true;
            _propertyName = e.PropertyName;
         };
         sut.Changed += o => _changed = true;
      }
   }

   public class When_a_property_is_set_for_which_the_on_property_changed_was_defined_dynamically : concern_for_notifier
   {
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

      protected override void Because()
      {
         sut.DowncastTo<MyObject>().Name = "toto";
      }
   }

   public class When_a_property_is_set_for_which_the_on_property_changed_was_defined_statically : concern_for_notifier
   {
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

      protected override void Because()
      {
         sut.DowncastTo<MyObject>().Description = "toto";
      }
   }

   public class MyObject : Notifier
   {
      private string _name;

      public string Name
      {
         get { return _name; }
         set
         {
            _name = value;
            OnPropertyChanged(() => Name);
         }
      }

      private string _description;

      public string Description
      {
         get { return _description; }
         set
         {
            _description = value;
            OnPropertyChanged();
         }
      }
   }
}