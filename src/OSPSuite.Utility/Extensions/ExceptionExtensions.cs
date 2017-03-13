using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Exceptions;

namespace OSPSuite.Utility.Extensions
{
   public static class ExceptionExtensions
   {
      public static void DoWithinExceptionHandler(this object callerObject, Action actionToExecute)
      {
         try
         {
            actionToExecute();
         }
         catch (Exception e)
         {
            IoC.Resolve<IExceptionManager>().LogException(e);
         }
      }

      public static string FullMessage(this Exception ex)
      {
         var builder = new StringBuilder();
         builder.AppendLine(ex.Message);
         ex = ex.InnerException;
         while (ex != null)
         {
            builder.AppendLine("-----------------------------------------------------");
            builder.AppendLine(ex.Message);
            ex = ex.InnerException;
         }
         return builder.ToString().Trim();
      }

      public static string FullStackTrace(this Exception ex)
      {
         var builder = new StringBuilder();

         var exceptionList = new List<Exception> {ex};

         ex = ex.InnerException;
         while (ex != null)
         {
            exceptionList.Add(ex);
            ex = ex.InnerException;
         }

         builder.AppendLine(exceptionList.Last().StackTrace);
         for (int i = exceptionList.Count - 2; i >= 0; i--)
         {
            builder.AppendLine(" --- End of inner exception stack trace ---");
            builder.AppendLine(exceptionList[i].StackTrace);
         }

         return builder.ToString().Trim();
      }
   }
}