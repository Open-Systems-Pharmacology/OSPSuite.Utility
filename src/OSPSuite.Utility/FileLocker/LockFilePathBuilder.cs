using System;
using System.Collections.Generic;
using System.IO;

namespace OSPSuite.Utility.FileLocker
{
   public interface ILockFilePathBuilder
   {
      string LockFilePathFor(string fileToAccess);
      string UserIdFrom(string fileToAcess, string lockFileFullPath);
      IEnumerable<FileInfo> LockFilesFor(string fileFullPath);
      string SearchPattern(string fileName);
   }

   public class LockFilePathBuilder : ILockFilePathBuilder
   {
      private const string DEFAULT_LOCK_FORMAT = "${0}.{1}.{2}.lock";
      private readonly string _lockFormat;

      public LockFilePathBuilder(string lockFormat)
      {
         _lockFormat = lockFormat;
      }

      public LockFilePathBuilder() : this(DEFAULT_LOCK_FORMAT)
      {
      }

      public string LockFilePathFor(string fileToAccess)
      {
         var fileInfo = new FileInfo(fileToAccess);

         return Path.Combine(fileInfo.DirectoryName, lockFileName(fileToAccess, EnvironmentHelper.UserName()));
      }

      public string SearchPattern(string fileName)
      {
         return lockFileName(fileName, "*");
      }

      private string lockFileName(string fileName, string userId)
      {
         var fileInfo = new FileInfo(fileName);
         return string.Format(_lockFormat, fileInfo.Name.Replace(fileInfo.Extension, string.Empty), userId, EnvironmentHelper.ApplicationName());
      }

      public string UserIdFrom(string fileToAcess, string lockFileFullPath)
      {
         string userId = EnvironmentHelper.UserName();
         string lockFile = LockFilePathFor(fileToAcess);
         int startIndex = lockFile.IndexOf(userId, StringComparison.Ordinal);
         string endOfLockFile = lockFile.Substring(startIndex + userId.Length);
         int endIndex = lockFile.Length;
         if (!string.IsNullOrEmpty(endOfLockFile))
            endIndex = lockFileFullPath.IndexOf(endOfLockFile, StringComparison.Ordinal);

         return lockFileFullPath.Substring(lockFile.IndexOf(userId, StringComparison.Ordinal), endIndex - startIndex);
      }

      public IEnumerable<FileInfo> LockFilesFor(string fileFullPath)
      {
         if (!FileHelper.FileExists(fileFullPath))
            yield break;

         var fileInfo = new FileInfo(fileFullPath);
         foreach (var fi in fileInfo.Directory.GetFiles(SearchPattern(fileFullPath)))
         {
            yield return fi;
         }
      }
   }
}