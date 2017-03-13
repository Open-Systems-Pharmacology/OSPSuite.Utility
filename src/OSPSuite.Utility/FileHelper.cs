using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Resources;

namespace OSPSuite.Utility
{
   public static class FileHelper
   {
      /// <summary>
      ///    Characters not allowed in a windows file name
      /// </summary>
      public static readonly IEnumerable<string> IllegalCharacters = new List<string> {"\\", "/", ":", "*", "?", "<", ">", "|", "{", "}"};

      /// <summary>
      ///    Deletes the file whose path was given as parameter. If the files is readonly, the methods try to remove the readonly
      ///    attribute
      /// </summary>
      /// <param name="fileFullPath">Full path of the file to delete</param>
      public static void DeleteFile(string fileFullPath)
      {
         if (!FileExists(fileFullPath))
            return;

         var fileInfo = new FileInfo(fileFullPath);

         //file is readonly. Set attribute to normal
         if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            fileInfo.Attributes = fileInfo.Attributes ^ FileAttributes.ReadOnly;

         fileInfo.Delete();
      }

      /// <summary>
      ///    Takes the string given as parameter and replaces all illegal characters with '_'
      /// </summary>
      /// <param name="nameToCorrect">Name to correct. This is not the full file name of a file</param>
      /// <returns>The corrected name</returns>
      public static string RemoveIllegalCharactersFrom(string nameToCorrect)
      {
         if (string.IsNullOrEmpty(nameToCorrect))
            return string.Empty;

         IllegalCharacters.Each(c => { nameToCorrect = nameToCorrect.Replace(c, "_"); });
         return nameToCorrect;
      }

      /// <summary>
      ///    Writes the content given as the first parameter to the path given as the second parameter
      /// </summary>
      public static Action<string, string> WriteStringToFile = (outputString, filePath) =>
      {
         using (var streamWriter = new StreamWriter(filePath))
         {
            streamWriter.Write(outputString);
            streamWriter.Close();
         }
      };

      /// <summary>
      ///    Returns <c>true</c> if the file given as parameter exists otherwise <c>false</c>
      /// </summary>
      public static Func<string, bool> FileExists = fileFullPath =>
      {
         if (string.IsNullOrEmpty(fileFullPath))
            return false;

         var fileInfo = new FileInfo(fileFullPath);
         return fileInfo.Exists;
      };

      /// <summary>
      ///    Returns <c>true</c> if the file given as parameter is locked otherwise <c>false</c>
      /// </summary>
      public static Func<string, bool> IsFileLocked = fileFullPath =>
      {
         if (!FileExists(fileFullPath))
            return false;

         try
         {
            using (File.Open(fileFullPath, FileMode.Open))
            {
               //could open file
            }
            return false;
         }
         catch (IOException)
         {
            return true;
         }
      };

      /// <summary>
      ///    Copy the source file to the target file and overwrite if the target already exists
      /// </summary>
      public static Action<string, string> Copy = (source, target) => File.Copy(source, target, true);

      /// <summary>
      ///    Returns the filename from the file path given as parameter
      /// </summary>
      /// <example>
      ///    For the parameter C:\test\toto.txt, the function returns toto
      /// </example>
      /// <param name="fileFullPath">File full path with extension</param>
      /// <returns></returns>
      public static string FileNameFromFileFullPath(string fileFullPath)
      {
         if (string.IsNullOrEmpty(fileFullPath))
            return string.Empty;

         var fileInfo = new FileInfo(fileFullPath);
         return fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
      }

      /// <summary>
      ///    Returns the folder where the file with the given path resides
      /// </summary>
      public static string FolderFromFileFullPath(string fileFullPath)
      {
         if (string.IsNullOrEmpty(fileFullPath))
            return string.Empty;

         return Path.GetDirectoryName(fileFullPath);
      }

      /// <summary>
      ///    Generates a random file name under the temporary path
      /// </summary>
      public static Func<string> GenerateTemporaryFileName = () => Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

      /// <summary>
      ///    Checks if the file is locked or not. if the file is locked, a <see cref="OSPSuiteException" /> is raised and the
      ///    saveAction will not be performed.
      ///    Otherwise the save action will be performed
      /// </summary>
      public static Action<string, Action> TrySaveFile = (fileToSave, saveAction) =>
      {
         if (IsFileLocked(fileToSave))
            throw new OSPSuiteException(Constants.Errors.CannotAccessFile(fileToSave));

         saveAction();
      };

      /// <summary>
      ///    Tries to open the given file in an application registered for the file extension on the machine.
      ///    If the file cannot be open, or an exception occurs, the problem will simply be ignored.
      /// </summary>
      public static Action<string> TryOpenFile = fileToOpen =>
      {
         try
         {
            //start external application associated with the file
            Process.Start(fileToOpen);
         }
         catch (Exception)
         {
            //unable to open file with extensions! nothing to do
         }
      };

