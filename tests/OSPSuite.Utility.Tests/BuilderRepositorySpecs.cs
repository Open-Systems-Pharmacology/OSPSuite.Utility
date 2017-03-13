using System;
using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_BuilderRepository : ContextSpecification<IBuilderRepository<IMyBuilder>>
   {
      private IContainer _container;
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