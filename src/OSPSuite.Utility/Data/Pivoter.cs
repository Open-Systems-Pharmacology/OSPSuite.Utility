using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Data
{
   public interface IPivoter
   {
      /// <summary>
      ///    Creates a new data table containing the pivoted data view data.
      /// </summary>
      /// <param name="data">Data view containing data to pivot.</param>
      /// <param name="pivotInfo">Information specifying the pivotation.</param>
      /// <returns>Pivoted data.</returns>
      DataTable PivotData(DataView data, PivotInfo pivotInfo);
   }

   public class Pivoter : IPivoter
   {
      private const string CONDITION_FORMAT = "[{0}] = '{1}'";
      private const string CONDITION_AND = " AND ";

      /// <summary>
      ///    Checks whether specified columns do exists within given data.
      /// </summary>
      /// <param name="data">Data to search in.</param>
      /// <param name="pivotInfo">Pivot information with column specification.</param>
      private static void validate(DataView data, PivotInfo pivotInfo)
      {
         const string errorFormat = "The column {0} does not exists in given data.";

         foreach (var col in pivotInfo.RowFields.Union(pivotInfo.ColumnFields).Union(pivotInfo.DataFields))
            if (!data.Table.Columns.Contains(col))
               throw new Exception(String.Format(errorFormat, col));
      }

      /// <summary>
      ///    Creates a new data table containing the pivoted data.
      /// </summary>
      /// <param name="data">Data view containing data to pivot.</param>
      /// <param name="pivotInfo">Information specifying the pivotation.</param>
      /// <returns>Pivoted data.</returns>
      public DataTable PivotData(DataView data, PivotInfo pivotInfo)
      {
         validate(data, pivotInfo);

         switch (pivotInfo.Mode)
         {
            case PivotInfo.PivotMode.CrossTable:
               return pivotDataCross(data, pivotInfo);
            case PivotInfo.PivotMode.FlatTable:
               return pivotDataFlat(data, pivotInfo);
            default:
               throw new InvalidEnumArgumentException();
         }
      }

      /// <summary>
      ///    Creates a new data table containing the pivoted data.
      /// </summary>
      /// <remarks>It creates a group by analysis as cross table (row fields vs. column fields).</remarks>
      /// <param name="data">Data view containing data to pivot.</param>
      /// <param name="pivotInfo">Information specifying the pivotation.</param>
      /// <returns>Pivoted data.</returns>
      private DataTable pivotDataCross(DataView data, PivotInfo pivotInfo)
      {
         var dt = new DataTable();

         //Creates the row columns
         foreach (var field in pivotInfo.RowFields)
            dt.Columns.Add(field);

         var distinctColumnFieldsValues = getDistinctValuesFor(data, pivotInfo.ColumnFields);

         if (pivotInfo.ColumnFields.Any())
         {
            //Creates the result columns
            foreach (var columnValue in distinctColumnFieldsValues)
            {
               var name = getNameForColumnFields(pivotInfo, columnValue);
               foreach (var func in pivotInfo.Aggregates)
               {
                  if (pivotInfo.DataFields.Count > 1)
                     foreach (var datafield in pivotInfo.DataFields)
                        dt.Columns.Add(getColumnNameFor(name, func, datafield, pivotInfo.ColumnFieldsSeparator), func.DataType);
                  else
                     dt.Columns.Add(getColumnNameFor(name, func, pivotInfo.ColumnFieldsSeparator), func.DataType);
               }
               if (!pivotInfo.Aggregates.Any())
                  dt.Columns.Add(name);
            }
         }
         else
         {
            foreach (var func in pivotInfo.Aggregates)
            {
               if (pivotInfo.DataFields.Count > 1)
                  foreach (var datafield in pivotInfo.DataFields)
                     dt.Columns.Add(getColumnNameFor(func, datafield), func.DataType);
               else
                  dt.Columns.Add(func.Name, func.DataType);
            }
         }

         // Gets the list of row headers
         var distinctRowFieldsValues = getDistinctFieldsValues(data, pivotInfo.RowFields);

         //Create the rows
         foreach (var rowFieldsValues in distinctRowFieldsValues)
         {
            var row = dt.NewRow();
            var rowFilter = getFilterForFields(pivotInfo.RowFields, rowFieldsValues, String.Empty);

            fillFields(pivotInfo.RowFields, row, rowFieldsValues);

            if (pivotInfo.ColumnFields.Any())
            {
               // Build filter for column fields
               foreach (var columnFieldsValues in distinctColumnFieldsValues)
               {
                  var filter = getFilterForFields(pivotInfo.ColumnFields, columnFieldsValues, rowFilter);
                  var name = getNameForColumnFields(pivotInfo, columnFieldsValues);

                  // Get aggregated value of data field
                  foreach (var func in pivotInfo.Aggregates)
                  {
                     if (pivotInfo.DataFields.Count > 1)
                     {
                        foreach (var datafield in pivotInfo.DataFields)
                           row[getColumnNameFor(name, func, datafield, pivotInfo.ColumnFieldsSeparator)] = getData(data, filter, datafield, func);
                     }
                     else if (pivotInfo.DataFields.Count == 1)
                        row[getColumnNameFor(name, func, pivotInfo.ColumnFieldsSeparator)] = getData(data, filter, pivotInfo.DataFields[0], func);
                     else
                        row[getColumnNameFor(name, func, pivotInfo.ColumnFieldsSeparator)] = DBNull.Value;
                  }
                  if (!pivotInfo.Aggregates.Any())
                     row[name] = DBNull.Value;
               }
            }
            else
            {
               foreach (var func in pivotInfo.Aggregates)
                  if (pivotInfo.DataFields.Count > 1)
                     foreach (var datafield in pivotInfo.DataFields)
                        row[getColumnNameFor(func, datafield)] = getData(data, rowFilter, datafield, func);
                  else if (pivotInfo.DataFields.Count == 1)
                     row[func.Name] = getData(data, rowFilter, pivotInfo.DataFields[0], func);
                  else
                     row[func.Name] = DBNull.Value;
            }
            dt.Rows.Add(row);
         }
         return dt;
      }

      /// <summary>
      ///    Creates a new data table with pivoted data.
      /// </summary>
      /// <remarks>It is like a group by over row and column fields.</remarks>
      /// <param name="data">Data view containing data to pivot.</param>
      /// <param name="pivotInfo">Information specifying the pivotation.</param>
      /// <returns>A data table with pivoted data.</returns>
      private DataTable pivotDataFlat(DataView data, PivotInfo pivotInfo)
      {
         var dt = new DataTable();
         var fields = pivotInfo.RowFields.Union(pivotInfo.ColumnFields).ToArray();
         var dataFieldColumnName = pivotInfo.DataFieldColumnName;

         //Creates the columns
         foreach (var field in fields)
            dt.Columns.Add(field);

         if (pivotInfo.DataFields.Any())
            dt.Columns.Add(dataFieldColumnName, typeof(string));

         foreach (var func in pivotInfo.Aggregates)
            dt.Columns.Add(func.Name, func.DataType);

         var distinctFieldValues = getDistinctFieldsValues(data, fields);

         //Create the rows
         foreach (var values in distinctFieldValues)
         {
            var rowFilter = getFilterForFields(fields, values, String.Empty);
            if (pivotInfo.DataFields.Any())
            {
               foreach (var dataField in pivotInfo.DataFields)
               {
                  var row = dt.NewRow();
                  fillFields(fields, row, values);

                  row[dataFieldColumnName] = dataField;
                  foreach (var func in pivotInfo.Aggregates)
                     row[func.Name] = getData(data, rowFilter, dataField, func);

                  dt.Rows.Add(row);
               }
            }
            else
            {
               var row = dt.NewRow();
               fillFields(fields, row, values);
               dt.Rows.Add(row);
            }
         }
         return dt;
      }

      private static void fillFields(IEnumerable<string> fields, DataRow row, DataRow values)
      {
         foreach (var fieldName in fields)
            row[fieldName] = values[fieldName];
      }

      private static IEnumerable<DataRow> getDistinctFieldsValues(DataView data, IReadOnlyList<string> fields)
      {
         // if no row fields are specified we need one row for the calculated values
         return fields.Any()
            ? getDistinctValuesFor(data, fields)
            : getSingleEmptyRow(data);
      }

      private static List<DataRow> getSingleEmptyRow(DataView data)
      {
         return new List<DataRow>(1) {data.ToTable().NewRow()};
      }

      private static List<DataRow> getDistinctValuesFor(DataView data, IReadOnlyList<string> fields)
      {
         return data.ToTable(true, fields.ToArray()).Rows.Cast<DataRow>().ToList();
      }

      private static string getColumnNameFor(string aggregateName, string datafield)
      {
         if (string.IsNullOrEmpty(aggregateName))
            return datafield;

         return $"{aggregateName}({datafield})";
      }

      private static string getColumnNameFor(Aggregate func, string datafield)
      {
         return getColumnNameFor(func.Name, datafield);
      }

      private static string getColumnNameFor(string name, Aggregate func, char separator)
      {
         if (string.IsNullOrEmpty(name))
            return func.Name;

         if (string.IsNullOrEmpty(func.Name))
            return name;

         return $"{name}{separator}{func.Name}";
      }

      private static string getColumnNameFor(string name, Aggregate func, string datafield, char separator)
      {
         return getColumnNameFor(getColumnNameFor(name, func, separator), datafield);
      }

      private static string getNameForColumnFields(PivotInfo pivotInfo, DataRow row)
      {
         var columnNames = new List<string>();
         foreach (var fieldName in pivotInfo.ColumnFields)
         {
            string value = row[fieldName].ToString();
            if (!string.IsNullOrEmpty(value))
               columnNames.Add(value);
         }
         return columnNames.ToString(pivotInfo.ColumnFieldsSeparator.ToString());
      }

      private static string getFilterForFields(IEnumerable<string> fieldNames, DataRow values, string rowFilter)
      {
         var filter = rowFilter;
         foreach (var fieldName in fieldNames)
         {
            if (filter.Length > 0)
               filter += CONDITION_AND;

            var columnNameForFilter = fieldName.Replace(@"\", @"\\").Replace(@"]", @"\]");
            filter += String.Format(CONDITION_FORMAT, columnNameForFilter, values[fieldName]);
         }
         return filter;
      }

      /// <summary>
      ///    Aggregates sub data of data view.
      /// </summary>
      /// <param name="data">Data view containing all data.</param>
      /// <param name="filter">Filter to filter with.</param>
      /// <param name="dataField">Column to take values from.</param>
      /// <param name="aggregate">Function to be used for aggregation.</param>
      /// <returns>Aggregated value.</returns>
      private static object getData(DataView data, string filter, string dataField, Aggregate aggregate)
      {
         var filteredRows = data.Table.Select(filter);
         var objList = filteredRows.Select(x => x[dataField]).ToArray();

         return aggregate.PerformAggregation(objList) ?? DBNull.Value;
      }
   }
}