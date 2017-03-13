namespace OSPSuite.Utility.Validation
{
   public interface IValidationEngine
   {
      INotification Validate<T>(T objectToValidate);
      INotification Validate<T>(T objectToValidate, string nameOfPropertyToValidate);
      INotification Validate<T>(T objectToValidate, string nameOfPropertyToValidate, object value);
   }

   public class ValidationEngine : IValidationEngine
   {
      private readonly INotificationFactory _factory;

      public ValidationEngine(INotificationFactory factory)
      {
         _factory = factory;
      }

      public ValidationEngine() : this(new NotificationFactory())
      {
      }

      public INotification Validate<T>(T objectToValidate)
      {
         var businessRuleObject = objectToValidate as IValidatable;
         if (businessRuleObject == null) return _factory.NoNotification();
         return _factory.CreateNotificationFrom(businessRuleObject.Validate());
      }

      public INotification Validate<T>(T objectToValidate, string nameOfPropertyToValidate)
      {
         var businessRuleObject = objectToValidate as IValidatable;
         if (businessRuleObject == null) return _factory.NoNotification();
         return _factory.CreateNotificationFrom(businessRuleObject.Validate(nameOfPropertyToValidate));
      }

      public INotification Validate<T>(T objectToValidate, string nameOfPropertyToValidate, object value)
      {
         var businessRuleObject = objectToValidate as IValidatable;
         if (businessRuleObject == null) return _factory.NoNotification();
         return _factory.CreateNotificationFrom(businessRuleObject.Validate(nameOfPropertyToValidate, value));
      }
   }
}