using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Visitor;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_type_simplifier_specs : ContextSpecification<ITypeSimplifier>
   {
      protected IList<Type> _listToSimplify;
      protected IEnumerable<Type> _result;

      protected override void Context()
      {
         _listToSimplify = new List<Type>();
         sut = new TypeSimplifier();
      }

      protected override void Because()
      {
         _result = sut.Simplify(_listToSimplify);
      }
   }

   public class when_simplifying_an_empty_list_of_type : concern_for_type_simplifier_specs
   {
      [Observation]
      public void should_return_an_empty_list_of_type()
      {
         _result.Count().ShouldBeEqualTo(0);
      }
   }

   public class when_simplifying_a_list_containing_only_one_type : concern_for_type_simplifier_specs
   {
      private Type _type;

      protected override void Context()
      {
         base.Context();
         _type = typeof(double);
         _listToSimplify.Add(_type);
      }

      [Observation]
      public void should_return_a_list_containg_that_one_type()
      {
         _result.ShouldOnlyContain(_type);
      }
   }

   public class when_simplifying_a_list_containing_only_types_that_can_simplify_themselves : concern_for_type_simplifier_specs
   {
      protected override void Context()
      {
         base.Context();
         _listToSimplify = new List<Type> {typeof(IMySimpleClass), typeof(IMyBaseClass), typeof(IMyEntity)};
      }

      [Observation]
      public void should_return_a_list_containg_only_the_lowest_type_in_the_hierarchy()
      {
         _result.ShouldOnlyContain(typeof(IMySimpleClass));
      }
   }

   public class when_simplifying_a_list_containing_types_that_cannot_simplify_themselves : concern_for_type_simplifier_specs
   {
      protected override void Context()
      {
         base.Context();
         _listToSimplify = new List<Type> {typeof(IMySimpleClass), typeof(IMyAnotherSimpleClass), typeof(IMyEntity)};
      }

      [Observation]
      public void should_return_a_list_containing_the_type_that_cannot_be_simplified()
      {
         _result.ShouldOnlyContain(typeof(IMySimpleClass), typeof(IMyAnotherSimpleClass));
      }
   }

   public class when_simplifying_a_list_containing_two_same_types : concern_for_type_simplifier_specs
   {
      protected override void Context()
      {
         base.Context();
         _listToSimplify = new List<Type> {typeof(IMySimpleClass), typeof(IMySimpleClass)};
      }

      [Observation]
      public void should_return_a_list_containg_only_one_type()
      {
         _result.ShouldOnlyContain(typeof(IMySimpleClass));
      }
   }

   public class when_simplifying_a_list_of_type_for_generic_that_can_simplify_themselves : concern_for_type_simplifier_specs
   {
      private List<TypeForGenericType> _genericTypeToSimplify;
      private IEnumerable<TypeForGenericType> _simplifiedReults;
      private TypeForGenericType _goodGenericType;

      protected override void Context()
      {
         base.Context();
         _goodGenericType = new TypeForGenericType(typeof(IMySimpleClass), typeof(IVisitor<IMySimpleClass>));
         _genericTypeToSimplify = new List<TypeForGenericType>
         {
            _goodGenericType,
            new TypeForGenericType(typeof(IMyBaseClass), typeof(IVisitor<IMyBaseClass>)),
            new TypeForGenericType(typeof(IMySimpleClass), typeof(IVisitor<IMySimpleClass>)),
            new TypeForGenericType(typeof(IMyEntity), typeof(IVisitor<IMyEntity>)),
            new TypeForGenericType(typeof(IMyEntity), typeof(IVisitor<IMyEntity>)),
         };
      }

      protected override void Because()
      {
         _simplifiedReults = sut.Simplify(_genericTypeToSimplify);
      }

      [Observation]
      public void should_return_a_list_containg_only_the_lowest_type_in_the_hierarchy()
      {
         _simplifiedReults.ShouldOnlyContain(_goodGenericType);
      }
   }
}