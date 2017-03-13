using System;
using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Visitor;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_AmbiguousVisitMethodException : ContextSpecification<Exception>
   {
      private Type _typeOfObjectToVisit;
      private Type _typeOfVisitor;
      private IList<TypeForGenericType> _allImplementations;

      protected override void Context()
      {
         _typeOfObjectToVisit = A.Fake<Type>();
         _typeOfVisitor = A.Fake<Type>();
         _allImplementations = new List<TypeForGenericType> {new TypeForGenericType(_typeOfVisitor, _typeOfObjectToVisit)};

         sut = new AmbiguousVisitMethodException(_allImplementations, _typeOfVisitor, _typeOfObjectToVisit);
      }
   }

   public class When_creating_a_unbiguous_visit_methods_exception : concern_for_AmbiguousVisitMethodException
   {
      [Observation]
      public void the_message_should_not_be_empty()
      {
         sut.Message.Length.ShouldNotBeNull();
      }
   }
}