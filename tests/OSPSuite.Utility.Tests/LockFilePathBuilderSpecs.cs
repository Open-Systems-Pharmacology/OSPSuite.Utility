using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.FileLocker;

namespace OSPSuite.Utility.Tests
{
   public class When_told_to_create_a_lock_for_a_file : ContextSpecification<ILockFilePathBuilder>
   {
      private string _file;
      private string _userId;

      [Observation]
      public void should_return_the_path_of_the_lock_file_corresponding_to_the_given_format()
      {
         string format = "${0}.{1}.{2}.lock";
         sut = new LockFilePathBuilder(format);
         string expectedResult = $@"c:\test\$toto.{_userId}.MyApp.lock";
         expectedResult.ShouldBeEqualTo(sut.LockFilePathFor(_file));

         format = "${0}_{1}_{2}.toto";
         sut = new LockFilePathBuilder(format);
         expectedResult = $@"c:\test\$toto_{_userId}_MyApp.toto";
         expectedResult.ShouldBeEqualTo(sut.LockFilePathFor(_file));

         format = "${0}_{1}_{2}";
         sut = new LockFilePathBuilder(format);
         expectedResult = $@"c:\test\$toto_{_userId}_MyApp";
         expectedResult.ShouldBeEqualTo(sut.LockFilePathFor(_file));
      }

      protected override void Context()
      {
         _file = @"c:\test\toto.txt";
         _userId = EnvironmentHelper.UserName();
         EnvironmentHelper.ApplicationName = () => "MyApp";
      }
   }

   public class When_told_to_create_a_pattern_to_search_from : ContextSpecification<ILockFilePathBuilder>
   {
      private string _file;
      private string _format;
      private string _result;

      [Observation]
      public void should_return_a_pattern_that_matches_the_format()
      {
         _result.ShouldBeEqualTo(@"$toto.*.MyApp.lock");
      }

      protected override void Because()
      {
         _result = sut.SearchPattern(_file);
      }

      protected override void Context()
      {
         _file = @"c:\test\toto.txt";
         _format = "${0}.{1}.{2}.lock";
         EnvironmentHelper.ApplicationName = () => "MyApp";
         sut = new LockFilePathBuilder(_format);
      }
   }

   public class When_told_to_retrieve_the_user_id_from_a_lock_file : ContextSpecification<ILockFilePathBuilder>
   {
      [Observation]
      public void should_return_the_id_of_the_user_with_the_lock_on_the_file()
      {
         string file = @"c:\test\toto.txt";
         string lockFile = @"c:\test\$toto.TOTO.MyApp.lock";
         string format = "${0}.{1}.{2}.lock";
         EnvironmentHelper.ApplicationName = () => "MyApp";
         sut = new LockFilePathBuilder(format);
         Assert.AreEqual(sut.UserIdFrom(file, lockFile), "TOTO");
      }
   }

   public class When_told_to_retrieve_the_list_of_available_lock_for_an_existing_file : ContextSpecification<ILockFilePathBuilder>
   {
      private string _fileToAccess;
      private string _format;
      private string _lock1;
      private string _lock2;
      private string _lock3;
      private IEnumerable<FileInfo> _result;

      protected override void Context()
      {
         _format = "${0}.{1}.{2}.lock";
         EnvironmentHelper.ApplicationName = () => "MyApp";

         sut = new LockFilePathBuilder(_format);
         _fileToAccess = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "toto.txt");
         _lock1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "$toto.USER1.MyApp.lock");
         _lock2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "$toto.USER2.MyApp.lock");
         _lock3 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "$toto.USER2.MyApp2.lock");
         createFile(_lock1);
         createFile(_lock2);
         createFile(_lock3);
         createFile(_fileToAccess);
      }

      protected override void Because()
      {
         _result = sut.LockFilesFor(_fileToAccess);
      }

      [Observation]
      public void Should_return_all_the_files_matching_the_search_pattern()
      {
         IList<FileInfo> list = new List<FileInfo>(_result);
         Assert.AreEqual(list.Count, 2);
      }

      private void deleteFile(string file)
      {
         FileHelper.DeleteFile(file);
      }

      private void createFile(string file)
      {
         using (new StreamWriter(file))
         {
         }
      }

      public override void Cleanup()
      {
         base.Cleanup();
         deleteFile(_lock1);
         deleteFile(_lock2);
         deleteFile(_lock3);
         deleteFile(_fileToAccess);
      }
   }

   public class When_told_to_retrieve_the_list_of_available_lock_for_a_file_that_does_not_exist : ContextSpecification<ILockFilePathBuilder>
   {
      private string _fileThatDoesNotExist;

      [Observation]
      public void Should_return_an_empty_collection()
      {
         IList<FileInfo> list = new List<FileInfo>(sut.LockFilesFor(_fileThatDoesNotExist));
         Assert.AreEqual(list.Count, 0);

         list = new List<FileInfo>(sut.LockFilesFor(string.Empty));
         Assert.AreEqual(list.Count, 0);
      }

      protected override void Because()
      {
      }

      protected override void Context()
      {
         sut = new LockFilePathBuilder();
         _fileThatDoesNotExist = "c:\\asdasd.txt";
      }
   }
}