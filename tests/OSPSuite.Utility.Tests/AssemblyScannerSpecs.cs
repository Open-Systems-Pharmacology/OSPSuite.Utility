using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.FileLocker;

namespace OSPSuite.Utility.Tests
{
   public abstract class concern_for_AssemblyScanner : StaticContextSpecification
   {
      protected IContainer _container;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
      }
   }

   public class When_told_to_scan_an_assembly : concern_for_AssemblyScanner
   {
      protected override void Because()
      {
         _container.AddScanner(scan => scan.AssemblyContainingType<When_told_to_scan_an_assembly>());
      }

      [Observation]
      public void Should_register_all_concrete_implementation_fullfilling_the_default_convention()
      {
         A.CallTo(() => _container.Register(typeof(IMyClass), typeof(MyClass), LifeStyle.Transient)).MustHaveHappened();
      }

      [Observation]
      public void Should_not_register_type_that_do_not_fulfill_the_default_convention()
      {
         A.CallTo(() => _container.Register(typeof(IMyInterace), A<Type>._, A<LifeStyle>._)).MustNotHaveHappened();
      }
   }

   public class When_told_to_scan_an_assembly_with_exclusion : concern_for_AssemblyScanner
   {
      protected override void Because()
      {
         _container.AddScanner(
            scan =>
            {
               scan.AssemblyContainingType<When_told_to_scan_an_assembly>();
               scan.ExcludeNamespaceContainingType<IFileLocker>();
            });
      }

      [Observation]
      public void Should_not_register_type_excluded()
      {
         A.CallTo(() => _container.Register(typeof(IFileLocker), A<Type>._, A<LifeStyle>._)).MustNotHaveHappened();
      }

      [Observation]
      public void Should_register_other_types()
      {
         A.CallTo(() => _container.Register(typeof(IMyClass), typeof(MyClass), LifeStyle.Transient)).MustHaveHappened();
      }
   }

   public class When_told_to_scan_an_assembly_with_inclusions : concern_for_AssemblyScanner
   {
      protected override void Because()
      {
         _container.AddScanner(
            scan =>
            {
               scan.AssemblyContainingType<When_told_to_scan_an_assembly>();
               scan.IncludeNamespaceContainingType<IMyInterace>();
            });
      }

      [Observation]
      public void Should_not_register_other_types()
      {
         A.CallTo(() => _container.Register(typeof(IFileLocker), A<Type>._, A<LifeStyle>._)).MustNotHaveHappened();
      }

      [Observation]
      public void Should_only_register_type_included()
      {
         A.CallTo(() => _container.Register(typeof(IMyClass), typeof(MyClass), LifeStyle.Transient)).MustHaveHappened();
      }
   }

   public interface IMyClass
   {
   }

   public class MyClass : IMyClass
   {
   }

   public interface IMyInterace
   {
   }

   public class MyImplementation : IMyInterace
   {
   }
}