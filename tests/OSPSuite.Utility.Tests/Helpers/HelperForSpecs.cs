using System;
using System.IO;

namespace OSPSuite.Utility.Tests.Helpers
{
   public static class HelperForSpecs
   {
      /// <summary>
      ///    Returns the full path of the test data file
      /// </summary>
      /// <param name="fileName">The filename and extension</param>
      /// <returns>The full path including the name and extension</returns>
      public static string TestFileFullPath(string fileName)
      {
         var dataFolder = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\.."), "TestFiles");
         return Path.Combine(dataFolder, fileName);
      }
   }
}