      /// <summary>
      ///    Returns true if the two files are equal otherwise false (equals when the hash are the same)
      /// </summary>
      /// <param name="fileFullPath1">Path of the first file</param>
      /// <param name="fileFullPath2">Path of the second file</param>
      public static bool AreFilesEqual(string fileFullPath1, string fileFullPath2)
      {
         if (!FileExists(fileFullPath1) || !FileExists(fileFullPath2)) return false;

         var hashAlgorithm = HashAlgorithm.Create();
         byte[] hashFile1;
         byte[] hashFile2;
         using (var fs = new FileStream(fileFullPath1, FileMode.Open))
         {
            hashFile1 = hashAlgorithm.ComputeHash(fs);
         }
         using (var fs = new FileStream(fileFullPath2, FileMode.Open))
         {
            hashFile2 = hashAlgorithm.ComputeHash(fs);
         }

         return BitConverter.ToString(hashFile1) == BitConverter.ToString(hashFile2);
      }

      /// <summary>
      ///    Returns true if the path is read only or do not have access to view the permissions otherwise false
      /// </summary>
      public static bool HasWriteAccessToFolder(string folderPath)
      {
         try
         {
            // Attempt to get a list of security permissions from the folder. 
            // This will raise an exception if the path is read only or do not have access to view the permissions. 
            Directory.GetAccessControl(folderPath);
            return true;
         }
         catch (UnauthorizedAccessException)
         {
            return false;
         }
      }

      /// <summary>
      ///    Returns true if the path is read only or do not have access to view the permissions otherwise false
      /// </summary>
      public static bool HasWriteAccessToFolder(DirectoryInfo directoryInfo)
      {
         return HasWriteAccessToFolder(directoryInfo.FullName);
      }

      /// <summary>
      ///    Creates a relative path from one file or folder (first parameter) relative to another file or folder (second
      ///    parameter).
      ///    Returns the original path if a relative path could not be created
      /// </summary>
      /// <example>
      ///    CreateRelativePath("C:\A\B\C\file1.txt", "C:\A\B\D\file2.txt") returns "..\C\file1.txt"
      /// </example>
      public static string CreateRelativePath(string originalPath, string relativeToPath)
      {
         if (string.IsNullOrEmpty(originalPath) || string.IsNullOrEmpty(relativeToPath))
            return originalPath;

         try
         {
            var originalUri = new Uri(appendDirectorySeparatorChar(originalPath));
            var relativeToUri = new Uri(appendDirectorySeparatorChar(relativeToPath));

            if (originalUri.Scheme != relativeToUri.Scheme)
               return originalPath;

            var relativeUri = relativeToUri.MakeRelativeUri(originalUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(relativeToUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
               relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
         }
         catch (Exception)
         {
            //we do not want any crash if for some reason the relative path cannot be computed. We will save as absolute path instead
            return originalPath;
         }
      }

      private static string appendDirectorySeparatorChar(string path)
      {
         // Append a slash only if the path is a directory and does not have a slash.
         if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            return path + Path.DirectorySeparatorChar;

         return path;
      }

      /// <summary>
      ///    Try to shorten a path to a given max length (e.g. replace the sub folder with ...)
      /// </summary>
      /// <remarks>
      ///    Implementation taken from http://www.codeproject.com/KB/cs/MruToolStripMenu.aspx
      /// </remarks>
      /// <param name="pathname"></param>
      /// <param name="maxLength"></param>
      /// <returns></returns>
      public static string ShortenPathName(string pathname, int maxLength)
      {
         if (pathname.Length <= maxLength)
            return pathname;

         string root = Path.GetPathRoot(pathname);
         if (root.Length > 3)
            root += Path.DirectorySeparatorChar;

         string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

         int filenameIndex = elements.GetLength(0) - 1;

         if (elements.GetLength(0) == 1) // pathname is just a root and filename
         {
            if (elements[0].Length > 5) // long enough to shorten
            {
               // if path is a UNC path, root may be rather long
               if (root.Length + 6 >= maxLength)
                  return root + elements[0].Substring(0, 3) + "...";

               return pathname.Substring(0, maxLength - 3) + "...";
            }
         }
         else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
         {
            root += "...\\";

            int len = elements[filenameIndex].Length;
            if (len < 6)
               return root + elements[filenameIndex];

            if ((root.Length + 6) >= maxLength)
            {
               len = 3;
            }
            else
            {
               len = maxLength - root.Length - 3;
            }
            return root + elements[filenameIndex].Substring(0, len) + "...";
         }
         else if (elements.GetLength(0) == 2)
         {
            return root + "...\\" + elements[1];
         }
         else
         {
            int len = 0;
            int begin = 0;

            for (int i = 0; i < filenameIndex; i++)
            {
               if (elements[i].Length > len)
               {
                  begin = i;
                  len = elements[i].Length;
               }
            }

            int totalLength = pathname.Length - len + 3;
            int end = begin + 1;

            while (totalLength > maxLength)
            {
               if (begin > 0)
                  totalLength -= elements[--begin].Length - 1;

               if (totalLength <= maxLength)
                  break;

               if (end < filenameIndex)
                  totalLength -= elements[++end].Length - 1;

               if (begin == 0 && end == filenameIndex)
                  break;
            }

            // assemble final string

            for (int i = 0; i < begin; i++)
            {
               root += elements[i] + '\\';
            }

            root += "...\\";

            for (int i = end; i < filenameIndex; i++)
            {
               root += elements[i] + '\\';
            }

            return root + elements[filenameIndex];
         }
         return pathname;
      }
   }
}