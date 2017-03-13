using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_ValidationEngine : ContextSpecification<IValidationEngine>
   {
      protected INotificationFactory _notificationFactory;

      protected override void Context()
      {
         _notificationFactory = A.Fake<INotificationFactory>();
         sut = new ValidationEngine(_notificationFactory);
      }
   }

   public class When_told_to_validate_an_object_that_is_not_validatable : concern_for_ValidationEngine
   {
      private INotification _result;
      private INotification _noNotification;

      protected override void Context()
      {
         base.Context();
         _noNotification = new EmptyNotification();
         A.CallTo(() => _notificationFactory.NoNotification()).Returns(_noNotification);
      }

      protected override void Because()
      {
         _result = sut.Validate(new TimeoutException());
      }

      [Observation]
      public void should_ask_the_factory_to_return_an_empty_notification()
      {
         A.CallTo(() => _notificationFactory.NoNotification()).MustHaveHappened();
      }

      [Observation]
      public void should_not_ask_the_factory_to_validate_the_object()
      {
         A.CallTo(() => _notificationFactory.CreateNotificationFrom(A<IBusinessRuleSet>.Ignored)).MustNotHaveHappened();
      }

      [Observation]
      public void should_return_the_empty_notification()
      {
         _result.ShouldBeEqualTo(_noNotification);
      }
   }
}