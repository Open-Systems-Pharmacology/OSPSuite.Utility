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
      /// <summary>
      ///    Exports the datable to csv using the <paramref name="delimiter" /> given as parameter
      /// </summary>
      /// <param name="dataTable">DataTable to export</param>
      /// <param name="fileName">File where the <paramref name="dataTable" /> will be saved</param>
      /// <param name="delimiter">Delimiter</param>
      /// <param name="comments">
      ///    List of comment that will be added at the beginning of the csv file. A # will be added at the
      ///    beginning of each item
      /// </param>
      public static void ExportToCSV(this DataTable dataTable, string fileName, string delimiter = ";", IReadOnlyList<string> comments = null)
      {
         FileHelper.TrySaveFile(fileName, () => { exportToCSV(dataTable, fileName, delimiter, comments); });
      }

      private static void exportToCSV(DataTable dataTable, string fileName, string delimiter, IReadOnlyList<string> comments)
      {
         using (var sw = new StreamWriter(new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
         {
            var allComments = comments ?? new List<string>();

            //Write the comments (and split  new lines if defined in one line)
            foreach (var comment in allComments.SelectMany(c => c.Split(Environment.NewLine.ToCharArray())))
            {
               sw.WriteLine("#{0}", comment);
            }

            // Write the headers.
            int colCount = dataTable.Columns.Count;
            for (int i = 0; i < colCount; i++)
            {
               sw.Write(dataTable.Columns[i]);
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