using System;

namespace OSPSuite.Utility.Container.Conventions
{
   /// <summary>
   ///    Registers the components with their default interface according to the following convention
   ///    MyObject will be registered in the component only it implements the interface IMyObject
   ///    In that case, MyObject will be registered with the interface IMyObject
   /// </summary>
   public class DefaultRegistrationConvention : IRegistrationConvention
   {
      public virtual void Process(Type concreteType, IContainer container, LifeStyle lifeStyle)
      {
         string interfaceName = "I" + concreteType.Name;
         Type[] interfaces = concreteType.GetInterfaces();
         Type concreteInterface = Array.Find(interfaces, t => t.Name == interfaceName);
         if (concreteInterface == null) return;
         container.Register(concreteInterface, concreteType, lifeStyle);
      }
   }
}