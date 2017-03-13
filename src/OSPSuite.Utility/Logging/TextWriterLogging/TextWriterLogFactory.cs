using System;

namespace OSPSuite.Utility.Logging.TextWriterLogging
{
   public class TextWriterLogFactory : ILogFactory
   {
      public ILogger CreateFor(Type typeThatRequiresLoggingService)
      {
         return new TextWriterLogger(Console.Out);
      }
   }
}