using System;

namespace OSPSuite.Utility
{
   public static class EnvironmentHelper
   {
      /// <summary>
      ///    Returns the current user name
      /// </summary>
      public static Func<string> UserName = () => Environment.UserName;

      /// <summary>
      ///    Returns the path to the User Application Data Folder (user specific)
      /// </summary>
      public static Func<string> UserApplicationDataFolder = () => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

      /// <summary>
      ///    Returns the path to the common Application Data Folder
      /// </summary>
      public static Func<string> ApplicationDataFolder = () => Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

      /// <summary>
      ///    Returns the current application name
      /// </summary>
      public static Func<string> ApplicationName = () => AppDomain.CurrentDomain.FriendlyName;
   }
}