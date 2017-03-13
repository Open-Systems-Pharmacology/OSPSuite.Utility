using System;

namespace OSPSuite.Utility.Container
{
   public static class ContainerExtensions
   {
      public static void AddScanner(this IContainer container, Action<IAssemblyScanner> scan)
      {
         IAssemblyScanner assemblyScanner = new AssemblyScanner(container);
         scan(assemblyScanner);
         assemblyScanner.Build();
      }

      public static void AddRegister(this IContainer container, Action<IRegisterExpression> containerInitialization)
      {
         IRegisterExpression registerExpression = new RegisterExpression(container);
         containerInitialization(registerExpression);
         registerExpression.Build();
      }
   }
}