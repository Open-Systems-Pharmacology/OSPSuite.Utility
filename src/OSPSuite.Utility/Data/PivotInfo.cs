using System.Collections.Generic;

namespace OSPSuite.Utility.Data
{
   /// <summary>
   ///    This class is used to specify the pivoting.
   /// </summary>
   public class PivotInfo
   {
      /// <summary>
      ///    List of column which distinct values are used for group by as new rows.
      /// </summary>
      public IReadOnlyList<string> RowFields { get; private set; }

      /// <summary>
      ///    Column which contains pivoted values.
      /// </summary>
      public IReadOnlyList<string> DataFields { get; private set; }

      /// <summary>
      ///    List of functions to be used for aggregation.
      /// </summary>
      public IEnumerable<Aggregate> Aggregates { get; private set; }

      /// <summary>
      ///    List of Column which distinct data builds new columns to group by.
      /// </summary>
      public IReadOnlyList<string> ColumnFields { get; private set; }

      /// <summary>
      ///    Separator used in column headers to separate fields defined in column fields in cross table mode.
      /// </summary>
      public char ColumnFieldsSeparator { get; private set; }

      /// <summary>
      ///    Specifies the layout of the output table.
      /// </summary>
      public enum PivotMode
      {
         /// <summary>
         ///    It creates a group by analysis as cross table (row fields vs. column fields).
         /// </summary>
         CrossTable,

         /// <summary>
         ///    It is like a group by over row and column fields.
         /// </summary>
         FlatTable
      }

      /// <summary>
      ///    Name of the column for datafields in flat mode.
      /// </summary>
      public string DataFieldColumnName { get; private set; }

      /// <summary>
      ///    Specifies the layout of the output table.
      /// </summary>
      public PivotMode Mode { get; private set; }

      /// <summary>
      ///    Constructor of class.
      /// </summary>
      /// <param name="rowFields">List of columns which distinct values are used for group by as new rows.</param>
      /// <param name="columnFields">List of columns which distinct data builds new columns to group by.</param>
      /// <param name="dataFields">List of columns which contains pivoted values.</param>
      /// <param name="aggregates">List of functions to be used for aggregation.</param>
      /// <param name="columnFieldSeparator">Specifies the separator used for column names in cross table mode.</param>
      /// <param name="mode">Specifies the output format of the pivoted data.</param>
      /// <param name="dataFieldColumnName">Name of the column for datafields in flat mode.</param>
      public PivotInfo(IReadOnlyList<string> rowFields, IReadOnlyList<string> columnFields, IReadOnlyList<string> dataFields, IEnumerable<Aggregate> aggregates, char columnFieldSeparator = '.', PivotMode mode = PivotMode.CrossTable, string dataFieldColumnName = "DataField")
      {
         RowFields = rowFields;
         DataFields = dataFields;
         Aggregates = aggregates;
         ColumnFields = columnFields;
         ColumnFieldsSeparator = columnFieldSeparator;
         Mode = mode;
         DataFieldColumnName = dataFieldColumnName;
      }
   }
}