using System;

namespace OSPSuite.Utility.Format
{
   public class DynamicFormatter<TObjectType> : IFormatter<TObjectType>
   {
      private readonly Func<TObjectType, string> _formatter;

      public DynamicFormatter(Func<TObjectType, string> formatter)
      {
         _formatter = formatter;
      }

      public virtual string Format(TObjectType valueToFormat)
      {
         return _formatter(valueToFormat);
      }
   }
}