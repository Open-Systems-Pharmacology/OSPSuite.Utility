using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.FileLocker;

namespace OSPSuite.Utility.Tests
{
   public abstract class FileLockerSpecsBase : ContextSpecification<IFileLocker>
   {
      protected string _fileThatDoesExist = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bbbbb.txt");
      protected string _fileThatDoesNotExist = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "aaaaa");
      protected ILockFilePathBuilder _lockFilePathBuilder;
      protected string _nameOfFileThatDoesExist = "bbbbb";
      protected StreamWriter _swOpened;

      protected void DeleteFile(string file)
      {
         FileHelper.DeleteFile(file);
      }

      protected void CreateFile(string file)
      {
         using (new StreamWriter(file))
         {
         }
      }

      protected void CreateOpenedAccessFile(string fileName)
      {
         _swOpened = new StreamWriter(fileName);
      }

      protected override void Context()
      {
         _lockFilePathBuilder = A.Fake<ILockFilePathBuilder>();
         sut = new FileLocker.FileLocker(_lockFilePathBuilder);
         CreateFile(_fileThatDoesExist);
      }

      public override void Cleanup()
      {
         DeleteFile(_fileThatDoesExist);
         if (_swOpened != null)
            _swOpened.Dispose();
      }
   }

   public class When_accessing_a_file_that_does_not_exist : FileLockerSpecsBase
   {
      [Observation]
      public void should_throw_a_file_not_found_exception()
      {
         The.Action(() => sut.AccessFile(_fileThatDoesNotExist)).ShouldThrowAn<FileNotFoundException>();
      }
   }

   public class When_accessing_a_file_that_is_already_open : FileLockerSpecsBase
   {
      private string _lockFilePath;

      protected override void Context()
      {
         base.Context();
         _lockFilePath = $"{_fileThatDoesExist}.UserID.App.lock";
         CreateOpenedAccessFile(_lockFilePath);
         var fileInfos = new List<FileInfo> {new FileInfo(_lockFilePath)};
         A.CallTo(() => _lockFilePathBuilder.LockFilePathFor(_fileThatDoesExist)).Returns(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesExist)).Returns(fileInfos);
      }

      [Observation]
      public void should_throw_a_file_already_open_exception()
      {
         The.Action(() => sut.AccessFile(_fileThatDoesExist)).ShouldThrowAn<FileAlreadyOpenException>();
      }
   }

   public class When_accessing_an_existing_file_that_is_not_already_open : FileLockerSpecsBase
   {
      private string _lockFilePath;

      protected override void Context()
      {
         base.Context();
         _lockFilePath = $"{_fileThatDoesExist}.TOTO.App.lock";
         var fileInfos = new List<FileInfo> {new FileInfo(_lockFilePath)};
         A.CallTo(() => _lockFilePathBuilder.LockFilePathFor(_fileThatDoesExist)).Returns(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesExist)).Returns(fileInfos);
      }

      [Observation]
      public void should_hold_a_reference_to_the_created_lock_so_that_file_cannot_be_deleted()
      {
         The.Action(() => DeleteFile(_lockFilePath)).ShouldThrowAn<IOException>();
      }

      protected override void Because()
      {
         sut.AccessFile(_fileThatDoesExist);
      }

      public override void Cleanup()
      {
         base.Cleanup();
         sut.ReleaseFile();
      }
   }

   public class When_releasing_a_lock_for_a_file : FileLockerSpecsBase
   {
      private string _lockFilePath;

      protected override void Context()
      {
         base.Context();
         _lockFilePath = $"{_fileThatDoesExist}.TOTO.App.lock";
         var fileInfos = new List<FileInfo> {new FileInfo(_lockFilePath)};
         A.CallTo(() => _lockFilePathBuilder.LockFilePathFor(_fileThatDoesExist)).Returns(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesExist)).Returns(fileInfos);
      }

      [Observation]
      public void the_lock_file_should_be_deleted()
      {
         FileHelper.FileExists(_lockFilePath).ShouldBeFalse();
      }

      protected override void Because()
      {
         sut.AccessFile(_fileThatDoesExist);
         sut.ReleaseFile();
      }
   }

   public class When_calling_the_dispose_method : FileLockerSpecsBase
   {
      private string _lockFilePath;

      protected override void Because()
      {
         sut.AccessFile(_fileThatDoesExist);
         sut.Dispose();
      }

      protected override void Context()
      {
         base.Context();
         _lockFilePath = $"{_fileThatDoesExist}.TOTO.App.lock";
         var fileInfos = new List<FileInfo> {new FileInfo(_lockFilePath)};
         A.CallTo(() => _lockFilePathBuilder.LockFilePathFor(_fileThatDoesExist)).Returns(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesExist)).Returns(fileInfos);
      }

      [Observation]
      public void the_lock_file_should_be_deleted()
      {
         FileHelper.FileExists(_lockFilePath).ShouldBeFalse();
      }
   }

   public class When_asked_if_a_file_that_is_locked_is_locked : FileLockerSpecsBase
   {
      private string _lockFilePath;
      private bool _result;

      protected override void Because()
      {
         _result = sut.IsLocked(_fileThatDoesExist);
      }

      protected override void Context()
      {
         base.Context();
         _lockFilePath = $"{_fileThatDoesExist}.UserID.App.lock";
         var fileInfos = new List<FileInfo> {new FileInfo(_lockFilePath)};
         CreateOpenedAccessFile(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilePathFor(_fileThatDoesExist)).Returns(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesExist)).Returns(fileInfos);
      }

      [Observation]
      public void should_return_true()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_asked_if_a_file_that_does_not_exist_is_locked : FileLockerSpecsBase
   {
      private bool _result;

      protected override void Because()
      {
         _result = sut.IsLocked(_fileThatDoesNotExist);
      }

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesNotExist)).Returns(new List<FileInfo>());
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_asked_if_a_file_that_is_not_locked_is_locked : FileLockerSpecsBase
   {
      private bool _result;

      protected override void Because()
      {
         _result = sut.IsLocked(_fileThatDoesExist);
      }

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesExist)).Returns(new List<FileInfo>());
      }

      [Observation]
      public void should_return_false()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_asked_for_the_user_accessing_a_file_that_is_already_open : FileLockerSpecsBase
   {
      private string _lockFilePath;
      private string _result;

      protected override void Because()
      {
         _result = sut.UserAccessing(_fileThatDoesExist);
      }

      protected override void Context()
      {
         base.Context();
         _lockFilePath = $"{_fileThatDoesExist}.UserID.App.lock";
         var fileInfos = new List<FileInfo> {new FileInfo(_lockFilePath)};
         CreateOpenedAccessFile(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilePathFor(_fileThatDoesExist)).Returns(_lockFilePath);
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesExist)).Returns(fileInfos);
         A.CallTo(() => _lockFilePathBuilder.UserIdFrom(_fileThatDoesExist, _lockFilePath)).Returns("UserID");
      }

      [Observation]
      public void should_return_the_user_id_of_the_user()
      {
         _result.ShouldBeEqualTo("UserID");
      }
   }

   public class When_asked_for_the_user_accessing_a_file_that_is_not_open : FileLockerSpecsBase
   {
      private string _result;

      protected override void Context()
      {
         base.Context();
         var fileInfos = new List<FileInfo>();
         A.CallTo(() => _lockFilePathBuilder.LockFilesFor(_fileThatDoesNotExist)).Returns(fileInfos);
      }

      [Observation]
      public void should_return_an_empty_string()
      {
         _result.ShouldBeEqualTo(string.Empty);
      }

      protected override void Because()
      {
         _result = sut.UserAccessing(_fileThatDoesNotExist);
      }
   }

   public class When_opening_a_file_that_was_just_released_by_another_file_locker : FileLockerSpecsBase
   {
      private IFileLocker _fileLocker1;
      private IFileLocker _fileLocker2;

      protected override void Because()
      {
      }

      protected override void Context()
      {
         base.Context();
         _fileLocker1 = new FileLocker.FileLocker();
         _fileLocker2 = new FileLocker.FileLocker();
      }

      [Observation]
      public void Should_be_able_to_lock_the_file()
      {
         _fileLocker1.AccessFile(_fileThatDoesExist);
         bool exceptionWasThrown = false;
         try
         {
            _fileLocker2.AccessFile(_fileThatDoesExist);
         }
         catch (Exception)
         {
            exceptionWasThrown = true;
         }
         exceptionWasThrown.ShouldBeTrue();
         _fileLocker1.ReleaseFile();
         _fileLocker2.AccessFile(_fileThatDoesExist);
      }

      public override void Cleanup()
      {
         _fileLocker1.ReleaseFile();
         _fileLocker2.ReleaseFile();
      }
   }
}