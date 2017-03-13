using System;
using System.Reflection;

namespace OSPSuite.Utility.Container
{
   public class AssemblyScannerException : Exception
   {
      private readonly string _currentMessage;
      private const string _assemblyScannerExceptionMessage = "Unable to scan assembly.";

      public AssemblyScannerException(Exception e)
         : base(_assemblyScannerExceptionMessage, e)
      {
         _currentMessage = _assemblyScannerExceptionMessage;
         var loadException = e as ReflectionTypeLoadException;
         if (loadException == null) return;

         foreach (var exception in loadException.LoaderExceptions)
         {
            _currentMessage += $"\n{exception.Message}";
         }
      }

      public override string Message
      {
         get { return _currentMessage; }
      }
   }
}