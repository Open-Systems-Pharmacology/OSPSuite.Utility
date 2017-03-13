using System;
using System.ComponentModel;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Visitor;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_VisitorInvoker : StaticContextSpecification
   {
   }

   public class When_visiting_a_visitor_for_an_object_for_which_a_matching_implementation_of_visit_already_exists : concern_for_VisitorInvoker
   {
      private MySimpleVisitor _visitor;
      private MySimpleClass _mySimpleClass;

      protected override void Context()
      {
         base.Context();
         _visitor = new MySimpleVisitor();
         _mySimpleClass = new MySimpleClass();
      }

      protected override void Because()
      {
         VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass);
      }

      [Observation]
      public void should_call_that_implementation()
      {
         _mySimpleClass.SimpleVisited.ShouldBeTrue();
         _mySimpleClass.BaseVisited.ShouldBeFalse();
      }
   }

   public class When_visiting_a_visitor_for_an_object_for_which_an_implementation_of_visit_exists_with_the_convention_type : concern_for_VisitorInvoker
   {
      private MyComplexVisitor _visitor;
      private MySimpleClass _mySimpleClass;

      protected override void Context()
      {
         base.Context();
         _visitor = new MyComplexVisitor();
         _mySimpleClass = new MySimpleClass();
      }

      protected override void Because()
      {
         VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass);
      }

      [Observation]
      public void should_call_that_implementation()
      {
         _mySimpleClass.SimpleVisited.ShouldBeTrue();
         _mySimpleClass.BaseVisited.ShouldBeFalse();
      }
   }

   public class When_visiting_a_visitor_for_an_object_for_which_only_an_implementation_of_visit_for_one_of_its_base_interface_exists : concern_for_VisitorInvoker
   {
      private MyInterfaceBaseVisitor _visitor;
      private MySimpleClass _mySimpleClass;

      protected override void Context()
      {
         base.Context();
         _mySimpleClass = new MySimpleClass();
         _visitor = new MyInterfaceBaseVisitor();
      }

      protected override void Because()
      {
         VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass);
      }

      [Observation]
      public void should_call_the_base_implementation()
      {
         _mySimpleClass.SimpleVisited.ShouldBeFalse();
         _mySimpleClass.BaseVisited.ShouldBeTrue();
      }
   }

   public class When_visiting_a_null_visitor : concern_for_VisitorInvoker
   {
      private MyInterfaceBaseVisitor _visitor;
      private MySimpleClass _mySimpleClass;

      protected override void Context()
      {
         base.Context();
         _mySimpleClass = new MySimpleClass();
         _visitor = null;
      }

      [Observation]
      public void should_not_crash()
      {
         VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass);
      }
   }

   public class When_visiting_a_visitor_for_an_object_for_which_only_an_implementation_of_one_of_its_base_class_exists : concern_for_VisitorInvoker
   {
      private MyBaseVisitor _visitor;
      private MySimpleClass _mySimpleClass;

      protected override void Context()
      {
         base.Context();
         _visitor = new MyBaseVisitor();
         _mySimpleClass = new MySimpleClass();
      }

      protected override void Because()
      {
         VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass);
         VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass);
      }

      [Observation]
      public void should_call_the_base_implementation()
      {
         _mySimpleClass.SimpleVisited.ShouldBeFalse();
         _mySimpleClass.BaseVisited.ShouldBeTrue();
      }
   }

   public class When_visiting_a_strict_visitor_for_an_object_for_which_no_valid_implementation_exists : concern_for_VisitorInvoker
   {
      private MyComplexVisitor _visitor;
      private MyVisitableClass _myVisitableClass;

      protected override void Context()
      {
         base.Context();
         _visitor = new MyComplexVisitor();
         _myVisitableClass = new MyVisitableClass();
      }

      [Observation]
      public void should_do_nothing()
      {
         The.Action(() => VisitorInvoker.InvokeVisit(_visitor, _myVisitableClass)).ShouldThrowAn<UnableToVisitObjectException>();
      }
   }

   public class When_visiting_a_visitor_for_an_object_implementing_a_generic_type : concern_for_VisitorInvoker
   {
      private IVisitor<IMyGenericClass<IMySimpleClass>> _visitor;
      private MyGenericClass<IMySimpleClass> _myGenericClass;

      protected override void Context()
      {
         base.Context();
         _visitor = new MyGenericVisitor();
         _myGenericClass = new MyGenericClass<IMySimpleClass>();
      }

      protected override void Because()
      {
         VisitorInvoker.InvokeVisit(_visitor, _myGenericClass);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_valid_implementation_of_the_validate_method()
      {
         _myGenericClass.Visited.ShouldBeTrue();
      }
   }

   public class When_visiting_a_visitor_for_an_object_for_which_no_direct_implementation_exists_but_only_base_implementation_remains_in_the_hierarchy : concern_for_VisitorInvoker
   {
      private MyHierarchicalVisitor _visitor;
      private MySimpleClass _mySimpleClass;

      protected override void Context()
      {
         base.Context();
         _visitor = new MyHierarchicalVisitor();
         _mySimpleClass = new MySimpleClass();
      }

      protected override void Because()
      {
         VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass);
      }

      [Observation]
      public void should_visit_the_visitee_with_the_visit_function_matching_the_hierarchy_in_the_visitor()
      {
         _mySimpleClass.BaseVisited.ShouldBeFalse();
         _mySimpleClass.EntityVisited.ShouldBeTrue();
         _mySimpleClass.SimpleVisited.ShouldBeFalse();
      }
   }

   public class When_visiting_a_visitor_for_an_object_for_which_no_direct_implementation_exists_and_two_or_more_base_implementation_exist : concern_for_VisitorInvoker
   {
      private MyStupidVisitor _visitor;
      private MySimpleClass _mySimpleClass;

      protected override void Context()
      {
         base.Context();
         _visitor = new MyStupidVisitor();
         _mySimpleClass = new MySimpleClass();
      }

      [Observation]
      public void should_trow_an_exception()
      {
         The.Action(() => VisitorInvoker.InvokeVisit(_visitor, _mySimpleClass)).ShouldThrowAn<AmbiguousVisitMethodException>();
      }
   }

   public class MyEmptyVisitor : IVisitor
   {
   }

   public class MyBaseVisitor : IVisitor<MyBaseClass>
   {
      public void Visit(MyBaseClass objToVisit)
      {
         objToVisit.BaseVisited = true;
      }

      public void Process(MyBaseClass objToProcess)
      {
         objToProcess.BaseProcessed = true;
      }
   }

   public class MyInterfaceBaseVisitor : IVisitor<IMyBaseClass>
   {
      public void Visit(IMyBaseClass objToVisit)
      {
         objToVisit.BaseVisited = true;
      }

      public void Process(IMyBaseClass objToProcess)
      {
         objToProcess.BaseProcessed = true;
      }
   }

   public class MySimpleVisitor : IVisitor<MySimpleClass>
   {
      public void Visit(MySimpleClass objToVisit)
      {
         objToVisit.SimpleVisited = true;
      }

      public void Process(MySimpleClass objToProcess)
      {
         objToProcess.SimpleProcessed = true;
      }
   }

   public class MyComplexVisitor : IVisitor<IMySimpleClass>, IVisitor<IMyBaseClass>, IStrictVisitor
   {
      public void Visit(IMySimpleClass objToVisit)
      {
         objToVisit.SimpleVisited = true;
      }

      public void Process(IMySimpleClass objToProcess)
      {
         objToProcess.SimpleProcessed = true;
      }

      public void Visit(IMyBaseClass objToVisit)
      {
         objToVisit.BaseVisited = true;
      }

      public void Process(IMyBaseClass objToProcess)
      {
         objToProcess.BaseProcessed = true;
      }
   }

   public class MyGenericVisitor : IVisitor<IMyGenericClass<IMySimpleClass>>
   {
      public void Visit(IMyGenericClass<IMySimpleClass> objToVisit)
      {
         objToVisit.Visited = true;
      }
   }

   public class MyHierarchicalVisitor : IVisitor<IMyBaseClass>, IVisitor<IMyEntity>
   {
      public void Visit(IMyBaseClass objToVisit)
      {
         objToVisit.BaseVisited = true;
      }

      public void Process(IMyBaseClass objToProcess)
      {
         objToProcess.BaseProcessed = true;
      }

      public void Visit(IMyEntity objToVisit)
      {
         objToVisit.EntityVisited = true;
      }

      public void Process(IMyEntity objToProcess)
      {
         objToProcess.EntityPocessed = true;
      }
   }

   public class MyStupidVisitor : IVisitor<IMyEntity>, IVisitor<IAnotherInteface>
   {
      public void Visit(IMyEntity objToVisit)
      {
         objToVisit.EntityVisited = true;
      }

      public void Process(IMyEntity objToProcess)
      {
         objToProcess.EntityPocessed = true;
      }

      public void Visit(IAnotherInteface objToVisit)
      {
         objToVisit.FirstName = "tralala";
      }

      public void Process(IAnotherInteface objToProcess)
      {
         objToProcess.FirstName = "tralala";
      }
   }

   public interface IMyGenericClass<T>
   {
      bool Visited { get; set; }
   }

   public class MyGenericClass<T> : IMyGenericClass<T>
   {
      public bool Visited { get; set; }
   }

   public interface IMySimpleClass : IMyEntity
   {
      bool SimpleVisited { get; set; }
      bool SimpleProcessed { get; set; }
   }

   public interface IMyAnotherSimpleClass : IMyEntity
   {
   }

   public class MySimpleClass : MyEntity, IMySimpleClass, IAnotherInteface
   {
      public bool SimpleVisited { get; set; }
      public bool SimpleProcessed { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public IAnInterface Child { get; set; }
      public string ReadOnlyProp { get; private set; }
      public event PropertyChangedEventHandler PropertyChanged = delegate { };
      public event Action<object> Changed = delegate { };
   }

   public interface IMyBaseClass : IVisitable<IVisitor>
   {
      bool BaseVisited { get; set; }
      bool BaseProcessed { get; set; }
   }

   public interface IMyEntity : IMyBaseClass
   {
      bool EntityVisited { get; set; }
      bool EntityPocessed { get; set; }
   }

   public class MyEntity : MyBaseClass, IMyEntity
   {
      public bool EntityVisited { get; set; }
      public bool EntityPocessed { get; set; }
   }

   public class MyBaseClass : IMyBaseClass
   {
      public void AcceptVisitor(IVisitor visitor)
      {
         visitor.Visit(this);
      }

      public bool BaseVisited { get; set; }
      public bool BaseProcessed { get; set; }
   }

   public class MyVisitableClass : IVisitable<IVisitor>
   {
      public void AcceptVisitor(IVisitor visitor)
      {
         visitor.Visit(this);
      }
   }
}