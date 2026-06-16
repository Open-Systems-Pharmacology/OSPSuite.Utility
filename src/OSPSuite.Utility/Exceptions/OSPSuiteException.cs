using System;

namespace OSPSuite.Utility.Exceptions
{
   /// <summary>
   ///    Standard base exception that should be used to defined an application thrown by an OSPSuite application itself
   /// </summary>
   public class OSPSuiteException : Exception
   {
      public OSPSuiteException()
      {
      }

      public OSPSuiteException(string message) : base(message)
      {
      }

      public OSPSuiteException(string message, Exception innerException) : base(message, innerException)
      {
      }
   }
}