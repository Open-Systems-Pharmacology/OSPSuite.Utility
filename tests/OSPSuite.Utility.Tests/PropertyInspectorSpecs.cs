using System;
using System.Reflection;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_property_inspector<TypeOfParameter> : ContextSpecification<IExpressionInspector<TypeOfParameter>>
   {
      protected override void Context()
      {
         sut = new ExpressionInspector<TypeOfParameter>();
      }
   }

   public class When_inspecting_a_property_for_a_given_object : concern_for_property_inspector<IAnInterface>
   {
      private PropertyInfo _result;

      [Observation]
      public void should_return_a_property_info_describing_that_property()
      {
         _result.ShouldNotBeNull();
         _result.Name.ShouldBeEqualTo("FirstName");
      }

      protected override void Because()
      {
         _result = sut.PropertyFor(item => item.FirstName);
      }
   }

   public class When_inspecting_a_public_member_for_an_object : concern_for_property_inspector<AnImplementation>
   {
      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.PropertyFor(item => item.OnePublicMEmber)).ShouldThrowAn<ArgumentException>();
      }
   }
}