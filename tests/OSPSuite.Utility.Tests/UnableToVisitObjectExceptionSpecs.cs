using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Visitor;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_unable_to_visit_object_exception : ContextSpecification<UnableToVisitObjectException>
   {
      protected Type _typeOfObjectToVisit;
      protected Type _typeOfVisitor;
      protected MyComplexVisitor _visitor;

      protected override void Context()
      {
         _visitor = new MyComplexVisitor();
         sut = new UnableToVisitObjectException(_visitor, typeof(IAnotherInteface));
      }
   }

   public class When_creating_a_unable_to_visit_methods_exception : concern_for_unable_to_visit_object_exception
   {
      [Observation]
      public void the_message_should_not_be_empty()
      {
         sut.Message.Length.ShouldNotBeNull();
      }
   }
}