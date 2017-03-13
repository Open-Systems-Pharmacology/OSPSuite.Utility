using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Logging;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_LogSpec : StaticContextSpecification
   {
      protected ILogFactory _logFactory;
      protected ILogger _logger;
      protected IContainer _container;

      protected override void Context()
      {
         _logFactory = A.Fake<ILogFactory>();
         _logger = A.Fake<ILogger>();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.Resolve<ILogFactory>()).Returns(_logFactory);
         _container.RegisterImplementationOf(_logFactory);
         IoC.InitializeWith(_container);
      }

      public override void Cleanup()
      {
         IoC.InitializeWith(null);
      }
   }

   public class when_requested_to_get_a_log_for_an_object_instance : concern_for_LogSpec
   {
      private ILogger _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _logFactory.CreateFor(GetType())).Returns(_logger);
      }

      protected override void Because()
      {
         _result = Log.For(this);
      }

      [Observation]
      public void should_leverage_the_log_factory_to_return_a_logger_bound_to_the_type()
      {
         _result.ShouldBeEqualTo(_logger);
      }
   }
}