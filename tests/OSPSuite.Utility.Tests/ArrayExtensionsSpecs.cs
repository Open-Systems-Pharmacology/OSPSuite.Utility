using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public class When_told_to_increase_the_dimension_of_an_array : StaticContextSpecification
   {
      private double[] _arrayToRedim;

      protected override void Context()
      {
         _arrayToRedim = new double[] {1, 2, 3};
      }

      protected override void Because()
      {
         _arrayToRedim = _arrayToRedim.RedimPreserve(4);
      }

      [Observation]
      public void should_preserve_the_content_of_the_array()
      {
         _arrayToRedim.ShouldOnlyContainInOrder(1, 2, 3, 0);
      }

      [Observation]
      public void should_increase_the_dimension_to_the_new_dimension()
      {
         _arrayToRedim.Length.ShouldBeEqualTo(4);
      }
   }

   public class When_told_to_decrease_the_dimension_of_an_array : StaticContextSpecification
   {
      private double[] _arrayToRedim;

      protected override void Context()
      {
         _arrayToRedim = new double[] {1, 2, 3};
      }

      protected override void Because()
      {
         _arrayToRedim = _arrayToRedim.RedimPreserve(2);
      }

      [Observation]
      public void should_preserve_the_content_of_the_array_up_to_the_dimension()
      {
         _arrayToRedim.ShouldOnlyContainInOrder(1, 2);
      }

      [Observation]
      public void should_decrease_the_dimension_to_the_new_dimension()
      {
         _arrayToRedim.Length.ShouldBeEqualTo(2);
      }
   }

   public class When_told_to_redim_an_array_with_a_negativ_or_zero_dimension : StaticContextSpecification
   {
      private double[] _arrayToRedim;

      protected override void Context()
      {
         _arrayToRedim = new double[] {1, 2, 3};
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => _arrayToRedim.RedimPreserve(-2)).ShouldThrowAn<ArgumentException>();
         The.Action(() => _arrayToRedim.RedimPreserve(0)).ShouldThrowAn<ArgumentException>();
      }
   }

   public class When_told_to_redim_a_null_array_with_a_positive_dimension : StaticContextSpecification
   {
      private double[] _arrayToRedim;

      protected override void Context()
      {
         _arrayToRedim = null;
      }

      protected override void Because()
      {
         _arrayToRedim = _arrayToRedim.RedimPreserve(2);
      }

      [Observation]
      public void should_return_a_new_array()
      {
         _arrayToRedim.Length.ShouldBeEqualTo(2);
      }
   }

   public class When_converting_an_array_to_a_byte_array : StaticContextSpecification
   {
      private double[] _arrayToConvert;
      private byte[] _byteArray;

      protected override void Context()
      {
         var random = new Random();
         _arrayToConvert = new double[1000];
         for (int i = 0; i < _arrayToConvert.Length; i++)
         {
            _arrayToConvert[i] = random.NextDouble();
         }
      }

      protected override void Because()
      {
         _byteArray = _arrayToConvert.ToByteArray();
      }

      [Observation]
      public void should_be_able_to_convert_back_to_the_original_array()
      {
         var convertedArray = _byteArray.ToDoubleArray();
         convertedArray.ShouldOnlyContainInOrder(_arrayToConvert);
      }
   }

   public class When_converting_a_nullable_double_array_to_a_byte_string : StaticContextSpecification
   {
      private double?[] _arrayToConvert;
      private string _byteString;

      protected override void Context()
      {
         _arrayToConvert = new double?[3];
         _arrayToConvert[0] = 10;
         _arrayToConvert[1] = null;
         _arrayToConvert[2] = 20;
      }

      protected override void Because()
      {
         _byteString = _arrayToConvert.ToByteString();
      }

      [Observation]
      public void should_be_able_to_convert_back_to_the_original_array()
      {
         var convertedArray = _byteString.ToNullableDoubleArray();
         convertedArray[0].ShouldBeEqualTo(10);
         convertedArray[1].ShouldBeNull();
         convertedArray[2].ShouldBeEqualTo(20);
      }
   }

   public class When_converting_an_array_to_a_byte_string : StaticContextSpecification
   {
      private double[] _arrayToConvert;
      private string _byteString;

      protected override void Context()
      {
         var random = new Random();
         _arrayToConvert = new double[1000];
         for (int i = 0; i < _arrayToConvert.Length; i++)
         {
            _arrayToConvert[i] = random.NextDouble();
         }
      }

      protected override void Because()
      {
         _byteString = _arrayToConvert.ToByteString();
      }

      [Observation]
      public void should_be_able_to_convert_back_to_the_original_array()
      {
         var convertedArray = _byteString.ToDoubleArray();
         convertedArray.ShouldOnlyContainInOrder(_arrayToConvert);
      }
   }

   public class When_converting_a_byte_array_to_a_byte_string : StaticContextSpecification
   {
      private byte[] _arrayToConvert;
      private string _byteString;

      protected override void Context()
      {
         var random = new Random();
         _arrayToConvert = new byte[1000];
         for (int i = 0; i < _arrayToConvert.Length; i++)
         {
            _arrayToConvert[i] = (byte) random.Next(0, 200);
         }
      }

      protected override void Because()
      {
         _byteString = _arrayToConvert.ToByteString();
      }

      [Observation]
      public void should_be_able_to_convert_back_to_the_original_array()
      {
         var convertedArray = _byteString.ToByteArray();
         convertedArray.ShouldOnlyContainInOrder(_arrayToConvert);
      }
   }
}