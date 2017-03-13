using System;

namespace OSPSuite.Utility.Container.Conventions
{
   public interface IRegistrationConvention
   {
      void Process(Type concreteType, IContainer container, LifeStyle lifeStyle);
   }
}