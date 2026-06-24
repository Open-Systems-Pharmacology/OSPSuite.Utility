using System;
using System.Collections.Generic;
using System.Linq;
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
      protected RecordingSynchronizationContext _synchronizationContext;

      protected override void Context()
      {
         _exceptionManager = new ExceptionManagerForTests();
         _synchronizationContext = new RecordingSynchronizationContext();
         sut = new EventPublisher(_synchronizationContext, _exceptionManager);
      }

      internal class ExceptionManagerForTests : ExceptionManagerBase
      {
         public override void LogException(Exception ex)
         {
            Exception = ex;
         }

         internal Exception Exception { get; set; }
      }

      // Runs the given action on a fresh thread whose current SynchronizationContext is the one
      // supplied, so a test can control whether PublishEvent sees itself as running on the
      // publisher's context thread (pass _synchronizationContext) or off it (pass null). Joining
      // the thread before returning makes the recorded flags safe to read on the test thread.
      protected static void runOnThreadWithSynchronizationContext(SynchronizationContext context, Action action)
      {
         Exception thrown = null;
         var thread = new Thread(() =>
         {
            SynchronizationContext.SetSynchronizationContext(context);
            try
            {
               action();
            }
            catch (Exception ex)
            {
               thrown = ex;
            }
         });
         thread.Start();
         thread.Join();
         if (thrown != null)
            throw thrown;
      }

      // Both Send and Post run the callback synchronously so listener notification stays
      // deterministic in tests (the default SynchronizationContext.Post would queue to the
      // ThreadPool and race the observations). The flags record which path PublishEvent chose.
      protected class RecordingSynchronizationContext : SynchronizationContext
      {
         public bool PostWasCalled { get; private set; }
         public bool SendWasCalled { get; private set; }

         public override void Post(SendOrPostCallback d, object state)
         {
            PostWasCalled = true;
            d(state);
         }

         public override void Send(SendOrPostCallback d, object state)
         {
            SendWasCalled = true;
            d(state);
         }
      }
   }

   public abstract class concern_for_publishing_an_event : concern_for_EventPublisher
   {
      protected object _eventToPublish;
      protected IListener<object> _listener;

      protected override void Context()
      {
         base.Context();
         _eventToPublish = A.Fake<object>();
         _listener = A.Fake<IListener<object>>();
         sut.AddListener(_listener);
      }
   }

   public class When_publishing_an_event_on_behalf_of_a_publisher : concern_for_publishing_an_event
   {
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

   public class When_publishing_an_event_from_the_thread_that_owns_the_synchronization_context : concern_for_publishing_an_event
   {
      protected override void Because()
      {
         // simulate publishing from the UI thread: the publishing thread's current context
         // is the very context the publisher dispatches to
         runOnThreadWithSynchronizationContext(_synchronizationContext, () => sut.PublishEvent(_eventToPublish));
      }

      [Observation]
      public void should_dispatch_synchronously_through_the_blocking_send_path()
      {
         _synchronizationContext.SendWasCalled.ShouldBeTrue();
         _synchronizationContext.PostWasCalled.ShouldBeFalse();
      }

      [Observation]
      public void should_notify_all_listeners_of_the_event()
      {
         A.CallTo(() => _listener.Handle(_eventToPublish)).MustHaveHappened();
      }
   }

   public class When_publishing_an_event_from_a_thread_that_does_not_own_the_synchronization_context : concern_for_publishing_an_event
   {
      protected override void Because()
      {
         // simulate publishing from a background thread: the publishing thread does not share
         // the publisher's context
         runOnThreadWithSynchronizationContext(null, () => sut.PublishEvent(_eventToPublish));
      }

      [Observation]
      public void should_dispatch_through_the_non_blocking_post_path()
      {
         _synchronizationContext.PostWasCalled.ShouldBeTrue();
         _synchronizationContext.SendWasCalled.ShouldBeFalse();
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

   public class When_publishing_events_concurrently_while_listeners_are_added_and_removed : concern_for_EventPublisher
   {
      private List<Exception> _exceptions;
      private int _completedOperations;
      private const int _threadCount = 8;
      private const int _iterations = 5000;
      // publisher loops + mutator loops, each running _threadCount x _iterations operations
      private const int _expectedOperationCount = 2 * _threadCount * _iterations;

      protected override void Context()
      {
         base.Context();
         _exceptions = new List<Exception>();
         // seed a handful of stable listeners so every publish has a non-trivial list to snapshot
         for (var i = 0; i < 50; i++)
         {
            sut.AddListener(new StressListener());
         }
      }

      protected override void Because()
      {
         var startSignal = new ManualResetEventSlim(false);
         var threads = new List<Thread>();

         // publisher threads: each PublishEvent prunes and snapshots the listener list
         for (var t = 0; t < _threadCount; t++)
         {
            threads.Add(threadRunning(startSignal, () =>
            {
               for (var i = 0; i < _iterations; i++)
               {
                  sut.PublishEvent(new object());
                  Interlocked.Increment(ref _completedOperations);
               }
            }));
         }

         // mutator threads: churn the listener list so Add (which can grow the backing
         // array) overlaps the publishers' snapshot
         for (var t = 0; t < _threadCount; t++)
         {
            threads.Add(threadRunning(startSignal, () =>
            {
               for (var i = 0; i < _iterations; i++)
               {
                  var listener = new StressListener();
                  sut.AddListener(listener);
                  sut.RemoveListener(listener);
                  Interlocked.Increment(ref _completedOperations);
               }
            }));
         }

         threads.ForEach(x => x.Start());
         startSignal.Set();
         threads.ForEach(x => x.Join());
      }

      private Thread threadRunning(ManualResetEventSlim startSignal, Action action)
      {
         return new Thread(() =>
         {
            startSignal.Wait();
            try
            {
               action();
            }
            catch (Exception ex)
            {
               lock (_exceptions)
               {
                  _exceptions.Add(ex);
               }
            }
         });
      }

      // Single observation on purpose: the BDD harness re-runs Context/Because per [Observation],
      // so a second observation would silently re-run the whole stress loop.
      [Observation]
      public void should_complete_all_concurrent_operations_without_throwing()
      {
         var messages = string.Join(Environment.NewLine, _exceptions.Select(ex => $"{ex.GetType().Name}: {ex.Message}"));
         messages.ShouldBeEqualTo(string.Empty);
         // guard against a vacuous pass: confirm every publisher and mutator thread ran its full loop
         _completedOperations.ShouldBeEqualTo(_expectedOperationCount);
      }

      private class StressListener : IListener<object>
      {
         public void Handle(object eventToHandle)
         {
            // no-op; the test exercises the publish/listener-list machinery, not the handler
         }
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