using System;
using System.Data;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_IoC : StaticContextSpecification
   {
      protected IContainer _container;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         IoC.InitializeWith(_container);
      }

      public override void Cleanup()
      {
         base.Cleanup();
         IoC.InitializeWith(null);
      }
   }

   public class when_told_to_retrieve_an_implementation_of_an_interface : concern_for_IoC
   {
      private IDbCommand _commandImplementation;
      private IDbCommand _result;

      protected override void Context()
      {
         base.Context();
         _commandImplementation = A.Fake<IDbCommand>();
         A.CallTo(() => _container.Resolve<IDbCommand>()).Returns(_commandImplementation);
      }

      protected override void Because()
      {
         _result = IoC.Resolve<IDbCommand>();
      }

      [Observation]
      public void should_delegate_to_the_real_container_to_retrieve_the_implementation()
      {
         _result.ShouldBeEqualTo(_commandImplementation);
      }
   }

   public class when_an_exception_is_received_from_the_underlying_container : concern_for_IoC
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _container.Resolve<IDbCommand>()).Throws(new InterfaceResolutionException(typeof(IDbCommand), new Exception()));
      }

      [Observation]
      public void should_rethrow_an_interface_resolution_exception()
      {
         The.Action(() => IoC.Resolve<IDbCommand>()).ShouldThrowAn<InterfaceResolutionException>();
      }
   }
}