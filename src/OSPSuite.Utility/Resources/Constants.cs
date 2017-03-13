namespace OSPSuite.Utility.Resources
{
   public static class Constants
   {
      public const string XLSXExtension = ".xlsx";

      public static class Errors
      {
         public static string CannotAccessFile(string fileFullPath)
         {
            return $"Cannot access the file '{fileFullPath}' because it is being used by another application. Please close this application and try again.";
         }
      }
   }
}