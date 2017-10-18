using System;

namespace OSPSuite.Utility.Exceptions
{
   /// <summary>
   ///    This interface defines a common gateway to manage exception. Each application can define its own implementation
   ///    of the Exception manager and log the exception as needed (let it be a file, standard output, email,etc...).
   /// </summary>
   public interface IExceptionManager
   {
      /// <summary>
      ///    Log the given exception in the system
      /// </summary>
      /// <param name="ex">Exception to log</param>
      void LogException(Exception ex);

      /// <summary>
      ///    Action to execute within exception handler
      /// </summary>
      /// <param name="action"></param>
      void Execute(Action action);
   }

   public abstract class ExceptionManagerBase : IExceptionManager
   {
      public abstract void LogException(Exception ex);

      public virtual void Execute(Action action)
      {
         try
         {
            action();
         }
         catch (Exception e)
         {
            LogException(e);
         }
      }
   }
}