using System;
using System.Collections.Specialized;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_NotifyList : ContextSpecification<INotifyList<IAnInterface>>
   {
      protected IAnInterface _implementation0;
      protected IAnInterface _implementation1;
      protected IAnInterface _implementation2;
      protected bool _eventWasRaised;
      protected NotifyCollectionChangedAction _action;
      protected NotifyCollectionChangedEventArgs _changeEventArgs;

      protected override void Context()
      {
         sut = new NotifyList<IAnInterface>();
         _implementation0 = new AnImplementation {FirstName = "aaa"};
         _implementation1 = new AnImplementation {FirstName = "toto"};
         _implementation2 = new AnImplementation {FirstName = "tata"};
         sut.Add(_implementation0);
         sut.Add(_implementation1);
         sut.Add(_implementation2);

         sut.CollectionChanged += (o, e) =>
         {
            _eventWasRaised = true;
            _action = e.Action;
            _changeEventArgs = e;
         };
      }
   }

   public class When_adding_an_item_to_the_list : concern_for_NotifyList
   {
      private IAnInterface _implementation3;

      protected override void Context()
      {
         base.Context();
         _implementation3 = new AnImplementation();
      }

      protected override void Because()
      {
         sut.Add(_implementation3);
      }

      [Observation]
      public void should_notify_a_property_change_event()
      {
         _eventWasRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_notified_action_should_be_of_type_add()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Add);
      }

      [Observation]
      public void should_notify_the_added_element()
      {
         _changeEventArgs.NewItems[0].ShouldBeEqualTo(_implementation3);
      }
   }

   public class When_clearing_the_list : concern_for_NotifyList
   {
      protected override void Because()
      {
         sut.Clear();
      }

      [Observation]
      public void should_notify_a_property_change_event()
      {
         _eventWasRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_notified_action_should_be_of_type_reset()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Reset);
      }
   }

   public class When_removing_an_element : concern_for_NotifyList
   {
      protected override void Because()
      {
         sut.Remove(_implementation1);
      }

      [Observation]
      public void should_notify_a_property_change_event()
      {
         _eventWasRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_notified_action_should_be_of_type_remove()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Remove);
      }

      [Observation]
      public void the_nofified_removed_item_should_be_the_item_removed()
      {
         _changeEventArgs.OldItems[0].ShouldBeEqualTo(_implementation1);
      }
   }

   public class When_removing_an_element_at_a_certain_index : concern_for_NotifyList
   {
      protected override void Because()
      {
         sut.RemoveAt(1);
      }

      [Observation]
      public void should_notify_a_property_change_event()
      {
         _eventWasRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_notified_action_should_be_of_type_remove()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Remove);
      }

      [Observation]
      public void the_nofified_removed_item_should_be_the_item_removed()
      {
         _changeEventArgs.OldItems[0].ShouldBeEqualTo(_implementation1);
      }

      [Observation]
      public void the_nofified_index_should_be_the_one_of_the_removed_item()
      {
         _changeEventArgs.OldStartingIndex.ShouldBeEqualTo(1);
      }
   }

   public class When_inserting_an_element_at_a_defined_position : concern_for_NotifyList
   {
      private IAnInterface _implementation4;

      protected override void Context()
      {
         base.Context();
         _implementation4 = new AnImplementation();
      }

      protected override void Because()
      {
         sut.Insert(1, _implementation4);
      }

      [Observation]
      public void should_notify_a_property_change_event()
      {
         _eventWasRaised.ShouldBeTrue();
      }

      [Observation]
      public void the_notified_action_should_be_of_type_add()
      {
         _action.ShouldBeEqualTo(NotifyCollectionChangedAction.Add);
      }

      [Observation]
      public void the_nofified_added_item_should_be_the_item_inserted()
      {
         _changeEventArgs.NewItems[0].ShouldBeEqualTo(_implementation4);
      }

      [Observation]
      public void the_nofified_new_index_should_be_the_index_where_the_insert_took_place()
      {
         _changeEventArgs.NewStartingIndex.ShouldBeEqualTo(1);
      }
   }

   public class when_remove_an_element_from_the_notify_list_that_implements_the_notifiable_property : concern_for_NotifyList
   {
      private WeakReference _reference;

      protected override void Context()
      {
         sut = new NotifyList<IAnInterface>();
         _implementation0 = new AnImplementation {FirstName = "aaa"};
         sut.Add(_implementation0);
         _reference = new WeakReference(_implementation0);
      }

      protected override void Because()
      {
         sut.Remove(_implementation0);
         _implementation0 = null;
         GC.Collect();
      }

      [Observation]
      public void the_element_should_be_released_from_the_memory()
      {
         _reference.IsAlive.ShouldBeFalse();
      }
   }
}