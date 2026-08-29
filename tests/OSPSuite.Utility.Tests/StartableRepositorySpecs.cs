using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public class When_starting_a_startable_repository_concurrently : StaticContextSpecification
   {
      private const int _threadCount = 16;
      private const int _attempts = 25;
      private readonly List<int> _doStartCallCounts = new List<int>();
      private readonly List<Exception> _exceptions = new List<Exception>();

      protected override void Because()
      {
         //the race only exists during the very first Start, so each attempt uses a fresh repository
         for (var attempt = 0; attempt < _attempts; attempt++)
         {
            var repository = new StartableRepositoryForSpecs();
            using var barrier = new Barrier(_threadCount);
            var threads = new List<Thread>();

            for (var t = 0; t < _threadCount; t++)
            {
               threads.Add(new Thread(() =>
               {
                  //release all threads into Start at the same moment to maximize the race window
                  barrier.SignalAndWait();
                  try
                  {
                     repository.Start();
                  }
                  catch (Exception ex)
                  {
                     lock (_exceptions)
                     {
                        _exceptions.Add(ex);
                     }
                  }
               }));
            }

            threads.ForEach(x => x.Start());
            threads.ForEach(x => x.Join());

            _doStartCallCounts.Add(repository.DoStartCallCount);
         }
      }

      [Observation]
      public void should_start_without_throwing()
      {
         var messages = string.Join(Environment.NewLine, _exceptions.Select(ex => $"{ex.GetType().Name}: {ex.Message}"));
         _exceptions.Count.ShouldBeEqualTo(0, messages);
      }

      [Observation]
      public void should_only_fill_the_repository_once()
      {
         _doStartCallCounts.Each(x => x.ShouldBeEqualTo(1));
      }
   }

   public class When_starting_a_startable_repository_whose_filling_failed : StaticContextSpecification
   {
      private FailingOnceStartableRepositoryForSpecs _repository;
      private Exception _exception;

      protected override void Context()
      {
         _repository = new FailingOnceStartableRepositoryForSpecs();
      }

      protected override void Because()
      {
         _exception = Catch(() => _repository.Start());
         _repository.Start();
      }

      private static Exception Catch(Action action)
      {
         try
         {
            action();
            return null;
         }
         catch (Exception e)
         {
            return e;
         }
      }

      [Observation]
      public void should_let_the_failure_propagate_to_the_caller()
      {
         _exception.ShouldBeAnInstanceOf<InvalidOperationException>();
      }

      [Observation]
      public void should_fill_the_repository_again_on_the_next_start()
      {
         _repository.DoStartCallCount.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_not_duplicate_the_content_when_the_filling_resets_partial_state()
      {
         _repository.All().Count().ShouldBeEqualTo(1);
      }
   }

   public class When_starting_a_startable_repository_whose_post_start_processing_failed : StaticContextSpecification
   {
      private FailingPostStartRepositoryForSpecs _repository;
      private Exception _exception;

      protected override void Context()
      {
         _repository = new FailingPostStartRepositoryForSpecs();
      }

      protected override void Because()
      {
         _exception = Catch(() => _repository.Start());
         _repository.Start();
      }

      private static Exception Catch(Action action)
      {
         try
         {
            action();
            return null;
         }
         catch (Exception e)
         {
            return e;
         }
      }

      [Observation]
      public void should_let_the_failure_propagate_to_the_caller()
      {
         _exception.ShouldBeAnInstanceOf<InvalidOperationException>();
      }

      [Observation]
      public void should_count_as_started_because_the_filling_succeeded()
      {
         _repository.DoStartCallCount.ShouldBeEqualTo(1);
         _repository.PostStartCallCount.ShouldBeEqualTo(1);
      }
   }

   public class When_starting_a_startable_repository_accessing_itself_during_post_start_processing : StaticContextSpecification
   {
      private ReentrantStartableRepositoryForSpecs _repository;

      protected override void Context()
      {
         _repository = new ReentrantStartableRepositoryForSpecs();
      }

      protected override void Because()
      {
         _repository.Start();
      }

      [Observation]
      public void should_only_fill_the_repository_once()
      {
         _repository.DoStartCallCount.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_see_the_filled_content_during_post_start_processing()
      {
         _repository.CountSeenDuringPostStartProcessing.ShouldBeEqualTo(1);
      }
   }

   public class When_starting_a_startable_repository_while_its_post_start_processing_is_still_running : StaticContextSpecification
   {
      private BlockingPostStartRepositoryForSpecs _repository;
      private Thread _initializingThread;
      private Thread _competingThread;
      private bool _postStartProcessingHadCompletedWhenCompetingStartReturned;

      protected override void Context()
      {
         _repository = new BlockingPostStartRepositoryForSpecs();
      }

      protected override void Because()
      {
         _initializingThread = new Thread(() => _repository.Start());
         _initializingThread.Start();
         //the initializing thread is now parked inside PerformPostStartProcessing
         _repository.PostStartProcessingStarted.Wait();

         _competingThread = new Thread(() =>
         {
            _repository.Start();
            //captured at the moment Start returns: a Start that does not block would observe false here
            _postStartProcessingHadCompletedWhenCompetingStartReturned = _repository.PostStartProcessingCompleted;
         });
         _competingThread.Start();

         //not an assertion: merely gives a non-blocking (buggy) Start every opportunity to return early.
         //A blocking Start ignores this window entirely, so the spec cannot fail because of timing.
         _competingThread.Join(TimeSpan.FromMilliseconds(250));

         _repository.ReleasePostStartProcessing();
         _initializingThread.Join();
         _competingThread.Join();
         _repository.Dispose();
      }

      [Observation]
      public void the_competing_start_should_only_return_once_the_post_start_processing_completed()
      {
         _postStartProcessingHadCompletedWhenCompetingStartReturned.ShouldBeTrue();
      }

      [Observation]
      public void should_only_fill_the_repository_once()
      {
         _repository.DoStartCallCount.ShouldBeEqualTo(1);
      }
   }

   internal class StartableRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();
      private int _doStartCallCount;

      public int DoStartCallCount => _doStartCallCount;

      protected override void DoStart()
      {
         Interlocked.Increment(ref _doStartCallCount);
         //give a competing thread a chance to enter DoStart as well
         Thread.Sleep(1);
         _values.Add("value");
      }

      public override IEnumerable<string> All() => _values;
   }

   internal class FailingOnceStartableRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();
      public int DoStartCallCount { get; private set; }

      protected override void DoStart()
      {
         DoStartCallCount++;
         //reset partial state so that a retry after a failure does not duplicate entries
         _values.Clear();
         _values.Add("value");
         if (DoStartCallCount == 1)
            throw new InvalidOperationException("cannot fill");
      }

      public override IEnumerable<string> All() => _values;
   }

   internal class FailingPostStartRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();
      public int DoStartCallCount { get; private set; }
      public int PostStartCallCount { get; private set; }

      protected override void DoStart()
      {
         DoStartCallCount++;
         _values.Add("value");
      }

      protected override void PerformPostStartProcessing()
      {
         PostStartCallCount++;
         throw new InvalidOperationException("post start processing failed");
      }

      public override IEnumerable<string> All() => _values;
   }

   internal class BlockingPostStartRepositoryForSpecs : StartableRepository<string>, IDisposable
   {
      private readonly List<string> _values = new List<string>();
      private readonly ManualResetEventSlim _postStartProcessingReleased = new ManualResetEventSlim(false);
      public volatile bool PostStartProcessingCompleted;

      public ManualResetEventSlim PostStartProcessingStarted { get; } = new ManualResetEventSlim(false);
      public int DoStartCallCount { get; private set; }

      public void ReleasePostStartProcessing() => _postStartProcessingReleased.Set();

      protected override void DoStart()
      {
         DoStartCallCount++;
         _values.Add("value");
      }

      protected override void PerformPostStartProcessing()
      {
         PostStartProcessingStarted.Set();
         _postStartProcessingReleased.Wait();
         PostStartProcessingCompleted = true;
      }

      public void Dispose()
      {
         _postStartProcessingReleased.Dispose();
         PostStartProcessingStarted.Dispose();
      }

      public override IEnumerable<string> All() => _values;
   }

   internal class ReentrantStartableRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();
      public int DoStartCallCount { get; private set; }
      public int CountSeenDuringPostStartProcessing { get; private set; }

      protected override void DoStart()
      {
         DoStartCallCount++;
         _values.Add("value");
      }

      protected override void PerformPostStartProcessing()
      {
         CountSeenDuringPostStartProcessing = All().Count();
      }

      public override IEnumerable<string> All()
      {
         Start();
         return _values;
      }
   }
}
