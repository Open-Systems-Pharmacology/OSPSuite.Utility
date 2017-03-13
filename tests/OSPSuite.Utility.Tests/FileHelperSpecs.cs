using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_FileHelper : StaticContextSpecification
   {
      protected string _existingFile;
      protected string _anotherExistingFile;
      protected string _oneFileThatDoesNotExist;
      protected string _copyOfExistingFile;
      protected string _anotherFileWithTheSameContent;

      protected override void Context()
      {
         _existingFile = FileHelper.GenerateTemporaryFileName();
         _copyOfExistingFile = FileHelper.GenerateTemporaryFileName();
         _anotherExistingFile = FileHelper.GenerateTemporaryFileName();
         _anotherFileWithTheSameContent = FileHelper.GenerateTemporaryFileName();
         _oneFileThatDoesNotExist = @"c:\treaara.tasdasd.txt";

         using (var sw = new StreamWriter(_existingFile))
         {
            sw.WriteLine("tralalal");
            sw.WriteLine("trilili");
         }

         using (var sw = new StreamWriter(_anotherExistingFile))
         {
            sw.WriteLine("qweqwe");
            sw.WriteLine("asdasd");
         }

         using (var sw = new StreamWriter(_anotherFileWithTheSameContent))
         {
            sw.WriteLine("tralalal");
            sw.WriteLine("trilili");
         }

         File.Copy(_existingFile, _copyOfExistingFile);
      }

      public override void Cleanup()
      {
         FileHelper.DeleteFile(_existingFile);
         FileHelper.DeleteFile(_copyOfExistingFile);
         FileHelper.DeleteFile(_anotherExistingFile);
         FileHelper.DeleteFile(_anotherFileWithTheSameContent);
      }
   }

   public class When_checking_if_two_files_are_identical : concern_for_FileHelper
   {
      [Observation]
      public void should_return_true_if_the_files_have_the_same_content()
      {
         FileHelper.AreFilesEqual(_existingFile, _existingFile).ShouldBeTrue();
         FileHelper.AreFilesEqual(_existingFile, _copyOfExistingFile).ShouldBeTrue();
         FileHelper.AreFilesEqual(_existingFile, _anotherFileWithTheSameContent).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_files_does_not_have_the_same_content()
      {
         FileHelper.AreFilesEqual(_existingFile, _anotherExistingFile).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_one_of_the_provided_file_name_does_not_exist()
      {
         FileHelper.AreFilesEqual(_existingFile, _oneFileThatDoesNotExist).ShouldBeFalse();
      }
   }

   public class When_checking_if_a_file_exists : concern_for_FileHelper
   {
      [Observation]
      public void should_return_true_if_the_file_does_exist()
      {
         FileHelper.FileExists(_existingFile).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_file_does_not_exist()
      {
         FileHelper.FileExists(_oneFileThatDoesNotExist).ShouldBeFalse();
      }
   }

   public class When_correcting_a_name : concern_for_FileHelper
   {
      [Observation]
      public void should_return_the_same_name_if_the_name_was_valid()
      {
         FileHelper.RemoveIllegalCharactersFrom("abcd").ShouldBeEqualTo("abcd");
      }

      [Observation]
      public void should_return_a_corrected_nane_name_if_the_name_was_invalid()
      {
         FileHelper.RemoveIllegalCharactersFrom("ab:cd>").ShouldBeEqualTo("ab_cd_");
      }

      [Observation]
      public void should_not_change_the_reference_of_a_string_passed_as_parameter()
      {
         string s = "ab:cd>";
         FileHelper.RemoveIllegalCharactersFrom(s);
         s.ShouldBeEqualTo("ab:cd>");
      }
   }

   public class When_checking_if_an_existing_file_that_is_not_used_by_any_process_is_locked : concern_for_FileHelper
   {
      [Observation]
      public void should_return_false()
      {
         FileHelper.IsFileLocked(_existingFile).ShouldBeFalse();
      }
   }

   public class When_checking_if_a_file_that_does_not_exist_is_locked : concern_for_FileHelper
   {
      [Observation]
      public void should_return_false()
      {
         FileHelper.IsFileLocked(_oneFileThatDoesNotExist).ShouldBeFalse();
      }
   }

   public class When_checking_if_a_file_that_does_exist_but_is_open_is_locked : concern_for_FileHelper
   {
      private FileStream _stream;

      protected override void Context()
      {
         base.Context();
         _stream = File.Open(_existingFile, FileMode.Open);
      }

      [Observation]
      public void should_return_true()
      {
         FileHelper.IsFileLocked(_existingFile).ShouldBeTrue();
      }

      public override void Cleanup()
      {
         _stream.Dispose();
         base.Cleanup();
      }
   }

   public class When_trying_to_save_a_file_that_is_not_locked : concern_for_FileHelper
   {
      private bool _savedCalled;

      protected override void Because()
      {
         FileHelper.TrySaveFile(_existingFile, () => _savedCalled = true);
      }

      [Observation]
      public void should_execute_the_save_action()
      {
         _savedCalled.ShouldBeTrue();
      }
   }

   public class When_trying_to_save_a_file_that_is_locked : concern_for_FileHelper
   {
      private FileStream _stream;

      protected override void Context()
      {
         base.Context();
         _stream = File.Open(_existingFile, FileMode.Open);
      }

      [Observation]
      public void should_execute_the_save_action()
      {
         The.Action(() => FileHelper.TrySaveFile(_existingFile, () => { })).ShouldThrowAn<OSPSuiteException>();
      }

      public override void Cleanup()
      {
         _stream.Dispose();
         base.Cleanup();
      }
   }

   public class When_creating_a_relative_path : StaticContextSpecification
   {
      private string _file1;
      private string _file2;
      private string _file3;
      private string _folder1;
      private string _folder2;
      private string _folder3;
      private string _folder4;

      protected override void Context()
      {
         base.Context();
         _file1 = @"C:\A\B\C\file1.txt";
         _file2 = @"C:\A\B\D\file2.txt";
         _file3 = @"C:/A/B/D/file2.txt";
         _folder1 = @"C:\A\B";
         _folder2 = @"C:\A\B\";
         _folder3 = @"D:\A\B\";
         _folder4 = @"Dblalalbla\B\";
      }

      [Observation]
      public void should_return_the_original_path_if_the_relative_path_is_not_defined()
      {
         FileHelper.CreateRelativePath(_file1, null).ShouldBeEqualTo(_file1);
      }

      [Observation]
      public void should_return_the_original_path_if_the_given_relative_path_is_garbage()
      {
         FileHelper.CreateRelativePath(_file1, _folder4).ShouldBeEqualTo(_file1);
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_file_and_a_folder_in_the_same_drive()
      {
         FileHelper.CreateRelativePath(_file1, _folder1).ShouldBeEqualTo(@"C\file1.txt");
         FileHelper.CreateRelativePath(_file1, _folder2).ShouldBeEqualTo(@"C\file1.txt");
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_file_and_a_folder_in_another_drive()
      {
         FileHelper.CreateRelativePath(_file1, _folder3).ShouldBeEqualTo(_file1);
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_file_and_another_file_on_the_same_drive()
      {
         FileHelper.CreateRelativePath(_file1, _file2).ShouldBeEqualTo(@"..\C\file1.txt");
         FileHelper.CreateRelativePath(_file1, _file3).ShouldBeEqualTo(@"..\C\file1.txt");
      }
   }
}