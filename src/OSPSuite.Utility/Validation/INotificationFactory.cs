namespace OSPSuite.Utility.Validation
{
   public interface INotificationFactory
   {
      INotification NoNotification();
      INotification CreateNotificationFrom(IBusinessRuleSet ruleSet);
   }

   public class NotificationFactory : INotificationFactory
   {
      public INotification NoNotification()
      {
         return new EmptyNotification();
      }

      public INotification CreateNotificationFrom(IBusinessRuleSet ruleSet)
      {
         if (ruleSet == null || ruleSet.IsEmpty)
            return NoNotification();

         return new Notification(ruleSet.Message);
      }
   }
}