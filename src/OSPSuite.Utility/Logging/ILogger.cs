using System;

namespace OSPSuite.Utility.Logging
{
   public interface ILogger
   {
      void Informational(string infoToLog);
      void Error(Exception e);
      void Debug(string debugInfo);
   }
}