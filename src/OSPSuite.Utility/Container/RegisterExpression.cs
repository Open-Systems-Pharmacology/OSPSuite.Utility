using System.Collections.Generic;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Container
{
   public interface IRegisterExpression
   {
      /// <summary>
      ///    Creates and adds a Register object of type T.
      /// </summary>
      /// <typeparam name="T">The Register Type</typeparam>
      void FromType<T>() where T : IRegister, new();

      /// <summary>
      ///    Imports all the configuration from a Register object
      /// </summary>
      /// <param name="register">register to import</param>
      void FromInstance(IRegister register);

      void Build();
   }

   public class RegisterExpression : IRegisterExpression
   {
      private readonly IContainer _container;
      private readonly IList<IRegister> _registers = new List<IRegister>();

      public RegisterExpression(IContainer container)
      {
         _container = container;
      }

      public void FromInstance(IRegister register)
      {
         if (!_registers.Contains(register))
         {
            _registers.Add(register);
         }
      }

      public void Build()
      {
         _registers.Each(reg => reg.RegisterInContainer(_container));
      }

      public void FromType<T>() where T : IRegister, new()
      {
         FromInstance(new T());
      }
   }
}