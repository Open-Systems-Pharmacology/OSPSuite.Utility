using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_delegate_factory : StaticContextSpecification
   {
      protected SimpleObject _objectUsedInSpecs;

      protected override void Context()
      {
         _objectUsedInSpecs = new SimpleObject();
      }
   }

   public class SimpleObject
   {
      public int OneField;
      public string OneProperty { get; set; }
   }

   public class When_retrieving_the_set_handler_for_a_field : concern_for_delegate_factory
   {
      private SetHandler _setHandler;

      protected override void Context()
      {
         base.Context();
         _setHandler = DelegateFactory.CreateSet(ReflectionHelper.AllFieldsFor(typeof(SimpleObject)).First());
      }

      protected override void Because()
      {
         _setHandler(_objectUsedInSpecs, 5);
      }

      [Observation]
      public void should_be_able_to_set_the_value()
      {
         _objectUsedInSpecs.OneField.ShouldBeEqualTo(5);
      }
   }

   public class When_retrieving_the_get_handler_for_a_field : concern_for_delegate_factory
   {
      private GetHandler _getHandler;

      protected override void Context()
      {
         base.Context();
         _objectUsedInSpecs.OneField = 33;
         _getHandler = DelegateFactory.CreateGet(ReflectionHelper.AllFieldsFor(typeof(SimpleObject)).First());
      }

      [Observation]
      public void should_be_able_to_retrieve_the_value()
      {
         _getHandler(_objectUsedInSpecs).ShouldBeEqualTo(_objectUsedInSpecs.OneField);
      }
   }

   public class When_retrieving_the_set_handler_for_a_property : concern_for_delegate_factory
   {
      private SetHandler _setHandler;

      protected override void Context()
      {
         base.Context();
         _setHandler = DelegateFactory.CreateSet(ReflectionHelper.PropertyFor<SimpleObject, string>(x => x.OneProperty));
      }

      protected override void Because()
      {
         _setHandler(_objectUsedInSpecs, "tralala");
      }

      [Observation]
      public void should_be_able_to_set_the_value()
      {
         _objectUsedInSpecs.OneProperty.ShouldBeEqualTo("tralala");
      }
   }

   public class When_retrieving_the_get_handler_for_a_property : concern_for_delegate_factory
   {
      private GetHandler _getHandler;

      protected override void Context()
      {
         base.Context();
         _objectUsedInSpecs.OneProperty = "toto";
         _getHandler = DelegateFactory.CreateGet(ReflectionHelper.PropertyFor<SimpleObject, string>(x => x.OneProperty));
      }

      [Observation]
      public void should_be_able_to_get_the_value()
      {
         _getHandler(_objectUsedInSpecs).ShouldBeEqualTo(_objectUsedInSpecs.OneProperty);
      }
   }
}