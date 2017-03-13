using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace OSPSuite.Utility.Tests
{
   public class When_retrieving_all_values_defined_in_an_enumeration : StaticContextSpecification
   {
      private enum TestEnum
      {
         Value1,
         Value2
      }

      [Observation]
      public void should_return_all_values_in_the_accurate_order()
      {
         EnumHelper.AllValuesFor<TestEnum>().ShouldOnlyContainInOrder(TestEnum.Value1, TestEnum.Value2);
      }
   }

   public class When_retrieving_all_values_defined_for_a_type_that_is_not_an_enumeration : StaticContextSpecification
   {
      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => EnumHelper.AllValuesFor<AnImplementation>()).ShouldThrowAn<ArgumentException>();
      }
   }

   public class When_parsing_a_string_to_an_enum_value : StaticContextSpecification
   {
      private enum TestEnum
      {
         Value1,
         Value2
      }

      [Observation]
      public void should_return_the_value_in_the_enum_if_the_parsing_succeeded()
      {
         EnumHelper.ParseValue<TestEnum>("Value1").ShouldBeEqualTo(TestEnum.Value1);
         EnumHelper.ParseValue<TestEnum>("vaLue2").ShouldBeEqualTo(TestEnum.Value2);
      }

      [Observation]
      public void should_throw_an_exception_otherwise()
      {
         The.Action(() => EnumHelper.ParseValue<TestEnum>("Value3")).ShouldThrowAn<Exception>();
      }
   }
}