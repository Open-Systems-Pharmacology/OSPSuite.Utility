namespace OSPSuite.Utility.Validation
{
   public interface INotification
   {
      string ErrorNotification { get; }
      bool HasError();
   }

   internal class EmptyNotification : INotification
   {
      public string ErrorNotification => string.Empty;

      public bool HasError()
      {
         return false;
      }
   }

   public class Notification : INotification
   {
      public string ErrorNotification { get; }

      public Notification() : this(string.Empty)
      {
      }

      public Notification(string errorNotification)
      {
         ErrorNotification = errorNotification;
      }

      public bool HasError()
      {
         return (string.IsNullOrEmpty(ErrorNotification) == false);
      }
   }
}