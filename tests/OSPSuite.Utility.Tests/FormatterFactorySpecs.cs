using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Format;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_FormatterFactory : ContextSpecification<IFormatterFactory>
   {
      protected override void Context()
      {
         sut = new FormatterFactory();
      }
   }

   public class When_resolving_a_formatter_for_a_type_that_is_not_numeric : concern_for_FormatterFactory
   {
      [Observation]
      public void should_return_a_default_formatter()
      {
         sut.CreateFor<string>().ShouldBeAnInstanceOf<DefaultFormatter<string>>();
      }
   }

   public class When_resolving_a_formatter_for_a_type_that_is_numeric : concern_for_FormatterFactory
   {
      [Observation]
      public void should_return_a_numeric_formatter()
      {
         sut.CreateFor<double>().ShouldBeAnInstanceOf<NumericFormatter<double>>();
      }
   }
}