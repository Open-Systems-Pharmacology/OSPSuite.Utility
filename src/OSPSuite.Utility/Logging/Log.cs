using OSPSuite.Utility.Container;
using OSPSuite.Utility.Logging.TextWriterLogging;

namespace OSPSuite.Utility.Logging
{
   public static class Log
   {
      public static ILogger For<T>(T objectThatWantsToLog)
      {
         return GetLogFactory().CreateFor(objectThatWantsToLog.GetType());
      }

      public static ILogFactory GetLogFactory()
      {
         //LogFactory should never crash when required
         try
         {
            return IoC.Resolve<ILogFactory>();
         }
         catch (InterfaceResolutionException)
         {
            return new TextWriterLogFactory();
         }
      }
   }
}