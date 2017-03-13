namespace OSPSuite.Utility.Container
{
   public interface IRegister
   {
      void RegisterInContainer(IContainer container);
   }

   public abstract class Register : IRegister
   {
      public abstract void RegisterInContainer(IContainer container);
   }
}