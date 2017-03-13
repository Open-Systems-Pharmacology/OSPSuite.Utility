using System;

namespace OSPSuite.Utility.Format
{
   public interface IFormatterConfigurator<in TObjectType, in TPropertyType>
   {
      IFormatter<TPropertyType> FormatterFor(TObjectType boundObject);
   }

   public class FormatterConfigurator<TObjectType, TPropertyType> : IFormatterConfigurator<TObjectType, TPropertyType>
   {
      private readonly IFormatterFactory _formatterFactory;
      private readonly Func<TObjectType, IFormatter<TPropertyType>> _formatterProvider;
      private readonly IFormatter<TPropertyType> _defaultFormatter;

      public FormatterConfigurator(Func<TObjectType, IFormatter<TPropertyType>> formatterProvider)
      {
         _formatterProvider = formatterProvider;
      }

      public FormatterConfigurator()
      {
         _formatterFactory = new FormatterFactory();
         _defaultFormatter = _formatterFactory.CreateFor<TPropertyType>();
         _formatterProvider = source => _defaultFormatter;
      }

      public IFormatter<TPropertyType> FormatterFor(TObjectType boundObject)
      {
         return _formatterProvider(boundObject);
      }
   }
}