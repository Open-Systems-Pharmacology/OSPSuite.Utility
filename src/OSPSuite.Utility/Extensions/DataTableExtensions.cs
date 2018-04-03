using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace OSPSuite.Utility.Extensions
{
   public static class DataTableExtensions
   {
      public static DataColumn AddColumn(this DataTable dataTable, string columnName)
      {
         return AddColumn<string>(dataTable, columnName);
      }

      public static DataColumn AddColumn(this DataTable dataTable, string columnName, Type type)
      {
         return AddColumn(dataTable, new DataColumn(columnName, type));
      }

      public static DataColumn AddColumn(this DataTable dataTable, DataColumn dataColumn)
      {
         dataTable.Columns.Add(dataColumn);
         return dataColumn;
      }

      public static DataColumn AddColumn<T>(this DataTable dataTable, string columnName)
      {
         return AddColumn(dataTable, columnName, typeof(T));
      }

      public static void AddColumns<T>(this DataTable dataTable, params string[] columnNames)
      {
         AddColumns<T>(dataTable, columnNames as IEnumerable<string>);
      }

      public static void AddColumns<T>(this DataTable dataTable, IEnumerable<string> columnNames)
      {
         columnNames.Each(name => dataTable.AddColumn<T>(name));
      }

      public static IReadOnlyList<T> AllValuesInColumn<T>(this DataTable dataTable, string columnName)
      {
         return dataTable.Rows.Cast<DataRow>().Select(row => row[columnName].ConvertedTo<T>()).ToList();
      }

      public static IReadOnlyList<T> AllValuesInColumn<T>(this DataView dataView, string columnName)
      {
         return dataView.Cast<DataRowView>().Select(row => row[columnName].ConvertedTo<T>()).ToList();
      }

      public static DataTable SubTable(this DataTable dataTable, IReadOnlyCollection<string> columnNames, bool distinctValues = true)
      {
         if (columnNames.Count == 0)
            return new DataTable();

         return dataTable.DefaultView.ToTable(distinctValues, columnNames.ToArray());
      }

      /// <summary>
      ///    Exports the datable to csv using the <paramref name="delimiter" /> given as parameter
      /// </summary>
      /// <param name="dataTable">DataTable to export</param>
      /// <param name="fileName">File where the <paramref name="dataTable" /> will be saved</param>
      /// <param name="delimiter">Delimiter. Default is ','</param>
      /// <param name="encloseHeaderInQuotes">
      ///    If set to <c>true</c> table column will be enclosed in quotes. Default is
      ///    <c>true</c>
      /// </param>
      /// <param name="comments">
      ///    List of comment that will be added at the beginning of the csv file. A # will be added at the
      ///    beginning of each item
      /// </param>
      public static void ExportToCSV(this DataTable dataTable, string fileName, string delimiter = ",", bool encloseHeaderInQuotes = true, IReadOnlyList<string> comments = null)
      {
         FileHelper.TrySaveFile(fileName, () => { exportToCSV(dataTable, fileName, delimiter, encloseHeaderInQuotes, comments); });
      }

      private static void exportToCSV(DataTable dataTable, string fileName, string delimiter, bool encloseHeaderInQuotes, IReadOnlyList<string> comments)
      {
         using (var sw = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
         {
            var allComments = comments ?? new List<string>();

            //Write the comments (and split  new lines if defined in one line)OSP
            foreach (var comment in allComments.SelectMany(c => c.Split(Environment.NewLine.ToCharArray())))
            {
               sw.WriteLine("#{0}", comment);
            }

            // Write the headers.
            int colCount = dataTable.Columns.Count;
            for (int i = 0; i < colCount; i++)
            {
               var columnName = dataTable.Columns[i].ColumnName;
               if (encloseHeaderInQuotes)
                  columnName = $"\"{columnName}\"";

               sw.Write(columnName);
               if (i < colCount - 1)
                  sw.Write(delimiter);
            }

            sw.Write(sw.NewLine);

            // Write rows.
            foreach (DataRow dr in dataTable.Rows)
            {
               for (int i = 0; i < colCount; i++)
               {
                  if (!Convert.IsDBNull(dr[i]))
                  {
                     sw.Write(dr[i].ToString());
                  }

                  if (i < colCount - 1)
                     sw.Write(delimiter);
               }

               sw.Write(sw.NewLine);
            }

            sw.Close();
         }
      }
   }
}