using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_NotifyCache : ContextSpecification<INotifyCache<string, IAnInterface>>
   {
      protected IAnInterface _implementation1;
      protected IAnInterface _implementation2;
      protected IAnInterface _implementation3;

      protected override void Context()
      {
         sut = new NotifyCache<string, IAnInterface>();
         _implementation1 = new AnImplementation();
         _implementation2 = new AnImplementation();
         _implementation3 = new AnImplementation();
         sut.Add("key1", _implementation1);
         sut.Add("key2", _implementation2);
         sut.Add("key3", _implementation3);
      }
   }

   public class When_retrieving_the_number_of_items_from_the_collection : concern_for_NotifyCache
   {
      [Observation]
      public void should_return_the_number_of_items_contained_in_the_collection()
      {
         sut.Count().ShouldBeEqualTo(3);
      }
   }

   public class When_retrieving_an_item_by_key : concern_for_NotifyCache
   {
      [Observation]
      public void should_return_a_valid_item_if_one_was_registered_for_that_key()
      {
         sut["key2"].ShouldBeEqualTo(_implementation2);
      }

      [Observation]
      public void should_throw_an_exception_if_not_element_was_registered_with_the_given_key()
      {
         The.Action(() =>
         {
            var x = sut["toto"];
         }).ShouldThrowAn<KeyNotFoundException>();
      }
   }

   public class When_performing_an_iteration_over_the_collection : concern_for_NotifyCache
   {
      private IEnumerable<IAnInterface> _results;

      protected override void Because()
      {
         _results = sut.All();
      }

      [Observation]
      public void should_return_all_the_registered_elements()
      {
         _results.ShouldOnlyContainInOrder(_implementation1, _implementation2, _implementation3);
      }
   }

   public class When_asked_if_a_key_is_contained : concern_for_NotifyCache
   {
      [Observation]
      public void should_return_true_for_an_existing_key()
      {
         sut.Contains("key1").ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_an_unknown_key()
      {
         sut.Contains("toto").ShouldBeFalse();
      }
   }

   public class When_removing_a_value_by_key : concern_for_NotifyCache
   {
      [Observation]
      public void should_not_crash_if_the_value_does_not_exist()
      {
         sut.Remove("toto");
      }

      [Observation]
      public void should_return_true_if_the_value_exists()
      {
         sut.Remove("key1");
         sut.Contains("key1").ShouldBeFalse();
      }
   }

   public class When_adding_an_item_to_the_collection : concern_for_NotifyCache
   {
      private NotifyCollectionChangedAction _action;
      private bool _eventRaised;
      private IAnInterface _implementation4;
      private object _addedObject;

      protected override void Because()
      {
         _implementation4 = new AnImplementation();
         sut.Add("tralala", _implementation4);
      }

      protected override void Context()
      {
         base.Context();
         sut.CollectionChanged += (o, e) =>
         {
            _eventRaised = true;
            _action = e.Action;
            _addedObject = e.NewItems[0];
         };
      }

      [Observation]
      public void should_notify_the_collection_changed_event()
      {
         _eventRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_action_type_should_be_add()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Add);
      }

      [Observation]
      public void the_new_items_should_be_equal_to_the_added_objects()
      {
         _addedObject.ShouldBeEqualTo(_implementation4);
      }
   }

   public class When_adding_an_item_to_the_collection_with_the_indexer_that_does_not_exist : concern_for_NotifyCache
   {
      private NotifyCollectionChangedAction _action;
      private bool _eventRaised;
      private IAnInterface _implementation4;

      protected override void Context()
      {
         base.Context();
         _implementation4 = new AnImplementation();
         sut.CollectionChanged += (o, e) =>
         {
            _eventRaised = true;
            _action = e.Action;
         };
      }

      protected override void Because()
      {
         sut["toto"] = _implementation4;
      }

      [Observation]
      public void should_notify_the_collection_changed_event()
      {
         _eventRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_action_type_should_be_add()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Add);
      }
   }

   public class When_adding_an_item_to_the_collection_with_the_indexer_that_already_exist : concern_for_NotifyCache
   {
      private NotifyCollectionChangedAction _action;
      private bool _eventRaised;
      private IAnInterface _implementation4;

      protected override void Context()
      {
         base.Context();
         _implementation4 = new AnImplementation();
         sut["toto"] = new AnImplementation();
         sut.CollectionChanged += (o, e) =>
         {
            _eventRaised = true;
            _action = e.Action;
         };
      }

      protected override void Because()
      {
         sut["toto"] = _implementation4;
      }

      [Observation]
      public void should_notify_the_collection_changed_event()
      {
         _eventRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_action_type_should_be_add()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Replace);
      }
   }

   public class When_removing_an_item_from_the_collection : concern_for_NotifyCache
   {
      private NotifyCollectionChangedAction _action;
      private bool _eventRaised;
      private object _removedObject;

      protected override void Because()
      {
         sut.Remove("key1");
      }

      protected override void Context()
      {
         base.Context();
         sut.CollectionChanged += (o, e) =>
         {
            _eventRaised = true;
            _action = e.Action;
            _removedObject = e.OldItems[0];
         };
      }

      [Observation]
      public void should_notify_the_collection_changed_event()
      {
         _eventRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_action_type_should_be_add()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Remove);
      }

      [Observation]
      public void the_new_items_should_be_equal_to_the_added_objects()
      {
         _removedObject.ShouldBeEqualTo(_implementation1);
      }
   }

   public class When_the_collection_is_being_cleared : concern_for_NotifyCache
   {
      private NotifyCollectionChangedAction _action;
      private bool _eventRaised;

      protected override void Context()
      {
         base.Context();
         sut.CollectionChanged += (o, e) =>
         {
            _eventRaised = true;
            _action = e.Action;
         };
      }

      protected override void Because()
      {
         sut.Clear();
      }

      [Observation]
      public void should_notify_the_collection_changed_event()
      {
         _eventRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_action_type_should_be_reset()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Reset);
      }
   }

   public class When_iterating_through_the_keys_registered_in_the_dictionary : concern_for_NotifyCache
   {
      [Observation]
      public void should_return_all_available_keys_in_the_dictionary()
      {
         sut.Keys.ShouldOnlyContainInOrder("key1", "key2", "key3");
      }
   }

   public class When_clearing_the_rich_dictionary : concern_for_NotifyCache
   {
      private bool _eventWasRaised;
      private NotifyCollectionChangedAction _value;

      protected override void Context()
      {
         base.Context();

         sut.CollectionChanged += (o, e) =>
         {
            _eventWasRaised = true;
            _value = e.Action;
         };
      }

      protected override void Because()
      {
         sut.Clear();
      }

      [Observation]
      public void should_raise_a_collection_changed_event_with_the_reset_flag()
      {
         _eventWasRaised.ShouldBeTrue();
         _value.ShouldBeEqualTo(NotifyCollectionChangedAction.Reset);
      }
   }
}