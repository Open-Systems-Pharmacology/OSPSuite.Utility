namespace OSPSuite.Utility.Events
{
   public class ProgressInitEvent
   {
      public ProgressInitEvent(int numberOfIterations, string message)
      {
         NumberOfIterations = numberOfIterations;
         Message = message;
      }

      public int NumberOfIterations { get; private set; }
      public string Message { get; private set; }
   }

   public class ProgressingEvent
   {
      public ProgressingEvent(int progress, int progressPercent, string message)
      {
         Progress = progress;
         Message = message;
         ProgressPercent = progressPercent;
      }

      public int ProgressPercent { get; private set; }
      public int Progress { get; private set; }
      public string Message { get; private set; }

      public string ProgressPercentAsString
      {
         get { return $"{ProgressPercent}%"; }
      }
   }

   public class ProgressDoneEvent
   {
   }

   public class StatusMessageEvent
   {
      public StatusMessageEvent(string message)
      {
         Message = message;
      }

      public string Message { get; }
   }
}