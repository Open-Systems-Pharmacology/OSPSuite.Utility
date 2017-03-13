namespace OSPSuite.Utility.Events
{
   public interface IListener
   {
   }

   public interface IListener<in T> : IListener
   {
      void Handle(T eventToHandle);
   }
}