using System;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Conversion;
using OSPSuite.Utility.Tests.Helpers;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_ByteArrayConverter : ContextSpecification<ByteArrayConverter>
   {
      protected override void Context()
      {
         sut = new ByteArrayConverter();
      }

      // Fixture .bin files under TestFiles/ are real BinaryFormatter (net8)
      // payloads, committed so the NrbfDecoder fallback in ByteArrayConverter
      // can be exercised against actual legacy bytes without needing
      // BinaryFormatter at test runtime.
      protected static byte[] LoadLegacyFixture(string name)
      {
         return File.ReadAllBytes(HelperForSpecs.TestFileFullPath(name));
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

         _doubleArray[0] = double.PositiveInfinity;
         _doubleArray[1] = double.NegativeInfinity;
         _doubleArray[2] = double.NaN;
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

   public class When_converting_a_legacy_binaryformatter_int_array : concern_for_ByteArrayConverter
   {
      private static readonly int[] _expected = { 1, 2, 3, 42, -7 };
      private int[] _result;

      protected override void Because()
      {
         _result = sut.ConvertFromByteArray<int>(LoadLegacyFixture("int_array.bin"));
      }

      [Observation]
      public void should_return_the_original_int_array()
      {
         _result.Length.ShouldBeEqualTo(_expected.Length);
         for (var i = 0; i < _expected.Length; i++)
            _result[i].ShouldBeEqualTo(_expected[i]);
      }
   }

   public class When_converting_a_legacy_binaryformatter_string_array : concern_for_ByteArrayConverter
   {
      private static readonly string[] _expected = { "alpha", "beta", "gamma" };
      private string[] _result;

      protected override void Because()
      {
         _result = sut.ConvertFromByteArray<string>(LoadLegacyFixture("string_array.bin"));
      }

      [Observation]
      public void should_return_the_original_string_array()
      {
         _result.Length.ShouldBeEqualTo(_expected.Length);
         for (var i = 0; i < _expected.Length; i++)
            _result[i].ShouldBeEqualTo(_expected[i]);
      }
   }

   public class When_converting_a_legacy_binaryformatter_float_array : concern_for_ByteArrayConverter
   {
      private static readonly float[] _expected = { 1.5f, 2.5f, -3.5f };
      private float[] _result;

      protected override void Because()
      {
         _result = sut.ConvertFromByteArray<float>(LoadLegacyFixture("float_array.bin"));
      }

      [Observation]
      public void should_return_the_original_float_array()
      {
         _result.Length.ShouldBeEqualTo(_expected.Length);
         for (var i = 0; i < _expected.Length; i++)
            _result[i].ShouldBeEqualTo(_expected[i]);
      }
   }

   public class When_converting_a_legacy_binaryformatter_double_array_with_special_values : concern_for_ByteArrayConverter
   {
      private static readonly double[] _expected = { 1.5, double.PositiveInfinity, double.NegativeInfinity, double.NaN, -2.25 };
      private double[] _result;

      protected override void Because()
      {
         _result = sut.ConvertFromByteArray<double>(LoadLegacyFixture("double_array_with_special_values.bin"));
      }

      [Observation]
      public void should_return_the_original_double_array_including_NaN_and_infinities()
      {
         _result.Length.ShouldBeEqualTo(_expected.Length);
         for (var i = 0; i < _expected.Length; i++)
            _result[i].ShouldBeEqualTo(_expected[i]);
      }
   }
}