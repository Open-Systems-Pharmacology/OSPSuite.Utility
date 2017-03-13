using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public class When_creating_an_object_reference_for_a_valid_object : ContextSpecification<WeakRef<IAnInterface>>
   {
      private IAnInterface _objectToBeReferenced;

      [Observation]
      public void should_not_hold_a_reference_to_the_object_when_the_object_is_garbage_collected()
      {
         _objectToBeReferenced = null;
         GC.Collect();
         var wr = new WeakReference(_objectToBeReferenced);
         wr.IsAlive.ShouldBeFalse();
      }

      [Observation]
      public void should_return_the_object_when_asked_for_the_it()
      {
         var underlyingObject = sut.Target;
         underlyingObject.ShouldBeEqualTo(_objectToBeReferenced);
      }

      [Observation]
      public void should_return_null_if_the_underlying_object_is_not_valid_anymore()
      {
         _objectToBeReferenced = null;
         GC.Collect();
         var underlyingObject = sut.Target;
         underlyingObject.ShouldBeNull();
      }

      protected override void Context()
      {
         _objectToBeReferenced = new AnImplementation();
         sut = new WeakRef<IAnInterface>(_objectToBeReferenced);
      }
   }
}