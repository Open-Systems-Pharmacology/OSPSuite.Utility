using System;
using System.IO;

namespace OSPSuite.Utility.Logging.TextWriterLogging
{
   public class TextWriterLogger : ILogger
   {
      private readonly TextWriter _writer;

      public TextWriterLogger(TextWriter writer)
      {
         _writer = writer;
      }

      public void Informational(string message)
      {
         _writer.WriteLine(message);
      }

      public void Error(Exception e)
      {
         _writer.WriteLine(e.Message);
      }

      public void Debug(string debugInfo)
      {
         _writer.WriteLine(debugInfo);
      }
   }
}