using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Conversion;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_ByteArrayConverter : ContextSpecification<ByteArrayConverter>
   {
      protected override void Context()
      {
         sut = new ByteArrayConverter();
      }
   }

   public class When_converting_a_float_array_to_a_byte_array : concern_for_ByteArrayConverter
   {
      private float[] _floatArray;
      private byte[] _byteArray;

      protected override void Context()
      {
         base.Context();
         var random = new Random();
         _floatArray = new float[100000];
         for (int i = 0; i < _floatArray.Length; i++)
         {
            _floatArray[i] = (float) (random.NextDouble() * 100);
         }
      }

      protected override void Because()
      {
         _byteArray = sut.ConvertToByteArray(_floatArray);
      }

      [Observation]
      public void should_return_a_byte_array()
      {
         _byteArray.ShouldNotBeNull();
      }

      [Observation]
      public void should_be_able_to_convert_the_resulting_byte_array_back_to_the_original_flot_array()
      {
         var convertedFloatArray = sut.ConvertFromByteArray<float>(_byteArray);
         convertedFloatArray.Length.ShouldBeEqualTo(_floatArray.Length);
         for (int i = 0; i < _floatArray.Length; i++)
         {
            convertedFloatArray[i].ShouldBeEqualTo(_floatArray[i]);
         }
      }
   }

   public class When_converting_a_double_array_to_a_byte_array : concern_for_ByteArrayConverter
   {
      private double[] _doubleArray;
      private byte[] _byteArray;

      protected override void Context()
      {
         base.Context();
         var random = new Random();
         _doubleArray = new double[100000];
         for (int i = 0; i < _doubleArray.Length; i++)
         {
            _doubleArray[i] = random.NextDouble() * 10;
         }
      }

      protected override void Because()
      {
         _byteArray = sut.ConvertToByteArray(_doubleArray);
      }

      [Observation]
      public void should_return_a_byte_array()
      {
         _byteArray.ShouldNotBeNull();
      }

      [Observation]
      public void should_be_able_to_convert_the_resulting_byte_array_back_to_the_original_flot_array()
      {
         var convertedDoubleArray = sut.ConvertFromByteArray<double>(_byteArray);
         convertedDoubleArray.Length.ShouldBeEqualTo(_doubleArray.Length);
         for (int i = 0; i < _doubleArray.Length; i++)
         {
            convertedDoubleArray[i].ShouldBeEqualTo(_doubleArray[i]);
         }
      }
   }
}