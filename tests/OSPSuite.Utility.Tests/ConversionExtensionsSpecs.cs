using System;
using System.Globalization;
using System.Threading;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_ConversionExtensions : StaticContextSpecification
   {
   }

   public class When_checking_if_a_double_value_is_an_integer : concern_for_ConversionExtensions
   {
      private double _doubleValue;

      protected override void Context()
      {
         _doubleValue = 5;
      }

      [Observation]
      public void should_return_false()
      {
         _doubleValue.IsAnImplementationOf<int>().ShouldBeFalse();
         _doubleValue.IsAnImplementationOf(typeof(int)).ShouldBeFalse();
      }
   }

   public class When_checking_if_a_double_value_is_a_double : concern_for_ConversionExtensions
   {
      private double _doubleValue;

      protected override void Context()
      {
         _doubleValue = 5;
      }

      [Observation]
      public void should_return_true()
      {
         _doubleValue.IsAnImplementationOf<double>().ShouldBeTrue();
         _doubleValue.IsAnImplementationOf(typeof(double)).ShouldBeTrue();
      }
   }

   public class When_checking_if_an_object_deriving_from_a_base_class_is_a_base_class : concern_for_ConversionExtensions
   {
      private ADerivedImplementation _derivedObject;

      protected override void Context()
      {
         _derivedObject = new ADerivedImplementation();
      }

      [Observation]
      public void should_return_true()
      {
         _derivedObject.IsAnImplementationOf<AnotherImplementation>().ShouldBeTrue();
         _derivedObject.IsAnImplementationOf(typeof(AnotherImplementation)).ShouldBeTrue();
      }
   }

   public class When_checking_if_an_object_deriving_from_a_base_class_is_an_implementation_of_an_interface_implemented_by_the_base_class : concern_for_ConversionExtensions
   {
      private ADerivedImplementation _derivedObject;

      protected override void Context()
      {
         _derivedObject = new ADerivedImplementation();
      }

      [Observation]
      public void should_return_true()
      {
         _derivedObject.IsAnImplementationOf<IAnInterface>().ShouldBeTrue();
         _derivedObject.IsAnImplementationOf(typeof(IAnInterface)).ShouldBeTrue();
      }
   }

   public class When_checking_if_an_object_deriving_from_a_base_class_is_an_implementation_of_another_object_deriving_from_the_same_base_class_ : concern_for_ConversionExtensions
   {
      private AnImplementation _oneImplementation;

      protected override void Context()
      {
         _oneImplementation = new AnImplementation();
      }

      [Observation]
      public void should_return_false()
      {
         _oneImplementation.IsAnImplementationOf<AnotherImplementation>().ShouldBeFalse();
         _oneImplementation.IsAnImplementationOf(typeof(AnotherImplementation)).ShouldBeFalse();
      }
   }

   public class When_checking_if_an_object_implementing_an_interface_is_an_implementation_of_another_interface_implemented_also_by_the_first_interface : concern_for_ConversionExtensions
   {
      private IAnotherInteface _object1;
      private IAnInterface _object2;

      protected override void Context()
      {
         _object1 = new AnotherImplementation();
         _object2 = new AnotherImplementation();
      }

      [Observation]
      public void should_return_true()
      {
         _object1.IsAnImplementationOf<IAnInterface>().ShouldBeTrue();
         _object1.IsAnImplementationOf(typeof(IAnInterface)).ShouldBeTrue();
         _object2.IsAnImplementationOf<IAnInterface>().ShouldBeTrue();
         _object2.IsAnImplementationOf(typeof(IAnInterface)).ShouldBeTrue();
         _object2.IsAnImplementationOf<IAnotherInteface>().ShouldBeTrue();
         _object2.IsAnImplementationOf(typeof(IAnotherInteface)).ShouldBeTrue();
      }
   }

   public class When_converting_a_double_to_a_nullable_double : concern_for_ConversionExtensions
   {
      private double _doubleValue;

      protected override void Context()
      {
         _doubleValue = 5;
      }

      [Observation]
      public void should_return_a_nullable_double_containing_the_double_value_as_value()
      {
         var result = _doubleValue.ConvertedTo<double?>();
         result.ShouldNotBeNull();
         result.Value.ShouldBeEqualTo(_doubleValue);
      }
   }

   public class When_converting_an_empty_string_to_a_nullable_double : concern_for_ConversionExtensions
   {
      private string _emptyString;

      protected override void Context()
      {
         _emptyString = string.Empty;
      }

      [Observation]
      public void should_return_a_nullable_double_containing_the_double_value_as_value()
      {
         var result = _emptyString.ConvertedTo<double?>();
         result.ShouldBeNull();
      }
   }

   public class When_converting_an_empty_string_to_a_double : concern_for_ConversionExtensions
   {
      private string _emptyString;

      protected override void Context()
      {
         _emptyString = string.Empty;
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => _emptyString.ConvertedTo<double>()).ShouldThrowAn<Exception>();
      }
   }

   public class When_converting_a_valid_double_string_to_a_double : concern_for_ConversionExtensions
   {
      private string _doubleValueAsStringWithPoint;
      private string _doubleValueAsStringWithComma;

      protected override void Context()
      {
         _doubleValueAsStringWithPoint = "3.5";
         _doubleValueAsStringWithComma = "3,5";
      }

      [Observation]
      public void should_be_able_to_convert_the_value_with_a_decimal_separator_set_to_point()
      {
         _doubleValueAsStringWithPoint.ConvertedTo<double>().ShouldBeEqualTo(3.5);
      }

      [Observation]
      public void should_be_able_to_convert_the_value_with_a_decimal_separator_set_to_comma()
      {
         _doubleValueAsStringWithComma.ConvertedTo<double>().ShouldBeEqualTo(3.5);
      }
   }

   public class When_checking_if_a_type_implements_another_type : concern_for_ConversionExtensions
   {
      [Observation]
      public void should_return_true_if_the_type_implements_the_other_type()
      {
         typeof(AnImplementation).IsAnImplementationOf<IAnInterface>().ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_type_are_not_related()
      {
         typeof(AnImplementation).IsAnImplementationOf<IAnotherInteface>().ShouldBeFalse();
      }
   }

   public class When_converting_a_valid_double_string_to_a_nullable_double : concern_for_ConversionExtensions
   {
      private string _doubleValueAsStringWithPoint;
      private string _doubleValueAsStringWithComma;

      protected override void Context()
      {
         _doubleValueAsStringWithPoint = "3.5";
         _doubleValueAsStringWithComma = "3,5";
      }

      [Observation]
      public void should_be_able_to_convert_the_value_with_a_decimal_separator_set_to_point()
      {
         _doubleValueAsStringWithPoint.ConvertedTo<double?>().ShouldBeEqualTo(3.5);
      }

      [Observation]
      public void should_be_able_to_convert_the_value_with_a_decimal_separator_set_to_comma()
      {
         _doubleValueAsStringWithComma.ConvertedTo<double?>().ShouldBeEqualTo(3.5);
      }
   }

   public class When_converting_a_double_value_to_string : concern_for_ConversionExtensions
   {
      private double _value;
      private CultureInfo _oldUICulture;
      private CultureInfo _oldCulture;

      protected override void Context()
      {
         base.Context();
         _oldUICulture = Thread.CurrentThread.CurrentUICulture;
         _oldCulture = Thread.CurrentThread.CurrentCulture;

         var culture = new CultureInfo("en-US");
         culture.NumberFormat.NumberDecimalSeparator = ",";
         Thread.CurrentThread.CurrentUICulture = culture;
         Thread.CurrentThread.CurrentCulture = culture;

         _value = 2.5;
      }

      [Observation]
      public void should_always_use_the_invariant_info_to_convert_the_value_to_string()
      {
         _value.ConvertedTo<string>().ShouldBeEqualTo("2.5");
      }

      public override void Cleanup()
      {
         Thread.CurrentThread.CurrentUICulture = _oldUICulture;
         Thread.CurrentThread.CurrentCulture = _oldCulture;
      }
   }
}