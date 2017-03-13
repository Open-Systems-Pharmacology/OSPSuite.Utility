using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_type_extensions : StaticContextSpecification
   {
      protected override void Context()
      {
      }
   }

   public class When_checking_if_an_integer_type_is_numeric : concern_for_type_extensions
   {
      [Observation]
      public void should_return_true()
      {
         typeof(int).IsNumeric().ShouldBeTrue();
      }
   }

   public class When_checking_if_an_integer_type_is_an_integer_type : concern_for_type_extensions
   {
      [Observation]
      public void should_return_true()
      {
         typeof(int).IsInteger().ShouldBeTrue();
      }
   }

   public class When_checking_if_an_integer_type_is_a_double_type : concern_for_type_extensions
   {
      [Observation]
      public void should_return_false()
      {
         typeof(int).IsDouble().ShouldBeFalse();
      }
   }

   public class When_checking_if_a_double_type_is_an_integer_type : concern_for_type_extensions
   {
      [Observation]
      public void should_return_false()
      {
         typeof(double).IsInteger().ShouldBeFalse();
      }
   }

   public class When_checking_if_a_double_type_is_an_nuemric_type : concern_for_type_extensions
   {
      [Observation]
      public void should_return_true()
      {
         typeof(double).IsNumeric().ShouldBeTrue();
      }
   }

   public class When_checking_if_a_double_type_is_an_double_type : concern_for_type_extensions
   {
      [Observation]
      public void should_return_true()
      {
         typeof(double).IsDouble().ShouldBeTrue();
      }
   }

   public class When_checking_if_an_abstract_type_is_an_abstract_class : concern_for_type_extensions
   {
      [Observation]
      public void should_return_true()
      {
         typeof(Notifier).IsAbstractClass().ShouldBeTrue();
      }
   }

   public class When_checking_if_an_interface_type_is_an_abstract_class : concern_for_type_extensions
   {
      [Observation]
      public void should_return_true()
      {
         typeof(IAnInterface).IsAbstractClass().ShouldBeTrue();
      }
   }

   public class When_checking_if_an_concrete_class_type_is_an_abstract_class : concern_for_type_extensions
   {
      [Observation]
      public void should_return_false()
      {
         typeof(AnImplementation).IsAbstractClass().ShouldBeFalse();
      }
   }

   public class When_instantiating_a_type_using_a_parameter_less_constructor : concern_for_type_extensions
   {
      [Observation]
      public void should_return_an_instance_of_that_type_for_a_type_with_a_parameter_less_constructor()
      {
         typeof(NotifyList<double>).CreateInstance().ShouldNotBeNull();
      }

      [Observation]
      public void should_return_an_instance_of_that_type_for_a_type_with_a_parameter_less_constructor_when_used_with_generics()
      {
         typeof(NotifyList<double>).CreateInstance<NotifyList<double>>().ShouldNotBeNull();
      }

      [Observation]
      public void should_throw_an_exception_when_using_an_abstract_type()
      {
         The.Action(() => typeof(Notifier).CreateInstance()).ShouldThrowAn<MissingParameterLessConstructorException>();
      }

      [Observation]
      public void should_throw_an_exception_when_using_an_interface()
      {
         The.Action(() => typeof(IAnInterface).CreateInstance()).ShouldThrowAn<MissingParameterLessConstructorException>();
      }

      [Observation]
      public void should_throw_an_exception_when_using_a_type_with_no_parmaeter_less_constructor()
      {
         The.Action(() => typeof(AssemblyScanner).CreateInstance()).ShouldThrowAn<MissingParameterLessConstructorException>();
      }
   }

   public class When_instantiating_a_type_using_a_constructor_with_parameter : concern_for_type_extensions
   {
      [Observation]
      public void should_return_an_instance_of_that_type_for_a_type_with_a_parameter_less_constructor()
      {
         typeof(NotifyList<double>).CreateInstanceUsing().ShouldNotBeNull();
      }

      [Observation]
      public void should_return_an_instance_of_that_type_for_a_matching_number_of_parameters()
      {
         typeof(AssemblyScanner).CreateInstanceUsing(A.Fake<IContainer>()).ShouldNotBeNull();
      }

      [Observation]
      public void should_return_an_instance_of_that_type_for_a_matching_number_of_parameters_when_used_with_generics()
      {
         typeof(AssemblyScanner).CreateInstanceUsing<AssemblyScanner>(A.Fake<IContainer>()).ShouldNotBeNull();
      }

      [Observation]
      public void should_throw_an_exception_if_no_matching_constructor_was_found()
      {
         The.Action(() => typeof(AssemblyScanner).CreateInstanceUsing(A.Fake<IContainer>(), new AnImplementation()))
            .ShouldThrowAn<MissingConstructorWithParametersException>();
      }
   }

   public class When_retrieving_the_assembly_name_from_a_given_type : concern_for_type_extensions
   {
      [Observation]
      public void should_return_the_name_of_the_assembly()
      {
         typeof(AssemblyScanner).AssemblyName().ShouldBeEqualTo("OSPSuite.Utility");
      }
   }
}