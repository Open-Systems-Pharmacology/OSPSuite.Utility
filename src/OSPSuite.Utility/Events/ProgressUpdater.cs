using System;

namespace OSPSuite.Utility.Events
{
   public interface IProgressUpdater : IDisposable
   {
      /// <summary>
      ///    Initializes a progesss with the given number of iterations and throws the event <see cref="ProgressInitEvent" />
      /// </summary>
      /// <param name="numberOfIterations">Number of total iterations in the process</param>
      void Initialize(int numberOfIterations);

      /// <summary>
      ///    Initializes a progress with the given number of iterations and the message and throws the event
      ///    <see cref="ProgressInitEvent" />
      /// </summary>
      /// <param name="numberOfIterations">Number of total iterations in the process</param>
      /// <param name="message">Message that will be forwarded to be for instance logged or displayed</param>
      void Initialize(int numberOfIterations, string message);

      /// <summary>
      ///    Increments the progress of one iteration and throws the event <see cref="ProgressingEvent" />
      /// </summary>
      void IncrementProgress();

      /// <summary>
      ///    Increments the progress of one iteration and throws the event <see cref="ProgressingEvent" /> with the given
      ///    <paramref name="message" />
      /// </summary>
      /// <param name="message">Message that will be forwarded to be for instance logged or displayed</param>
      void IncrementProgress(string message);

      /// <summary>
      ///    Reports the current progress for the iteration given as parameter and throws the event
      ///    <see cref="ProgressingEvent" />
      /// </summary>
      /// <param name="iteration">Iteration indication the progress made</param>
      void ReportProgress(int iteration);

      /// <summary>
      ///    Reports the current progress for the iteration given as parameter and throws the event
      ///    <see cref="ProgressingEvent" />
      /// </summary>
      /// <param name="iteration">Iteration indication the progress made</param>
      /// <param name="numberOfIterations">Number total of iterations</param>
      void ReportProgress(int iteration, int numberOfIterations);

      /// <summary>
      ///    Reports the current progress for the iteration given as parameter and throws the event
      ///    <see cref="ProgressingEvent" /> with the given <paramref name="message" />
      /// </summary>
      /// <param name="iteration">Iteration indication the progress made</param>
      /// <param name="message">Message that will be forwarded to be for instance logged or displayed</param>
      void ReportProgress(int iteration, string message);

      /// <summary>
      ///    Reports the current progress for the iteration given as parameter and throws the event
      ///    <see cref="ProgressingEvent" /> with the given <paramref name="message" />
      /// </summary>
      /// <param name="iteration">Iteration indication the progress made</param>
      /// <param name="numberOfIterations">Number total of iterations</param>
      /// <param name="message">Message that will be forwarded to be for instance logged or displayed</param>
      void ReportProgress(int iteration, int numberOfIterations, string message);

      /// <summary>
      ///    Reports a status for the given <paramref name="message" /> by throwing the event <see cref="StatusMessageEvent" />
      /// </summary>
      /// <param name="message">Message that will be forwarded to be for instance logged or displayed</param>
      void ReportStatusMessage(string message);

      /// <summary>
      ///    Terminates the progress by throwing the event
      /// </summary>
      void Terminate();
   }

   public class ProgressUpdater : IProgressUpdater
   {
      private readonly IEventPublisher _eventPublisher;
      private int _numberOfIterations;
      private int _currentIteration;

      public ProgressUpdater(IEventPublisher eventPublisher)
      {
         _eventPublisher = eventPublisher;
      }

      public virtual void Initialize(int numberOfIterations)
      {
         Initialize(numberOfIterations, string.Empty);
      }

      public virtual void Initialize(int numberOfIterations, string message)
      {
         _numberOfIterations = numberOfIterations;
         _currentIteration = 0;
         _eventPublisher.PublishEvent(new ProgressInitEvent(numberOfIterations, message));
      }

      public virtual void IncrementProgress() => IncrementProgress(string.Empty);

      public virtual void IncrementProgress(string message) => ReportProgress(_currentIteration + 1, message);

      public virtual void ReportProgress(int iteration) => ReportProgress(iteration, _numberOfIterations);

      public virtual void ReportProgress(int iteration, int numberOfIterations) => ReportProgress(iteration, numberOfIterations, string.Empty);

      public virtual void ReportProgress(int iteration, string message) => ReportProgress(iteration, _numberOfIterations, message);

      public virtual void ReportProgress(int iteration, int numberOfIterations, string message)
      {
         _currentIteration = iteration;
         _eventPublisher.PublishEvent(new ProgressingEvent(iteration, PercentFrom(iteration, numberOfIterations), message));
      }

      public virtual void ReportStatusMessage(string message) => _eventPublisher.PublishEvent(new StatusMessageEvent(message));

      protected virtual int PercentFrom(int iteration, int numberOfIterations)
      {
         return (int) Math.Floor((double) iteration * 100 / numberOfIterations);
      }

      public virtual void Terminate()
      {
         _eventPublisher.PublishEvent(new ProgressDoneEvent());
         _eventPublisher.PublishEvent(new StatusMessageEvent(string.Empty));
      }

      protected virtual void Cleanup()
      {
         Terminate();
      }

      #region Disposable properties

      private bool _disposed;

      public void Dispose()
      {
         if (_disposed) return;

         Cleanup();
         GC.SuppressFinalize(this);
         _disposed = true;
      }

      ~ProgressUpdater()
      {
         Cleanup();
      }

      #endregion
   }
}