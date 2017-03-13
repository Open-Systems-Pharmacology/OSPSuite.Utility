using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_AssemblyExtensions : StaticContextSpecification
   {
      protected override void Context()
      {
      }
   }

   public class When_resolving_all_concrete_types_implementing_a_given_interface : concern_for_type_extensions
   {
      private IEnumerable<Type> _results;

      protected override void Because()
      {
         _results = typeof(IAnInterface).Assembly.GetAllConcreteTypesImplementing<IAnInterface>();
      }

      [Observation]
      public void should_return_all_the_availble_type_from_the_assembly()
      {
         _results.ShouldOnlyContain(typeof(AnImplementation), typeof(MySimpleClass),
            typeof(ADerivedImplementation), typeof(AnInternalClass),
            typeof(AnotherImplementation));
      }
   }

   public class When_resolving_only_the_exported_types_implementing_a_given_interface : concern_for_type_extensions
   {
      private IEnumerable<Type> _results;

      protected override void Because()
      {
         _results = typeof(AssemblyExtensions).Assembly.GetConcreteTypesImplementing<IAnInterface>(true);
      }

      [Observation]
      public void should_return_all_the_availble_type_from_the_assembly()
      {
         _results.ContainsItem(typeof(AnInternalClass)).ShouldBeFalse();
      }
   }
}