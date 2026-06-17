using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_BuilderRepository : ContextSpecification<IBuilderRepository<IMyBuilder>>
   {
      protected IContainer _container;
      protected List<IMyBuilder> _allTEXBuilder;

      protected override void Context()
      {
         _allTEXBuilder = new List<IMyBuilder>();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.ResolveAll<IMyBuilder>()).Returns(_allTEXBuilder);

         sut = new BuilderRepositoryForSpecs(_container);
      }
   }

   public class When_resolving_a_builder_for_a_type_that_was_not_registered : concern_for_BuilderRepository
   {
      [Observation]
      public void should_return_null()
      {
         sut.BuilderFor(new Parameter()).ShouldBeNull();
      }
   }

   public class When_resolving_a_builder_for_a_type_that_could_use_many_possible_builder : concern_for_BuilderRepository
   {
      protected override void Context()
      {
         base.Context();
         _allTEXBuilder.Add(new ParameterBuilder());
         _allTEXBuilder.Add(new TEXMySuperBuilder());
      }

      [Observation]
      public void should_use_the_most_accurate_builder()
      {
         sut.BuilderFor(new MySuperParameter()).ShouldBeAnInstanceOf<TEXMySuperBuilder>();
      }
   }

   public class When_resolving_a_builder_concurrently_for_the_same_type : concern_for_BuilderRepository
   {
      private List<Exception> _exceptions;
      private const int _threadCount = 16;
      private const int _attempts = 100;

      protected override void Context()
      {
         base.Context();
         // exactly one builder matches MySuperParameter, so BuilderFor takes the single-builder Add path
         _allTEXBuilder.Add(new TEXMySuperBuilder());
         _exceptions = new List<Exception>();
      }

      protected override void Because()
      {
         // The cache→Add race only exists during the first population of a given type, so each
         // attempt uses a fresh repository to re-open that window, with all threads released at once.
         for (var attempt = 0; attempt < _attempts; attempt++)
         {
            var repository = new BuilderRepositoryForSpecs(_container);
            var startSignal = new ManualResetEventSlim(false);
            var threads = new List<Thread>();

            for (var t = 0; t < _threadCount; t++)
            {
               threads.Add(new Thread(() =>
               {
                  startSignal.Wait();
                  try
                  {
                     repository.BuilderFor(new MySuperParameter());
                  }
                  catch (Exception ex)
                  {
                     lock (_exceptions)
                     {
                        _exceptions.Add(ex);
                     }
                  }
               }));
            }

            threads.ForEach(x => x.Start());
            startSignal.Set();
            threads.ForEach(x => x.Join());
         }
      }

      [Observation]
      public void should_resolve_the_builder_without_throwing()
      {
         var messages = string.Join(Environment.NewLine, _exceptions.Select(ex => $"{ex.GetType().Name}: {ex.Message}"));
         messages.ShouldBeEqualTo(string.Empty);
      }
   }

   internal class Parameter
   {
   }

   internal class MySuperParameter : Parameter
   {
   }

   internal class TEXMySuperBuilder : IMyBuilder<MySuperParameter>
   {
      public bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<MySuperParameter>();
      }
   }

   internal class ParameterBuilder : IMyBuilder<Parameter>
   {
      public bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<MySuperParameter>();
      }
   }

   internal class BuilderRepositoryForSpecs : BuilderRepository<IMyBuilder>
   {
      public BuilderRepositoryForSpecs(IContainer container) : base(container, typeof(IMyBuilder<>))
      {
      }
   }

   public interface IMyBuilder : ISpecification<Type>
   {
   }

   public interface IMyBuilder<T> : IMyBuilder
   {
   }
}