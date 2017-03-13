using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public class When_retrieving_the_string_expression_of_a_list_of_object : StaticContextSpecification
   {
      private IList<string> _list;
      private string _result1;
      private string _result2;

      [Observation]
      public void should_return_the_expected_string()
      {
         _result1.ShouldBeEqualTo("toto:tata:titi");
         _result2.ShouldBeEqualTo("'toto';'tata';'titi'");
      }

      protected override void Because()
      {
         _result1 = _list.ToString(":");
         _result2 = _list.ToString(";", "'");
      }

      protected override void Context()
      {
         _list = new List<string> {"toto", "tata", "titi"};
      }
   }

   public class When_using_the_each_with_index_method : StaticContextSpecification
   {
      private List<string> _list;
      private readonly Cache<string, int> _cache = new Cache<string, int>();

      protected override void Context()
      {
         _list = new List<string>();
         _list.Add("String1");
         _list.Add("String2");
      }

      protected override void Because()
      {
         _list.Each((item, index) => _cache.Add(item, index));
      }

      [Observation]
      public void should_iterate_over_all_items_using_their_accurage_indexes()
      {
         _cache["String1"].ShouldBeEqualTo(0);
         _cache["String2"].ShouldBeEqualTo(1);
      }
   }
}