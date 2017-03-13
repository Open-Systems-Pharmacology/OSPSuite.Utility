using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_ProgressUpdater : ContextSpecification<IProgressUpdater>
   {
      protected IEventPublisher _eventPublisher;
      protected int _numerOfIterations;

      protected override void Context()
      {
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new ProgressUpdater(_eventPublisher);
      }
   }

   public class When_initializing_the_progress_updater : concern_for_ProgressUpdater
   {
      protected override void Context()
      {
         base.Context();
         _numerOfIterations = 10;
      }

      [Observation]
      public void should_notify_the_progress_init_event_with_the_correct_number_of_iterations()
      {
         ProgressInitEvent raisedEvent = null;
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProgressInitEvent>.Ignored)).Invokes(x => raisedEvent = x.GetArgument<ProgressInitEvent>(0));
         sut.Initialize(_numerOfIterations);
         raisedEvent.NumberOfIterations.ShouldBeEqualTo(_numerOfIterations);
      }
   }

   public class When_terminating_the_progress_updater : concern_for_ProgressUpdater
   {
      protected override void Because()
      {
         sut.Terminate();
      }

      [Observation]
      public void should_notify_that_the_work_in_progress_is_finished()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProgressDoneEvent>.Ignored)).MustHaveHappened();
      }
   }

   public class When_reporting_an_iteration_for_the_work_in_progress_with_a_message : concern_for_ProgressUpdater
   {
      private string _message;

      protected override void Context()
      {
         base.Context();
         _numerOfIterations = 10;
         _message = "Tralala";
         sut.Initialize(_numerOfIterations);
      }

      [Observation]
      public void should_notify_a_progressing_event_with_the_iteration_number_the_overall_percentage_and_the_message()
      {
         ProgressingEvent raisedEvent = null;
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProgressingEvent>.Ignored)).Invokes(x => raisedEvent = x.GetArgument<ProgressingEvent>(0));
         sut.ReportProgress(5, _message);
         raisedEvent.Message.ShouldBeEqualTo(_message);
         raisedEvent.Progress.ShouldBeEqualTo(5);
         raisedEvent.ProgressPercent.ShouldBeEqualTo(50);
      }
   }
}