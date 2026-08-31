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
      private readonly Lock _sync = new Lock();
      private readonly List<int> _doStartCallCounts = new List<int>();
      private readonly List<Exception> _exceptions = new List<Exception>();

      protected override void Because()
      {
         //BDDHelper runs Context and Because once per observation, so accumulating collections must start empty each time
         _doStartCallCounts.Clear();
         _exceptions.Clear();

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
                  try
                  {
                     //release all threads into Start at the same moment to maximize the race window
                     barrier.SignalAndWait(StartableRepositorySpecsHelper.SpecTimeout);
                     repository.Start();
                  }
                  catch (Exception ex)
                  {
                     lock (_sync)
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
         _exception = StartableRepositorySpecsHelper.Catch(() => _repository.Start());
         _repository.Start();
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

   public class When_starting_a_startable_repository_concurrently_while_the_first_filling_fails : StaticContextSpecification
   {
      private const int _threadCount = 8;
      private FailingOnceStartableRepositoryForSpecs _repository;
      private readonly Lock _sync = new Lock();
      private readonly List<Exception> _exceptions = new List<Exception>();
      private readonly List<int> _contentCountsSeenByNonThrowingThreads = new List<int>();

      protected override void Context()
      {
         _repository = new FailingOnceStartableRepositoryForSpecs();
      }

      protected override void Because()
      {
         //cleared for the per-observation rerun, see When_starting_a_startable_repository_concurrently
         _exceptions.Clear();
         _contentCountsSeenByNonThrowingThreads.Clear();

         using var barrier = new Barrier(_threadCount);
         var threads = new List<Thread>();

         for (var t = 0; t < _threadCount; t++)
         {
            threads.Add(new Thread(() =>
            {
               barrier.SignalAndWait(StartableRepositorySpecsHelper.SpecTimeout);
               try
               {
                  _repository.Start();
               }
               catch (Exception ex)
               {
                  lock (_sync)
                  {
                     _exceptions.Add(ex);
                  }

                  return;
               }

               lock (_sync)
               {
                  _contentCountsSeenByNonThrowingThreads.Add(_repository.All().Count());
               }
            }));
         }

         threads.ForEach(x => x.Start());
         threads.ForEach(x => x.Join());
      }

      [Observation]
      public void should_report_the_failure_to_exactly_the_one_caller_that_ran_the_failing_filling()
      {
         var messages = string.Join(Environment.NewLine, _exceptions.Select(ex => $"{ex.GetType().Name}: {ex.Message} @ {ex.StackTrace}"));
         _exceptions.Count.ShouldBeEqualTo(1, messages);
         _exceptions[0].ShouldBeAnInstanceOf<InvalidOperationException>();
      }

      [Observation]
      public void should_retry_the_filling_exactly_once()
      {
         _repository.DoStartCallCount.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_let_every_other_caller_observe_the_consistent_content()
      {
         _contentCountsSeenByNonThrowingThreads.Count.ShouldBeEqualTo(_threadCount - 1);
         _contentCountsSeenByNonThrowingThreads.Each(x => x.ShouldBeEqualTo(1));
      }
   }

   public class When_starting_a_startable_repository_whose_post_start_processing_failed : StaticContextSpecification
   {
      private FailingOncePostStartRepositoryForSpecs _repository;
      private Exception _exception;

      protected override void Context()
      {
         _repository = new FailingOncePostStartRepositoryForSpecs();
      }

      protected override void Because()
      {
         _exception = StartableRepositorySpecsHelper.Catch(() => _repository.Start());
         _repository.Start();
      }

      [Observation]
      public void should_let_the_failure_propagate_to_the_caller()
      {
         _exception.ShouldBeAnInstanceOf<InvalidOperationException>();
      }

      [Observation]
      public void should_not_publish_the_repository_as_started_and_run_the_full_start_again_on_the_next_call()
      {
         _repository.DoStartCallCount.ShouldBeEqualTo(2);
         _repository.PostStartCallCount.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_not_duplicate_the_content_when_the_filling_resets_partial_state()
      {
         _repository.All().Count().ShouldBeEqualTo(1);
      }
   }

   public class When_the_failing_post_start_processing_does_not_reset_its_partial_state : StaticContextSpecification
   {
      private NonResettingFailingPostStartRepositoryForSpecs _repository;
      private Exception _firstException;
      private Exception _retryException;

      protected override void Context()
      {
         _repository = new NonResettingFailingPostStartRepositoryForSpecs();
      }

      protected override void Because()
      {
         _firstException = StartableRepositorySpecsHelper.Catch(() => _repository.Start());
         _retryException = StartableRepositorySpecsHelper.Catch(() => _repository.Start());
      }

      [Observation]
      public void should_let_the_original_failure_propagate_to_the_first_caller()
      {
         _firstException.ShouldBeAnInstanceOf<InvalidOperationException>();
      }

      //documents the known limitation of the retry contract: a hook that fills a cache without resetting it
      //fails loudly when the retry re-runs it - it never silently serves duplicated content. The exact exception
      //type is Cache's business, so only loudness and the absence of duplication are asserted
      [Observation]
      public void should_fail_loudly_on_the_retry_instead_of_silently_duplicating_the_hook_built_cache()
      {
         _retryException.ShouldNotBeNull();
         _repository.All().Count().ShouldBeEqualTo(1);
      }
   }

   public class When_a_repository_filling_tries_to_use_the_repository_it_is_currently_filling : StaticContextSpecification
   {
      private ReentrantDoStartRepositoryForSpecs _repository;
      private Exception _exception;

      protected override void Context()
      {
         _repository = new ReentrantDoStartRepositoryForSpecs();
      }

      protected override void Because()
      {
         _exception = StartableRepositorySpecsHelper.Catch(() => _repository.Start());
      }

      [Observation]
      public void should_fail_with_a_clear_error_instead_of_silently_returning_empty_content()
      {
         _exception.ShouldBeAnInstanceOf<InvalidOperationException>();
         _exception.Message.Contains(nameof(ReentrantDoStartRepositoryForSpecs)).ShouldBeTrue();
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
         _repository.PostStartProcessingStarted.Wait(StartableRepositorySpecsHelper.SpecTimeout).ShouldBeTrue();

         using var competingStartEntered = new ManualResetEventSlim(false);
         _competingThread = new Thread(() =>
         {
            competingStartEntered.Set();
            _repository.Start();
            //captured at the moment Start returns: a Start that does not block would observe false here
            _postStartProcessingHadCompletedWhenCompetingStartReturned = _repository.PostStartProcessingCompleted;
         });
         _competingThread.Start();
         competingStartEntered.Wait(StartableRepositorySpecsHelper.SpecTimeout).ShouldBeTrue();

         //opportunity window, not an assertion: only a few instructions separate the signal above from the lock,
         //so a non-blocking Start would almost surely return inside this window and record false. A blocking Start
         //ignores the window entirely, so scheduling can only delay regression detection, never fail the spec.
         _competingThread.Join(TimeSpan.FromMilliseconds(250));

         _repository.ReleasePostStartProcessing();
         _initializingThread.Join();
         _competingThread.Join();
      }

      public override void Cleanup()
      {
         //safety net for a Because that failed midway: unpark the initializing thread before disposing the events
         _repository?.ReleasePostStartProcessing();
         _initializingThread?.Join(StartableRepositorySpecsHelper.SpecTimeout);
         _competingThread?.Join(StartableRepositorySpecsHelper.SpecTimeout);
         _repository?.Dispose();
         base.Cleanup();
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

   internal static class StartableRepositorySpecsHelper
   {
      //bounded waits turn a hanging concurrency spec into a diagnosable CI failure
      public static readonly TimeSpan SpecTimeout = TimeSpan.FromSeconds(30);

      public static Exception Catch(Action action)
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
      private int _doStartCallCount;

      public int DoStartCallCount => _doStartCallCount;

      protected override void DoStart()
      {
         var callCount = Interlocked.Increment(ref _doStartCallCount);
         //reset partial state so that a retry after a failure does not duplicate entries
         _values.Clear();
         _values.Add("value");
         if (callCount == 1)
            throw new InvalidOperationException("cannot fill");
      }

      public override IEnumerable<string> All() => _values;
   }

   internal class FailingOncePostStartRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();
      public int DoStartCallCount { get; private set; }
      public int PostStartCallCount { get; private set; }

      protected override void DoStart()
      {
         DoStartCallCount++;
         //reset partial state so that a retry after a failure does not duplicate entries
         _values.Clear();
         _values.Add("value");
      }

      protected override void PerformPostStartProcessing()
      {
         PostStartCallCount++;
         if (PostStartCallCount == 1)
            throw new InvalidOperationException("post start processing failed");
      }

      public override IEnumerable<string> All() => _values;
   }

   internal class NonResettingFailingPostStartRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();
      //mirrors the PK-Sim hook pattern: a cache filled via Add without a reset, so a re-run hits the duplicate key
      private readonly Cache<string, string> _cacheBuiltByHook = new Cache<string, string>(getKey: x => x);
      private int _postStartCallCount;

      protected override void DoStart()
      {
         _values.Clear();
         _values.Add("value");
      }

      protected override void PerformPostStartProcessing()
      {
         _postStartCallCount++;
         _values.Each(_cacheBuiltByHook.Add);
         if (_postStartCallCount == 1)
            throw new InvalidOperationException("post start processing failed after filling its cache");
      }

      public override IEnumerable<string> All() => _values;
   }

   internal class ReentrantDoStartRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();

      protected override void DoStart()
      {
         //using the repository while it is being filled would silently return empty content
         _ = All().Count();
      }

      public override IEnumerable<string> All()
      {
         Start();
         return _values;
      }
   }

   internal class BlockingPostStartRepositoryForSpecs : StartableRepository<string>, IDisposable
   {
      private readonly List<string> _values = new List<string>();
      private readonly ManualResetEventSlim _postStartProcessingReleased = new ManualResetEventSlim(false);
      private volatile bool _postStartProcessingCompleted;

      public ManualResetEventSlim PostStartProcessingStarted { get; } = new ManualResetEventSlim(false);
      public bool PostStartProcessingCompleted => _postStartProcessingCompleted;
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
         _postStartProcessingReleased.Wait(StartableRepositorySpecsHelper.SpecTimeout);
         _postStartProcessingCompleted = true;
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
