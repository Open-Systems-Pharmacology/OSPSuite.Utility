using System;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Data;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_Aggregate : ContextSpecification<Aggregate>
   {
      protected override void Context()
      {
         sut = new Aggregate<double, double>();
      }
   }

   public class When_aggregating_values_for_sum : concern_for_Aggregate
   {
      protected override void Context()
      {
         base.Context();

         sut = new Aggregate<double, double>
         {
            Aggregation = values => values.Sum(),
            Name = "Sum"
         };
      }

      [Observation]
      public void should_return_the_expected_result_for_sum()
      {
         sut.PerformAggregation(new object[] {0D, 1D, 2D, 3D, 4D}).ShouldBeEqualTo(10);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_with_DBNull_values()
      {
         sut.PerformAggregation(new object[] {0D, 1D, DBNull.Value, 3D, 4D}).ShouldBeEqualTo(8);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_for_only_DBNull_values()
      {
         sut.PerformAggregation(new object[] {DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value}).ShouldBeEqualTo(DBNull.Value);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_for_empty_list()
      {
         sut.PerformAggregation(new object[] {}).ShouldBeEqualTo(DBNull.Value);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_for_null_object()
      {
         sut.PerformAggregation(null).ShouldBeEqualTo(DBNull.Value);
      }
   }

   public class When_aggregating_values_for_min : concern_for_Aggregate
   {
      protected override void Context()
      {
         base.Context();

         sut = new Aggregate<double, double>
         {
            Aggregation = values => values.Min(),
            Name = "Min"
         };
      }

      [Observation]
      public void should_return_the_expected_result_for_sum()
      {
         sut.PerformAggregation(new object[] {0D, 1D, 2D, 3D, 4D}).ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_with_DBNull_values()
      {
         sut.PerformAggregation(new object[] {0D, 1D, DBNull.Value, 3D, 4D}).ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_for_only_DBNull_values()
      {
         sut.PerformAggregation(new object[] {DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value}).ShouldBeEqualTo(DBNull.Value);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_for_empty_list()
      {
         sut.PerformAggregation(new object[] {}).ShouldBeEqualTo(DBNull.Value);
      }

      [Observation]
      public void should_return_the_expected_result_for_sum_for_null_object()
      {
         sut.PerformAggregation(null).ShouldBeEqualTo(DBNull.Value);
      }
   }
}