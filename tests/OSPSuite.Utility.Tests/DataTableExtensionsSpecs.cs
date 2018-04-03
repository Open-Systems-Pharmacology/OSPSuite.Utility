using System.Data;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_DataTableExtensions : StaticContextSpecification
   {
      protected DataTable _dataTable;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _dataTable = new DataTable("TEST");

         _dataTable.AddColumn<double>("col1");
         _dataTable.AddColumn<double>("col2");

         var row = _dataTable.NewRow();
         row["col1"] = 1.5;
         row["col2"] = 2.8;
         _dataTable.Rows.Add(row);
      }
   }

   public class When_exporting_a_datatable_to_csv_using_the_default_settings : concern_for_DataTableExtensions
   {
      private string _fileName;
      private string[] _allLines;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _fileName = FileHelper.GenerateTemporaryFileName();
         _dataTable.ExportToCSV(_fileName);
         _allLines = File.ReadAllLines(_fileName);
      }

      [Observation]
      public void should_have_exported_one_row_for_the_header_and_one_for_each_row()
      {
         _allLines.Length.ShouldBeEqualTo(_dataTable.Rows.Count + 1);
      }

      [Observation]
      public void should_export_the_datatable_to_a_file_using_the_comma_separator()
      {
         var header = _allLines[0].Split(',');
         header.Length.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_added_quotes_between_parameter_names()
      {
         var header = _allLines[0].Split(',');
         header[0].ShouldBeEqualTo("\"col1\"");
         header[1].ShouldBeEqualTo("\"col2\"");
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.DeleteFile(_fileName);
      }
   }

   public class When_exporting_a_datatable_to_csv_using_another_separator_and_without_quote_encapsulation_for_header : concern_for_DataTableExtensions
   {
      private string _fileName;
      private string[] _allLines;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _fileName = FileHelper.GenerateTemporaryFileName();
         _dataTable.ExportToCSV(_fileName, delimiter: "-", encloseHeaderInQuotes: false);
         _allLines = File.ReadAllLines(_fileName);
      }

      [Observation]
      public void should_have_exported_one_row_for_the_header_and_one_for_each_row()
      {
         _allLines.Length.ShouldBeEqualTo(_dataTable.Rows.Count + 1);
      }

      [Observation]
      public void should_export_the_datatable_to_a_file_using_the_comma_separator()
      {
         var header = _allLines[0].Split('-');
         header.Length.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_added_quotes_between_parameter_names()
      {
         var header = _allLines[0].Split('-');
         header[0].ShouldBeEqualTo("col1");
         header[1].ShouldBeEqualTo("col2");
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.DeleteFile(_fileName);
      }
   }

   public class When_exporting_a_datatable_to_csv_with_comments : concern_for_DataTableExtensions
   {
      private string _fileName;
      private string[] _allLines;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _fileName = FileHelper.GenerateTemporaryFileName();
         _dataTable.ExportToCSV(_fileName, comments: new[] {"Hello"});
         _allLines = File.ReadAllLines(_fileName);
      }

      [Observation]
      public void should_have_exported_one_row_for_the_header_and_one_for_each_row_and_one_for_each_comment()
      {
         _allLines.Length.ShouldBeEqualTo(_dataTable.Rows.Count + 1 + 1);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.DeleteFile(_fileName);
      }
   }
}