using System;
using System.IO;

namespace OSPSuite.Utility.FileLocker
{
   public interface IFileLocker : IDisposable
   {
      /// <summary>
      ///    Tries to access the file <paramref name="fileToAccess" />.
      ///    This action also releases any existing lock to other files if available
      /// </summary>
      /// <param name="fileToAccess">File to access</param>
      /// <exception cref="FileNotFoundException">is thrown if the file does not exist</exception>
      /// <exception cref="FileAlreadyOpenException">is thrown if the file is already accessed by another user</exception>
      void AccessFile(string fileToAccess);

      /// <summary>
      ///    Release the lock if defined to the active file
      /// </summary>
      void ReleaseFile();

      /// <summary>
      ///    Returns true if the file <paramref name="fileName" /> is being accessed by a user otherwise false
      /// </summary>
      bool IsLocked(string fileName);

      /// <summary>
      ///    Returns the name of the user accessing the file. Returns an empty string if nobody is accessing the file
      /// </summary>
      /// <returns></returns>
      string UserAccessing(string fileLocked);
   }

   public class FileLocker : IFileLocker
   {
      private readonly ILockFilePathBuilder _filePathBuilder;
      private string _lockFilePath;
      private StreamWriter _swLockFile;
      public string Application { get; set; }

      public FileLocker() : this(new LockFilePathBuilder())
      {
      }

      public FileLocker(ILockFilePathBuilder filePathBuilder)
      {
         _filePathBuilder = filePathBuilder;
      }

      public void AccessFile(string fileToAccess)
      {
         checkFileStatus(fileToAccess);
         ReleaseFile();
         deleteExistingLockFileFor(fileToAccess);
         createLockFile(fileToAccess);
      }

      private void checkFileStatus(string fileToAccess)
      {
         if (!FileHelper.FileExists(fileToAccess))
            throw new FileNotFoundException(Constants.FileDoesNotExist(fileToAccess));
      }

      public void ReleaseFile()
      {
         if (_swLockFile == null) return;
         _swLockFile.Dispose();
         FileHelper.DeleteFile(_lockFilePath);
         _lockFilePath = string.Empty;
         _swLockFile = null;
      }

      public bool IsLocked(string fileName)
      {
         if (!FileHelper.FileExists(fileName))
            return false;

         try
         {
            deleteExistingLockFileFor(fileName);
            return false;
         }
         catch (Exception)
         {
            return true;
         }
      }

      public string UserAccessing(string fileLocked)
      {
         try
         {
            deleteExistingLockFileFor(fileLocked);
            return string.Empty;
         }
         catch (FileAlreadyOpenException e)
         {
            return e.UserAccessingTheFile;
         }
      }

      private void deleteExistingLockFileFor(string fileToAccess)
      {
         foreach (var fi in _filePathBuilder.LockFilesFor(fileToAccess))
         {
            try
            {
               FileHelper.DeleteFile(fi.FullName);
            }
            catch (Exception)
            {
               throw new FileAlreadyOpenException(fileToAccess, _filePathBuilder.UserIdFrom(fileToAccess, fi.FullName));
            }
         }
      }

      private void createLockFile(string fileToAccess)
      {
         _lockFilePath = _filePathBuilder.LockFilePathFor(fileToAccess);
         _swLockFile = new StreamWriter(_lockFilePath);
      }

      private bool _disposed;

      public void Dispose()
      {
         if (_disposed) return;

         ReleaseFile();
         GC.SuppressFinalize(this);
         _disposed = true;
      }

      ~FileLocker()
      {
         ReleaseFile();
      }
   }
}