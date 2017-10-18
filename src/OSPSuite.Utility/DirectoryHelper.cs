using System;
using System.IO;

namespace OSPSuite.Utility
{
   public static class DirectoryHelper
   {
      /// <summary>
      ///    Tests for existence of a directory with path given by string.
      /// </summary>
      public static Func<string, bool> DirectoryExists = directoryExists;

      /// <summary>
      ///    Creates a new directory at the path given by string
      /// </summary>
      public static Func<string, string> CreateDirectory = createDirectory;

      /// <summary>
      ///    Deletes a directory at the path given by string. Optionally recurses, removing all files and subfolders
      /// </summary>
      public static Action<string, bool> DeleteDirectory = deleteDirectory;

      /// <summary>
      ///    Returns all <see cref="FileInfo" /> for files defined in the folder (first parameter) matching the file filter
      ///    (second parameter)
      /// </summary>
      public static Func<string, string, FileInfo[]> AllFilesFromDirectory = allFilesFromDirectory;

      private static bool directoryExists(string path)
      {
         return Directory.Exists(path);
      }

      private static string createDirectory(string path)
      {
         Directory.CreateDirectory(path);
         return path;
      }

      private static void deleteDirectory(string path, bool recursive)
      {
         Directory.Delete(path, recursive);
      }

      private static FileInfo[] allFilesFromDirectory(string folder, string filter)
      {
         var directory = new DirectoryInfo(folder);
         return directory.GetFiles(filter);
      }
   }
}