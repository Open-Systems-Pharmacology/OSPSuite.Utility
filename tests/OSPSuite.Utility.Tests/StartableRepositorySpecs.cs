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
      private const int _attempts = 100;
      private readonly List<int> _doStartCallCounts = new List<int>();
      private readonly List<Exception> _exceptions = new List<Exception>();

      protected override void Because()
      {
         //the race only exists during the very first Start, so each attempt uses a fresh repository
         for (var attempt = 0; attempt < _attempts; attempt++)
         {
            var repository = new StartableRepositoryForSpecs();
            var startSignal = new ManualResetEventSlim(false);
            var threads = new List<Thread>();

            for (var t = 0; t < _threadCount; t++)
            {
               threads.Add(new Thread(() =>
               {
                  startSignal.Wait();
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
            startSignal.Set();
            threads.ForEach(x => x.Join());

            _doStartCallCounts.Add(repository.DoStartCallCount);
         }
      }

      [Observation]
      public void should_start_without_throwing()
      {
         var messages = string.Join(Environment.NewLine, _exceptions.Select(ex => $"{ex.GetType().Name}: {ex.Message}"));
         messages.ShouldBeEqualTo(string.Empty);
      }

      [Observation]
      public void should_only_fill_the_repository_once()
      {
         _doStartCallCounts.Each(x => x.ShouldBeEqualTo(1));
      }
   }

   public class When_starting_a_startable_repository_whose_filling_failed : StaticContextSpecification
   {
      private FailingStartableRepositoryForSpecs _repository;
      private Exception _firstException;
      private Exception _secondException;

      protected override void Context()
      {
         _repository = new FailingStartableRepositoryForSpecs();
      }

      protected override void Because()
      {
         _firstException = Catch(() => _repository.Start());
         _secondException = Catch(() => _repository.Start());
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
      public void should_rethrow_the_original_exception()
      {
         _secondException.ShouldBeEqualTo(_firstException);
      }

      [Observation]
      public void should_not_try_to_fill_the_repository_again()
      {
         _repository.DoStartCallCount.ShouldBeEqualTo(1);
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
      private bool _competingStartReturnedDuringPostStartProcessing;

      protected override void Context()
      {
         _repository = new BlockingPostStartRepositoryForSpecs();
      }

      protected override void Because()
      {
         _initializingThread = new Thread(() => _repository.Start());
         _initializingThread.Start();
         _repository.PostStartProcessingStarted.Wait();

         var competingStartEntered = new ManualResetEventSlim(false);
         _competingThread = new Thread(() =>
         {
            competingStartEntered.Set();
            _repository.Start();
         });
         _competingThread.Start();
         competingStartEntered.Wait();

         //Join returns true only if the competing Start already returned, i.e. while the repository is still starting
         _competingStartReturnedDuringPostStartProcessing = _competingThread.Join(TimeSpan.FromMilliseconds(200));

         _repository.ReleasePostStartProcessing();
         _initializingThread.Join();
         _competingThread.Join();
      }

      [Observation]
      public void should_block_the_other_caller_until_the_post_start_processing_completed()
      {
         _competingStartReturnedDuringPostStartProcessing.ShouldBeFalse();
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

   internal class FailingStartableRepositoryForSpecs : StartableRepository<string>
   {
      public int DoStartCallCount { get; private set; }

      protected override void DoStart()
      {
         DoStartCallCount++;
         throw new InvalidOperationException("cannot fill");
      }

      public override IEnumerable<string> All() => new List<string>();
   }

   internal class BlockingPostStartRepositoryForSpecs : StartableRepository<string>
   {
      private readonly List<string> _values = new List<string>();
      private readonly ManualResetEventSlim _postStartProcessingReleased = new ManualResetEventSlim(false);

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
