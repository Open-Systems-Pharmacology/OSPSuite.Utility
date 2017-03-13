using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_Cache : ContextSpecification<ICache<string, IAnInterface>>
   {
      protected override void Context()
      {
         sut = new Cache<string, IAnInterface>(o => o.FirstName);
      }
   }

   public class When_adding_an_object_to_the_cache_by_key : concern_for_Cache
   {
      private AnImplementation _objectToAdd;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
      }

      protected override void Because()
      {
         sut.Add(_objectToAdd);
      }

      [Observation]
      public void should_be_able_to_retrieve_that_object_by_key()
      {
         sut[_objectToAdd.FirstName].ShouldBeEqualTo(_objectToAdd);
      }
   }

   public class When_retrieving_the_key_used_to_register_the_object : concern_for_Cache
   {
      private AnImplementation _objectToAdd;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd);
      }

      [Observation]
      public void should_be_able_to_retrieve_that_object_by_key()
      {
         sut.Keys.ShouldOnlyContainInOrder(_objectToAdd.FirstName);
      }
   }

   public class When_adding_an_object_to_the_cache_by_key_that_already_exists : concern_for_Cache
   {
      private IAnInterface _objectToAdd;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.Add(_objectToAdd)).ShouldThrowAn<Exception>();
      }
   }

   public class When_asking_if_the_cache_contains_an_object_by_key_with_a_null_key : concern_for_Cache
   {
      [Observation]
      public void should_return_false()
      {
         sut.Contains(null).ShouldBeFalse();
      }
   }

   public class When_retrieving_all_values_defined_in_the_cache : concern_for_Cache
   {
      private IAnInterface _objectToAdd1;
      private IAnInterface _objectToAdd2;

      protected override void Context()
      {
         base.Context();
         _objectToAdd1 = new AnImplementation {FirstName = "toto"};
         _objectToAdd2 = new AnImplementation {FirstName = "tata"};
         sut.Add(_objectToAdd1);
         sut.Add(_objectToAdd2);
      }

      [Observation]
      public void should_return_the_values_registered_in_the_cache()
      {
         sut.ShouldOnlyContainInOrder(_objectToAdd1, _objectToAdd2);
      }
   }

   public class When_retrieving_a_vale_for_a_key_that_was_not_registered : concern_for_Cache
   {
      [Observation]
      public void should_throw_a_key_not_found_exception()
      {
         The.Action(() =>
         {
            var val = sut["titi"];
         }).ShouldThrowAn<KeyNotFoundException>();
      }
   }

   public class When_retrieving_a_value_for_a_key_that_was_not_registered_but_a_default_missing_function_was_provided : concern_for_Cache
   {
      private IAnInterface _defaultValue;

      protected override void Context()
      {
         base.Context();
         _defaultValue = A.Fake<IAnInterface>();
         sut = new Cache<string, IAnInterface>(o => o.FirstName, s => _defaultValue);
      }

      [Observation]
      public void should_return_the_value_defined_by_the_default_missing_function()
      {
         sut["titi"].ShouldBeEqualTo(_defaultValue);
      }
   }

   public class When_asked_if_it_has_a_given_key : concern_for_Cache
   {
      private IAnInterface _objectToAdd;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd);
      }

      [Observation]
      public void should_return_true_for_a_registered_key()
      {
         sut.Contains("toto").ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_for_a_registered_that_was_not_registered()
      {
         sut.Contains("titi").ShouldBeFalse();
      }
   }

   public class When_removing_an_object_by_key : concern_for_Cache
   {
      private IAnInterface _objectToAdd;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd);
      }

      protected override void Because()
      {
         sut.Remove("tata");
      }

      [Observation]
      public void should_not_contain_the_removed_element()
      {
         sut.Contains("tata").ShouldBeFalse();
      }
   }

   public class When_clearing_the_cache : concern_for_Cache
   {
      private IAnInterface _objectToAdd;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd);
      }

      protected override void Because()
      {
         sut.Clear();
      }

      [Observation]
      public void should_not_contain_any_element()
      {
         sut.Contains("tata").ShouldBeFalse();
      }
   }

   public class When_the_cache_is_created_without_any_key_delegate : concern_for_Cache
   {
      private IAnInterface _objectToAdd;

      protected override void Context()
      {
         base.Context();
         sut = new Cache<string, IAnInterface>();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
      }

      [Observation]
      public void should_crash_when_trying_to_add_an_element_without_supplying_a_key()
      {
         The.Action(() => sut.Add(_objectToAdd)).ShouldThrowAn<InvalidOperationException>();
      }
   }

   public class When_setting_a_value_by_key_with_a_key_that_was_already_registered : concern_for_Cache
   {
      private IAnInterface _objectToAdd;
      private IAnInterface _anotherObject;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd);
         _anotherObject = new AnImplementation();
      }

      protected override void Because()
      {
         sut["toto"] = _anotherObject;
      }

      [Observation]
      public void should_update_the_value_for_that_key()
      {
         sut["toto"].ShouldBeEqualTo(_anotherObject);
      }
   }

   public class When_setting_a_value_by_key_with_a_key_that_was_never_registered : concern_for_Cache
   {
      private IAnInterface _objectToAdd;
      private IAnInterface _anotherObject;

      protected override void Context()
      {
         base.Context();
         _objectToAdd = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd);
         _anotherObject = new AnImplementation();
      }

      protected override void Because()
      {
         sut["trililili"] = _anotherObject;
      }

      [Observation]
      public void should_add_a_new_value_for_that_key()
      {
         sut["trililili"].ShouldBeEqualTo(_anotherObject);
      }
   }

   public class When_adding_an_element_with_a_key_that_already_exists : concern_for_Cache
   {
      private IAnInterface _objectToAdd1;
      private bool _exceptionThrown;
      private string _exceptionMessage;

      protected override void Context()
      {
         base.Context();
         _objectToAdd1 = new AnImplementation {FirstName = "toto"};
         sut.Add(_objectToAdd1);
      }

      protected override void Because()
      {
         try
         {
            sut.Add(_objectToAdd1);
         }
         catch (ArgumentException e)
         {
            _exceptionThrown = true;
            _exceptionMessage = e.Message;
         }
      }

      [Observation]
      public void should_throw_an_exception_that_contains_the_key_in_the_error_message()
      {
         _exceptionThrown.ShouldBeTrue();
         _exceptionMessage.Contains(_objectToAdd1.FirstName).ShouldBeTrue();
      }
   }

   public class When_adding_a_range_of_new_objects : concern_for_Cache
   {
      private IEnumerable<IAnInterface> _someImplementations;
      private IAnInterface _totoImplementation;
      private IAnInterface _harryImplementation;

      protected override void Context()
      {
         base.Context();
         _totoImplementation = new AnImplementation {FirstName = "Toto"};
         _harryImplementation = new AnImplementation {FirstName = "Harry"};
         _someImplementations = new Collection<IAnInterface>
         {
            _totoImplementation,
            _harryImplementation
         };
      }

      protected override void Because()
      {
         sut.AddRange(_someImplementations);
      }

      [Observation]
      public void Should_be_able_to_retrive_the_objects_by_key()
      {
         sut["Toto"].ShouldBeEqualTo(_totoImplementation);
         sut["Harry"].ShouldBeEqualTo(_harryImplementation);
      }
   }

   public class When_adding_a_range_of_new_objects_mapping_to_the_same_key : concern_for_Cache
   {
      private IEnumerable<IAnInterface> _someImplementations;
      private IAnInterface _totoImplementation;
      private IAnInterface _harryImplementation;
      private bool _exceptionThrown;
      private string _exceptionMessage;

      protected override void Context()
      {
         base.Context();
         _totoImplementation = new AnImplementation {FirstName = "Toto"};
         _harryImplementation = new AnImplementation {FirstName = "Toto"};
         _someImplementations = new Collection<IAnInterface>
         {
            _totoImplementation,
            _harryImplementation
         };
      }

      protected override void Because()
      {
         try
         {
            sut.AddRange(_someImplementations);
         }
         catch (ArgumentException e)
         {
            _exceptionThrown = true;
            _exceptionMessage = e.Message;
         }
      }

      [Observation]
      public void Should_be_able_to_retrive_the_first_objects_by_key()
      {
         sut["Toto"].ShouldBeEqualTo(_totoImplementation);
      }

      [Observation]
      public void should_throw_an_exception_that_contains_the_key_in_the_error_message()
      {
         _exceptionThrown.ShouldBeTrue();
         _exceptionMessage.Contains(_harryImplementation.FirstName).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_list_of_key_value_pairs_defined_in_the_cache : concern_for_Cache
   {
      private IAnInterface _objectToAdd1;
      private IAnInterface _objectToAdd2;
      private IEnumerable<KeyValuePair<string, IAnInterface>> _keyValues;

      protected override void Context()
      {
         base.Context();
         _objectToAdd1 = new AnImplementation {FirstName = "toto"};
         _objectToAdd2 = new AnImplementation {FirstName = "tata"};
         sut.Add(_objectToAdd1);
         sut.Add(_objectToAdd2);
      }

      protected override void Because()
      {
         _keyValues = sut.KeyValues;
      }

      [Observation]
      public void should_return_all_the_key_values_pairs_available()
      {
         _keyValues.Count().ShouldBeEqualTo(2);
         _keyValues.ElementAt(0).Key.ShouldBeEqualTo(_objectToAdd1.FirstName);
         _keyValues.ElementAt(0).Value.ShouldBeEqualTo(_objectToAdd1);
         _keyValues.ElementAt(1).Key.ShouldBeEqualTo(_objectToAdd2.FirstName);
         _keyValues.ElementAt(1).Value.ShouldBeEqualTo(_objectToAdd2);
      }
   }
}