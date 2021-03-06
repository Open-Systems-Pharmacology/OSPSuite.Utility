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
      private string _folder5;

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
         _folder5 = @"C:\A\B\C\D";
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
         FileHelper.CreateRelativePath(_file1, _file2).ShouldBeEqualTo(@"..\..\C\file1.txt");
         FileHelper.CreateRelativePath(_file1, _file3).ShouldBeEqualTo(@"..\..\C\file1.txt");
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_file_and_another_file_on_the_same_drive_using_unix_separator()
      {
         FileHelper.CreateRelativePath(_file1, _file2, true).ShouldBeEqualTo("../../C/file1.txt");
         FileHelper.CreateRelativePath(_file1, _file3, true).ShouldBeEqualTo("../../C/file1.txt");
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_folder_and_a_folder()
      {
         FileHelper.CreateRelativePath("c:/A/B/C/D", "c:/A/B").ShouldBeEqualTo(@"C\D\");
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_folder_and_a_folder_ending_with_a_folder_char()
      {
         FileHelper.CreateRelativePath("c:/A/B/C/D", "c:/A/B/").ShouldBeEqualTo(@"C\D\");
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_folder_ending_with_a_folder_char_and_a_folder()
      {
         FileHelper.CreateRelativePath("c:/A/B/C/D/", "c:/A/B").ShouldBeEqualTo(@"C\D\");
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_folder_ending_with_a_folder_char_and_a_folder_ending_with_a_folder_char()
      {
         FileHelper.CreateRelativePath("c:/A/B/C/D/", "c:/A/B/").ShouldBeEqualTo(@"C\D\");
      }


      [Observation]
      public void should_return_the_expected_relative_path_between_a_folder_and_a_file()
      {
         FileHelper.CreateRelativePath(_folder5, _file1).ShouldBeEqualTo(@"..\D\");
      }

      [Observation]
      public void should_return_the_expected_relative_path_between_a_folder_and_a_folder_when_discarding_the_directory_separator()
      {
         FileHelper.CreateRelativePath("c:/A/B/C/D", "c:/A/B", appendDirectorySeparatorAtEndOfRelativePath: false).ShouldBeEqualTo(@"C\D");
      }
   }

   public class When_copying_the_content_of_one_directory_to_another_with_the_root_directory_copied : concern_for_FileHelper
   {
      private string _sourceDirectory;
      private string _targetDirectory;
      private readonly string _file1Name = "file1.txt";
      private readonly string _file2Name = "file2.txt";
      private string _srcFile1;
      private string _srcFile2;
      private string _targetSubDirectory;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sourceDirectory = DirectoryHelper.CreateDirectory("C:/temp/source");
         _targetDirectory = DirectoryHelper.CreateDirectory("C:/temp/target");
         _targetSubDirectory = Path.Combine(_targetDirectory, "source");

         _srcFile1 = Path.Combine(_sourceDirectory, _file1Name);
         _srcFile2 = Path.Combine(_sourceDirectory, _file2Name);
         File.WriteAllText(Path.Combine(_sourceDirectory, _srcFile1), "FILE1");
         File.WriteAllText(Path.Combine(_sourceDirectory, _srcFile2), "FILE2");


         FileHelper.CopyDirectory(_sourceDirectory, _targetDirectory);
      }

      [Observation]
      public void should_create_a_sub_folder_under_the_target_directory()
      {
         DirectoryHelper.DirectoryExists(_targetSubDirectory).ShouldBeTrue();
      }

      [Observation]
      public void should_have_copied_the_file_from_source_in_target()
      {
         FileHelper.FileExists(Path.Combine(_targetSubDirectory, _file1Name)).ShouldBeTrue();
         FileHelper.FileExists(Path.Combine(_targetSubDirectory, _file2Name)).ShouldBeTrue();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         DirectoryHelper.DeleteDirectory(_sourceDirectory, true);
         DirectoryHelper.DeleteDirectory(_targetDirectory, true);
      }
   }

   public class When_copying_the_content_of_one_directory_to_another_without_creating_a_sub_structure : concern_for_FileHelper
   {
      private string _sourceDirectory;
      private string _targetDirectory;
      private readonly string _file1Name = "file1.txt";
      private readonly string _file2Name = "file2.txt";
      private string _srcFile1;
      private string _srcFile2;
      private string _targetSubDirectory;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sourceDirectory = DirectoryHelper.CreateDirectory("C:/temp/source");
         _targetDirectory = DirectoryHelper.CreateDirectory("C:/temp/target");
         _targetSubDirectory = Path.Combine(_targetDirectory, "source");

         _srcFile1 = Path.Combine(_sourceDirectory, _file1Name);
         _srcFile2 = Path.Combine(_sourceDirectory, _file2Name);
         File.WriteAllText(Path.Combine(_sourceDirectory, _srcFile1), "FILE1");
         File.WriteAllText(Path.Combine(_sourceDirectory, _srcFile2), "FILE2");


         FileHelper.CopyDirectory(_sourceDirectory, _targetDirectory, createRootDirectory: false);
      }

      [Observation]
      public void should_not_create_a_sub_folder_under_the_target_directory()
      {
         DirectoryHelper.DirectoryExists(_targetSubDirectory).ShouldBeFalse();
      }

      [Observation]
      public void should_have_copied_the_file_from_source_in_target()
      {
         FileHelper.FileExists(Path.Combine(_targetDirectory, _file1Name)).ShouldBeTrue();
         FileHelper.FileExists(Path.Combine(_targetDirectory, _file2Name)).ShouldBeTrue();
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         DirectoryHelper.DeleteDirectory(_sourceDirectory, true);
         DirectoryHelper.DeleteDirectory(_targetDirectory, true);
      }
   }

   public class When_copying_the_content_of_one_directory_to_another_without_allowing_file_overwrite : concern_for_FileHelper
   {
      private string _sourceDirectory;
      private string _targetDirectory;
      private readonly string _file1Name = "file1.txt";
      private readonly string _file2Name = "file2.txt";
      private string _srcFile1;
      private string _srcFile2;
      private string _targetFile1;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _sourceDirectory = DirectoryHelper.CreateDirectory("C:/temp/source");
         _targetDirectory = DirectoryHelper.CreateDirectory("C:/temp/target");

         _srcFile1 = Path.Combine(_sourceDirectory, _file1Name);
         _srcFile2 = Path.Combine(_sourceDirectory, _file2Name);
         _targetFile1 = Path.Combine(_sourceDirectory, _file1Name);
         File.WriteAllText(Path.Combine(_sourceDirectory, _srcFile1), "FILE1_SOURCE");
         File.WriteAllText(Path.Combine(_sourceDirectory, _srcFile2), "FILE2_SOURCE");
         File.WriteAllText(Path.Combine(_targetDirectory, _targetFile1), "FILE1_TARGET");

         FileHelper.CopyDirectory(_sourceDirectory, _targetDirectory, overwrite: false, createRootDirectory: false);
      }

      [Observation]
      public void should_have_copied_the_non_existing_file_from_source_in_target()
      {
         FileHelper.FileExists(Path.Combine(_targetDirectory, _file2Name)).ShouldBeTrue();
      }

      [Observation]
      public void should_have_kept_the_existing_file_with_same_name_in_target()
      {
         File.ReadAllText(Path.Combine(_targetDirectory, _targetFile1)).ShouldBeEqualTo("FILE1_TARGET");
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         DirectoryHelper.DeleteDirectory(_sourceDirectory, true);
         DirectoryHelper.DeleteDirectory(_targetDirectory, true);
      }
   }
}