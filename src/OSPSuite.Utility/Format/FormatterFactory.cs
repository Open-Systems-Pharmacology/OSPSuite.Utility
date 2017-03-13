using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Format
{
   public interface IFormatterFactory
   {
      IFormatter<TObjectType> CreateFor<TObjectType>();
   }

   public class FormatterFactory : IFormatterFactory
   {
      public IFormatter<TObjectType> CreateFor<TObjectType>()
      {
         if (typeof(TObjectType).IsNumeric())
            return new NumericFormatter<TObjectType>(NumericFormatterOptions.Instance);

         return new DefaultFormatter<TObjectType>();
      }
   }
}