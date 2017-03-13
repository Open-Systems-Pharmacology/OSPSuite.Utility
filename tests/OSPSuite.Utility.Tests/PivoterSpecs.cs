using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Data;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_Pivoter : ContextSpecification<IPivoter>
   {
      protected override void Context()
      {
         sut = new Pivoter();
      }
   }

   #region Mode = CrossTable

   public class When_pivoting_a_well_formatted_data_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var category2 = dataTable.Columns.Add("Category2", typeof(string));
         var subgroup = dataTable.Columns.Add("Subgroup", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var keyfigure = dataTable.Columns.Add("KeyFigure", typeof(string));

         for (var i = 0; i < 2; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i == 0) ? "A" : "B";
            row[category2] = (i == 0) ? "C" : "D";
            row[subgroup] = (i == 0) ? "1" : "2";
            row[dataValue] = i;
            row[keyfigure] = (i == 0) ? "Figure 1" : "Figure 2";
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = values => values.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName, category2.ColumnName,},
            columnFields: new[] {subgroup.ColumnName, keyfigure.ColumnName},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void should_return_the_expected_pivot_table_using_the_given_aggregation()
      {
         _pivotTable.Rows[0]["1.Figure 1.Sum"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["2.Figure 2.Sum"].ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_return_null_in_the_empty_areas()
      {
         _pivotTable.Rows[0]["2.Figure 2.Sum"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[1]["1.Figure 1.Sum"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_pivoting_a_well_formatted_data_table_with_some_empty_column_used_for_aggregation : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var category2 = dataTable.Columns.Add("Category2", typeof(string));
         var subgroup = dataTable.Columns.Add("Subgroup", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var keyfigure = dataTable.Columns.Add("KeyFigure", typeof(string));

         for (var i = 0; i < 2; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i == 0) ? "A" : "B";
            row[category2] = (i == 0) ? "C" : "D";
            row[subgroup] = "";
            row[dataValue] = i;
            row[keyfigure] = "";
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = values => values.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName, category2.ColumnName,},
            columnFields: new[] {subgroup.ColumnName, keyfigure.ColumnName},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void should_return_the_expected_pivot_table_using_the_given_aggregation()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(0);
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(45);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(145);
      }
   }

   public class When_using_special_characters_in_column_names : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category [Strange characters @€³/µ[]]", typeof(string));
         var dataValue = dataTable.Columns.Add("Value[µg/ml]", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(45);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(145);
      }
   }

   public class When_no_datafield_is_specified : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new string[] {},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_no_rowfield_is_specified : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new string[] {},
            columnFields: new[] {category.ColumnName},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["A.Sum"].ShouldBeEqualTo(45);
         _pivotTable.Rows[0]["B.Sum"].ShouldBeEqualTo(145);
      }
   }

   public class When_no_pivot_data_are_specified : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new string[] {},
            columnFields: new string[] {},
            dataFields: new string[] {},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggreates_value_should_not_have_any_rows()
      {
         _pivotTable.Rows.Count.ShouldBeEqualTo(1);
         _pivotTable.Columns.Count.ShouldBeEqualTo(1);
         _pivotTable.Rows[0][0].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data_with_null_values : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = (i % 2 == 0) ? i : (object) DBNull.Value;
            dataTable.Rows.Add(row);
         }
         var newRow = dataTable.NewRow();
         newRow.ItemArray = new object[] {"C", DBNull.Value};
         dataTable.Rows.Add(newRow);

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };
         aggregates.Add(sumAggregate);

         var minAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Min(),
            Name = "Min"
         };
         aggregates.Add(minAggregate);

         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(20);
         _pivotTable.Rows[0]["Min"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(70);
         _pivotTable.Rows[1]["Min"].ShouldBeEqualTo(10);
         _pivotTable.Rows[2]["Sum"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[2]["Min"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_pivoting_a_well_formatted_data_table_for_two_datafields : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var category2 = dataTable.Columns.Add("Category2", typeof(string));
         var subgroup = dataTable.Columns.Add("Subgroup", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var dataValue2 = dataTable.Columns.Add("Value2", typeof(double));
         var keyfigure = dataTable.Columns.Add("KeyFigure", typeof(string));

         for (var i = 0; i < 2; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i == 0) ? "A" : "B";
            row[category2] = (i == 0) ? "C" : "D";
            row[subgroup] = (i == 0) ? "1" : "2";
            row[dataValue] = i;
            row[dataValue2] = i * i;
            row[keyfigure] = (i == 0) ? "Figure 1" : "Figure 2";
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = values => values.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName, category2.ColumnName,},
            columnFields: new[] {subgroup.ColumnName, keyfigure.ColumnName},
            dataFields: new[] {dataValue.ColumnName, dataValue2.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void should_return_the_expected_pivot_table_using_the_given_aggregation()
      {
         _pivotTable.Rows[0]["1.Figure 1.Sum(Value)"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["2.Figure 2.Sum(Value)"].ShouldBeEqualTo(1);
         _pivotTable.Rows[0]["1.Figure 1.Sum(Value2)"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["2.Figure 2.Sum(Value2)"].ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_return_null_in_the_empty_areas()
      {
         _pivotTable.Rows[0]["2.Figure 2.Sum(Value)"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[1]["1.Figure 1.Sum(Value)"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[0]["2.Figure 2.Sum(Value2)"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[1]["1.Figure 1.Sum(Value2)"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data_for_two_datafields : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var dataValue2 = dataTable.Columns.Add("Value2", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            row[dataValue2] = i * i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName, dataValue2.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum(Value)"].ShouldBeEqualTo(45);
         _pivotTable.Rows[1]["Sum(Value)"].ShouldBeEqualTo(145);
         _pivotTable.Rows[0]["Sum(Value2)"].ShouldBeEqualTo(285);
         _pivotTable.Rows[1]["Sum(Value2)"].ShouldBeEqualTo(2185);
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data_with_null_values_for_two_datafields : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var dataValue2 = dataTable.Columns.Add("Value2", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = (i % 2 == 0) ? i : (object) DBNull.Value;
            row[dataValue2] = (i % 2 == 0) ? i * i : (object) DBNull.Value;
            dataTable.Rows.Add(row);
         }
         var newRow = dataTable.NewRow();
         newRow.ItemArray = new object[] {"C", DBNull.Value};
         dataTable.Rows.Add(newRow);

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };
         aggregates.Add(sumAggregate);

         var minAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Min(),
            Name = "Min"
         };
         aggregates.Add(minAggregate);

         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName, dataValue2.ColumnName},
            aggregates: aggregates
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum(Value)"].ShouldBeEqualTo(20);
         _pivotTable.Rows[0]["Min(Value)"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["Sum(Value)"].ShouldBeEqualTo(70);
         _pivotTable.Rows[1]["Min(Value)"].ShouldBeEqualTo(10);
         _pivotTable.Rows[2]["Sum(Value)"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[2]["Min(Value)"].ShouldBeEqualTo(DBNull.Value);

         _pivotTable.Rows[0]["Sum(Value2)"].ShouldBeEqualTo(120);
         _pivotTable.Rows[0]["Min(Value2)"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["Sum(Value2)"].ShouldBeEqualTo(1020);
         _pivotTable.Rows[1]["Min(Value2)"].ShouldBeEqualTo(100);
         _pivotTable.Rows[2]["Sum(Value2)"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[2]["Min(Value2)"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   #endregion

   #region Mode = FlatTable

   public class When_pivoting_a_well_formatted_data_table_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var category2 = dataTable.Columns.Add("Category2", typeof(string));
         var subgroup = dataTable.Columns.Add("Subgroup", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var keyfigure = dataTable.Columns.Add("KeyFigure", typeof(string));

         for (var i = 0; i < 2; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i == 0) ? "A" : "B";
            row[category2] = (i == 0) ? "C" : "D";
            row[subgroup] = (i == 0) ? "1" : "2";
            row[dataValue] = i;
            row[keyfigure] = (i == 0) ? "Figure 1" : "Figure 2";
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = values => values.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName, category2.ColumnName,},
            columnFields: new[] {subgroup.ColumnName, keyfigure.ColumnName},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void should_return_the_expected_pivot_table_using_the_given_aggregation()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_return_only_2_rows()
      {
         _pivotTable.Rows.Count.ShouldBeEqualTo(2);
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(45);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(145);
      }
   }

   public class When_no_datafield_is_specified_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new string[] {},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_no_rowfield_is_specified_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new string[] {},
            columnFields: new[] {category.ColumnName},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(45);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(145);
      }
   }

   public class When_no_pivot_data_are_specified_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new string[] {},
            columnFields: new string[] {},
            dataFields: new string[] {},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggreates_value_should_not_have_any_rows()
      {
         _pivotTable.Rows.Count.ShouldBeEqualTo(1);
         _pivotTable.Columns.Count.ShouldBeEqualTo(1);
         _pivotTable.Rows[0][0].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data_with_null_values_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = (i % 2 == 0) ? i : (object) DBNull.Value;
            dataTable.Rows.Add(row);
         }
         var newRow = dataTable.NewRow();
         newRow.ItemArray = new object[] {"C", DBNull.Value};
         dataTable.Rows.Add(newRow);

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };
         aggregates.Add(sumAggregate);

         var minAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Min(),
            Name = "Min"
         };
         aggregates.Add(minAggregate);

         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(20);
         _pivotTable.Rows[0]["Min"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(70);
         _pivotTable.Rows[1]["Min"].ShouldBeEqualTo(10);
         _pivotTable.Rows[2]["Sum"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[2]["Min"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_pivoting_a_well_formatted_data_table_for_two_datafields_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var category2 = dataTable.Columns.Add("Category2", typeof(string));
         var subgroup = dataTable.Columns.Add("Subgroup", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var dataValue2 = dataTable.Columns.Add("Value2", typeof(double));
         var keyfigure = dataTable.Columns.Add("KeyFigure", typeof(string));

         for (var i = 0; i < 2; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i == 0) ? "A" : "B";
            row[category2] = (i == 0) ? "C" : "D";
            row[subgroup] = (i == 0) ? "1" : "2";
            row[dataValue] = i;
            row[dataValue2] = i * i;
            row[keyfigure] = (i == 0) ? "Figure 1" : "Figure 2";
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = values => values.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName, category2.ColumnName,},
            columnFields: new[] {subgroup.ColumnName, keyfigure.ColumnName},
            dataFields: new[] {dataValue.ColumnName, dataValue2.ColumnName},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void should_return_the_expected_pivot_table_using_the_given_aggregation()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(0);
         _pivotTable.Rows[0]["DataField"].ShouldBeEqualTo("Value");
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["DataField"].ShouldBeEqualTo("Value2");
         _pivotTable.Rows[2]["Sum"].ShouldBeEqualTo(1);
         _pivotTable.Rows[2]["DataField"].ShouldBeEqualTo("Value");
         _pivotTable.Rows[3]["Sum"].ShouldBeEqualTo(1);
         _pivotTable.Rows[3]["DataField"].ShouldBeEqualTo("Value2");
      }

      [Observation]
      public void should_return_only_4_rows()
      {
         _pivotTable.Rows.Count.ShouldBeEqualTo(4);
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data_for_two_datafields_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var dataValue2 = dataTable.Columns.Add("Value2", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = i;
            row[dataValue2] = i * i;
            dataTable.Rows.Add(row);
         }

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };

         aggregates.Add(sumAggregate);
         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName, dataValue2.ColumnName},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(45);
         _pivotTable.Rows[0]["DataField"].ShouldBeEqualTo("Value");
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(285);
         _pivotTable.Rows[1]["DataField"].ShouldBeEqualTo("Value2");
         _pivotTable.Rows[2]["Sum"].ShouldBeEqualTo(145);
         _pivotTable.Rows[2]["DataField"].ShouldBeEqualTo("Value");
         _pivotTable.Rows[3]["Sum"].ShouldBeEqualTo(2185);
         _pivotTable.Rows[3]["DataField"].ShouldBeEqualTo("Value2");
      }
   }

   public class When_using_an_aggreation_function_that_should_aggregate_some_data_with_null_values_for_two_datafields_for_flat_table : concern_for_Pivoter
   {
      private DataView _data;
      private PivotInfo _pivotInfo;
      private DataTable _pivotTable;

      protected override void Context()
      {
         base.Context();

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var dataValue2 = dataTable.Columns.Add("Value2", typeof(double));

         for (var i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 10) ? "A" : "B";
            row[dataValue] = (i % 2 == 0) ? i : (object) DBNull.Value;
            row[dataValue2] = (i % 2 == 0) ? i * i : (object) DBNull.Value;
            dataTable.Rows.Add(row);
         }
         var newRow = dataTable.NewRow();
         newRow.ItemArray = new object[] {"C", DBNull.Value};
         dataTable.Rows.Add(newRow);

         _data = dataTable.DefaultView;

         var aggregates = new List<Aggregate>();
         var sumAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Sum(),
            Name = "Sum"
         };
         aggregates.Add(sumAggregate);

         var minAggregate = new Aggregate<double, double>
         {
            Aggregation = x => x.Min(),
            Name = "Min"
         };
         aggregates.Add(minAggregate);

         _pivotInfo = new PivotInfo(
            rowFields: new[] {category.ColumnName},
            columnFields: new string[] {},
            dataFields: new[] {dataValue.ColumnName, dataValue2.ColumnName},
            aggregates: aggregates,
            mode: PivotInfo.PivotMode.FlatTable
         );
      }

      protected override void Because()
      {
         _pivotTable = sut.PivotData(_data, _pivotInfo);
      }

      [Observation]
      public void the_aggregated_value_should_be_found_in_the_returned_table()
      {
         _pivotTable.Rows[0]["DataField"].ShouldBeEqualTo("Value");
         _pivotTable.Rows[0]["Sum"].ShouldBeEqualTo(20);
         _pivotTable.Rows[0]["Min"].ShouldBeEqualTo(0);
         _pivotTable.Rows[1]["DataField"].ShouldBeEqualTo("Value2");
         _pivotTable.Rows[1]["Sum"].ShouldBeEqualTo(120);
         _pivotTable.Rows[1]["Min"].ShouldBeEqualTo(0);
         _pivotTable.Rows[2]["DataField"].ShouldBeEqualTo("Value");
         _pivotTable.Rows[2]["Sum"].ShouldBeEqualTo(70);
         _pivotTable.Rows[2]["Min"].ShouldBeEqualTo(10);
         _pivotTable.Rows[3]["DataField"].ShouldBeEqualTo("Value2");
         _pivotTable.Rows[3]["Sum"].ShouldBeEqualTo(1020);
         _pivotTable.Rows[3]["Min"].ShouldBeEqualTo(100);
         _pivotTable.Rows[4]["DataField"].ShouldBeEqualTo("Value");
         _pivotTable.Rows[4]["Sum"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[4]["Min"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[5]["DataField"].ShouldBeEqualTo("Value2");
         _pivotTable.Rows[5]["Sum"].ShouldBeEqualTo(DBNull.Value);
         _pivotTable.Rows[5]["Min"].ShouldBeEqualTo(DBNull.Value);
      }
   }

   #endregion
}