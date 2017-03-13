using System;
using OSPSuite.Utility.Logging;

namespace OSPSuite.Utility.Extensions
{
   public static class LoggingExtensions
   {
      public static ILogger Log<T>(this T objectToLog)
      {
         return Logging.Log.For(objectToLog);
      }

      public static void LogInfo<T>(this T objectToLog, string infoToLog)
      {
         objectToLog.Log().Informational(infoToLog);
      }

      public static void LogError<T>(this T objectToLog, Exception e)
      {
         objectToLog.Log().Error(e);
      }

      public static void LogError<T>(this T objectToLog) where T : Exception
      {
         objectToLog.Log().Error(objectToLog);
      }

      public static void LogDebug<T>(this T objectToLog, string formattedMessage, params object[] arguments)
      {
         objectToLog.Log().Debug(formattedMessage.FormatWith(arguments));
      }
   }
}