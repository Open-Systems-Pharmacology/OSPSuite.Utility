using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Format;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_NumericFormatter<T> : ContextSpecification<INumericFormatter<T>>
   {
      protected INumericFormatterOptions _numericOptions;

      protected override void Context()
      {
         _numericOptions = NumericFormatterOptions.Instance;
         sut = new NumericFormatter<T>(_numericOptions);
      }
   }

   public class When_formatting_a_nullable_double_value : concern_for_NumericFormatter<double?>
   {
      protected override void Context()
      {
         base.Context();
         _numericOptions.DecimalPlace = 2;
      }

      [Observation]
      public void should_return_an_empty_string_if_the_nullable_is_empty()
      {
         sut.Format(new double?()).ShouldBeEqualTo(string.Empty);
      }

      [Observation]
      public void should_return_a_string_representing_the_double_value_otherwise()
      {
         sut.Format(3.56).ShouldBeEqualTo("3.56");
         sut.Format(3.567).ShouldBeEqualTo("3.57");
      }
   }

   public class When_formatting_a_double_value_with_3_decimal_places_and_scientific_notation : concern_for_NumericFormatter<double>
   {
      protected override void Context()
      {
         base.Context();
         _numericOptions.DecimalPlace = 3;
         _numericOptions.AllowsScientificNotation = true;
      }

      [Observation]
      public void should_return_a_string_representing_the_double_value_in_the_desired_format()
      {
         sut.Format(3.56).ShouldBeEqualTo("3.560");
         sut.Format(3.567).ShouldBeEqualTo("3.567");
         sut.Format(12315678).ShouldBeEqualTo("1.232E+7");
         sut.Format(15).ShouldBeEqualTo("15.000");
         sut.Format(150).ShouldBeEqualTo("150.000");
         sut.Format(1500).ShouldBeEqualTo("1500.000");
         sut.Format(0.15).ShouldBeEqualTo("0.150");
         sut.Format(0.015).ShouldBeEqualTo("0.015");
         sut.Format(0.0015).ShouldBeEqualTo("1.500E-3");
         sut.Format(0.00000).ShouldBeEqualTo("0");
      }
   }

   public class When_formatting_a_double_value_without_any_digit : concern_for_NumericFormatter<double>
   {
      protected override void Context()
      {
         base.Context();
         _numericOptions.DecimalPlace = 0;
         _numericOptions.AllowsScientificNotation = false;
      }

      [Observation]
      public void should_return_a_string_representing_the_double_value_in_the_desired_format()
      {
         sut.Format(3.56).ShouldBeEqualTo("4");
         sut.Format(3.567).ShouldBeEqualTo("4");
         sut.Format(12315678).ShouldBeEqualTo("12315678");
         sut.Format(15).ShouldBeEqualTo("15");
         sut.Format(150).ShouldBeEqualTo("150");
         sut.Format(0.15).ShouldBeEqualTo("0");
         sut.Format(0.015).ShouldBeEqualTo("0");
         sut.Format(0.0015).ShouldBeEqualTo("0");
         sut.Format(0.00000).ShouldBeEqualTo("0");
      }
   }

   public class When_formatting_an_integer_value : concern_for_NumericFormatter<int>
   {
      protected override void Context()
      {
         base.Context();
         _numericOptions.DecimalPlace = 2;
         _numericOptions.AllowsScientificNotation = false;
      }

      [Observation]
      public void should_return_a_string_representing_the_integer_in_the_desired_format()
      {
         sut.Format(4).ShouldBeEqualTo("4");
         sut.Format(12315678).ShouldBeEqualTo("12315678");
         sut.Format(15).ShouldBeEqualTo("15");
         sut.Format(150).ShouldBeEqualTo("150");
      }
   }

   public class When_formatting_an_unsigned_long_value : concern_for_NumericFormatter<ulong>
   {
      protected override void Context()
      {
         base.Context();
         _numericOptions.DecimalPlace = 2;
         _numericOptions.AllowsScientificNotation = false;
      }

      [Observation]
      public void should_return_a_string_representing_the_unsigned_long_in_the_desired_format()
      {
         sut.Format(18446744073709551000).ShouldBeEqualTo("18446744073709551000");
      }
   }
}