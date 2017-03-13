using System;

namespace OSPSuite.Utility.Logging
{
   public interface ILogFactory
   {
      ILogger CreateFor(Type typeThatWantsToLog);
   }
}