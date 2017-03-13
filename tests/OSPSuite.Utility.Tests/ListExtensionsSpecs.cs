using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public class When_checkign_if_two_lists_of_user_defined_objects_are_equals : StaticContextSpecification
   {
      private IList<IAnInterface> _list1;
      private IList<IAnInterface> _list2;
      private IList<IAnInterface> _list3;
      private IList<IAnInterface> _list4;

      protected override void Context()
      {
         var obj1 = new AnImplementation();
         var obj2 = new AnImplementation();
         var obj3 = new AnImplementation();
         _list1 = new List<IAnInterface> {obj1, obj2};
         _list2 = new List<IAnInterface> {obj1, obj2};
         _list3 = new List<IAnInterface> {obj1};
         _list4 = new List<IAnInterface> {obj1, obj3};
      }

      [Observation]
      public void should_return_true_if_the_list_contains_the_same_objects()
      {
         _list1.ListEquals(_list2).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         _list1.ListEquals(_list3).ShouldBeFalse();
         _list1.ListEquals(_list4).ShouldBeFalse();
         _list4.ListEquals(_list3).ShouldBeFalse();
      }
   }
}