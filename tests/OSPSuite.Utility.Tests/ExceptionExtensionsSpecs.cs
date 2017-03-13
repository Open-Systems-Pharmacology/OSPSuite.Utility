using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_ExceptionExtensions : StaticContextSpecification
   {
      protected IExceptionManager _exceptionManager;

      public override void GlobalContext()
      {
         _exceptionManager = A.Fake<IExceptionManager>();
         var container = A.Fake<IContainer>();
         A.CallTo(() => container.Resolve<IExceptionManager>()).Returns(_exceptionManager);
         IoC.InitializeWith(container);
         IoC.RegisterImplementationOf(_exceptionManager);
      }

      public override void GlobalCleanup()
      {
         IoC.InitializeWith(null);
      }
   }

   public class When_executing_an_action_for_an_object_within_exception_handler : concern_for_ExceptionExtensions
   {
      private Exception _exception;
      private Action _actionToExecute;

      protected override void Because()
      {
         var myObject = new object();
         _exception = new ApplicationException();
         _actionToExecute = () => { throw _exception; };
         myObject.DoWithinExceptionHandler(_actionToExecute);
      }

      [Observation]
      public void should_catch_any_exception_happening_during_the_execution_and_leverage_th_exception_manager()
      {
         A.CallTo(() => _exceptionManager.LogException(_exception)).MustHaveHappened();
      }
   }
}