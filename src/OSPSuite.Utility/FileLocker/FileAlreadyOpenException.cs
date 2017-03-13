using System;

namespace OSPSuite.Utility.FileLocker
{
   public class FileAlreadyOpenException : Exception
   {
      private const string _exceptionMessage = "File '{0}' is already accessed by user '{1}'.";

      public string UserAccessingTheFile { get; }

      public FileAlreadyOpenException(string fileToAccess, string userAccessingTheFile)
         : base(string.Format(_exceptionMessage, fileToAccess, userAccessingTheFile))
      {
         UserAccessingTheFile = userAccessingTheFile;
      }
   }
}