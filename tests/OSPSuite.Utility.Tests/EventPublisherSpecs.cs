using System;
using System.Threading;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Exceptions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_EventPublisher : ContextSpecification<IEventPublisher>
   {
      private IExceptionManager _exceptionManager;

      protected override void Context()
      {
         _exceptionManager = new ExceptionManagerForTests();
         sut = new EventPublisher(new SynchronizationContext(), _exceptionManager);
      }

      internal class ExceptionManagerForTests : ExceptionManagerBase
      {
         public override void LogException(Exception ex)
         {
            Exception = ex;
         }

         internal Exception Exception { get; set; }
      }
   }

   public class When_publishing_an_event_on_behalf_of_a_publisher : concern_for_EventPublisher
   {
      private object _eventToPublish;
      private IListener<object> _listener;

      protected override void Context()
      {
         base.Context();
         _eventToPublish = A.Fake<object>();
         _listener = A.Fake<IListener<object>>();
         sut.AddListener(_listener);
      }

      protected override void Because()
      {
         sut.PublishEvent(_eventToPublish);
      }

      [Observation]
      public void should_notify_all_listeners_of_the_event()
      {
         A.CallTo(() => _listener.Handle(_eventToPublish)).MustHaveHappened();
      }
   }

   public class When_asked_to_add_a_listener : concern_for_EventPublisher
   {
      private IListener<object> _listener;

      [Observation]
      public void should_register_the_listener_into_the_underlaying_structure()
      {
         sut.Contains(_listener).ShouldBeTrue();
      }

      protected override void Because()
      {
         sut.AddListener(_listener);
      }

      protected override void Context()
      {
         base.Context();
         _listener = A.Fake<IListener<object>>();
      }
   }

   public class When_asked_to_remove_a_listener : concern_for_EventPublisher
   {
      private IListener<object> listener1;
      private IListener<object> listener2;

      [Observation]
      public void should_remove_the_listener_from_the_underlaying_structure()
      {
         sut.Contains(listener1).ShouldBeTrue();
         sut.Contains(listener2).ShouldBeFalse();
      }

      protected override void Because()
      {
         sut.RemoveListener(listener2);
      }

      protected override void Context()
      {
         base.Context();
         listener1 = A.Fake<IListener<object>>();
         listener2 = A.Fake<IListener<object>>();
         sut.AddListener(listener1);
         sut.AddListener(listener2);
      }
   }

   public class When_publishing_an_event_for_a_class_that_listens_to_many_events : concern_for_EventPublisher
   {
      private IFakeListener listener;
      private FakeEvent1 fakeEvent1;
      private FakeEvent2 fakeEvent2;

      [Observation]
      public void shoulld_notify_the_class_only_once()
      {
         A.CallTo(() => listener.Handle(fakeEvent1)).MustHaveHappened();
         A.CallTo(() => listener.Handle(fakeEvent2)).MustNotHaveHappened();
      }

      protected override void Because()
      {
         sut.PublishEvent(fakeEvent1);
      }

      protected override void Context()
      {
         base.Context();
         listener = A.Fake<IFakeListener>();
         fakeEvent1 = A.Fake<FakeEvent1>();
         fakeEvent2 = A.Fake<FakeEvent2>();
         sut.AddListener(listener);
      }
   }

   public class FakeEvent1
   {
   }

   public class FakeEvent2
   {
   }

   public interface IFakeListener : IListener<FakeEvent1>, IListener<FakeEvent2>
   {
   }
}