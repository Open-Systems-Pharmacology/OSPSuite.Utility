using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Utility.Container;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_register : ContextSpecification<IRegister>
   {
      protected override void Context()
      {
         sut = new TestRegister();
      }
   }

   public class When_registering_components_with_a_register : concern_for_register
   {
      private IContainer _container;

      protected override void Context()
      {
         base.Context();
         _container = A.Fake<IContainer>();
      }

      protected override void Because()
      {
         _container.AddRegister(x => x.FromInstance(sut));
      }

      [Observation]
      public void should_add_the_components_to_the_container()
      {
         A.CallTo(() => _container.Register(A<Type>.Ignored, A<Type>.Ignored, LifeStyle.Transient)).MustHaveHappened();
      }
   }

   public class TestRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(x =>
         {
            x.AssemblyContainingType<IRegister>();
            x.IncludeType<FileLocker.FileLocker>();
         });
      }
   }
}