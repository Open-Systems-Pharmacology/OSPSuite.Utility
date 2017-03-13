using OSPSuite.Utility.Reflection;

namespace OSPSuite.Utility.Tests
{
   public interface IAnInterface : INotifier
   {
      string FirstName { get; set; }
      string LastName { get; set; }
      IAnInterface Child { get; set; }
      string ReadOnlyProp { get; }
   }

   public interface IAnotherInteface : IAnInterface
   {
   }

   public class AnImplementation : Notifier, IAnInterface
   {
      private string _firstName;

      public string FirstName
      {
         get { return _firstName; }
         set
         {
            _firstName = value;
            OnPropertyChanged(() => FirstName);
         }
      }

      public string LastName { get; set; }
      public IAnInterface Child { get; set; }
      public string ReadOnlyProp { get; private set; }
      public double OnePublicMEmber;

      public AnImplementation()
      {
         ReadOnlyProp = "tutu";
      }
   }

   public class AnotherImplementation : Notifier, IAnotherInteface
   {
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public IAnInterface Child { get; set; }
      public string ReadOnlyProp { get; private set; }
   }

   public class ADerivedImplementation : AnotherImplementation
   {
   }

   internal class AnInternalClass : AnImplementation
   {
   }
}