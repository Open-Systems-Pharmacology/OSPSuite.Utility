using System;
using System.Globalization;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.Utility.Format
{
   public interface INumericFormatter<in TObjectType> : IFormatter<TObjectType>
   {
   }

   public class NumericFormatter<TObjectType> : INumericFormatter<TObjectType>
   {
      private readonly INumericFormatterOptions _numericFormatterOptions;
      private readonly NumberFormatInfo _numerFormatInfo;

      public NumericFormatter(INumericFormatterOptions numericFormatterOptions)
      {
         _numericFormatterOptions = numericFormatterOptions;
         _numerFormatInfo = new NumberFormatInfo();
      }

      public virtual string Format(TObjectType valueToFormat)
      {
         var typeOfValue = typeof(TObjectType);
         if (typeOfValue.IsNullableType())
         {
            object obj = valueToFormat;
            if (obj == null) return string.Empty;
         }

         if (typeOfValue.IsDouble())
         {
            return formattedValueFor(valueToFormat.ConvertedTo<double>());
         }
         if (typeOfValue.IsUnsignedInteger())
         {
            return valueToFormat.ConvertedTo<ulong>().ToString(_numerFormatInfo);
         }

         if (typeOfValue.IsSignedInteger())
         {
            return valueToFormat.ConvertedTo<long>().ToString(_numerFormatInfo);
         }

         return string.Empty;
      }

      //adapted from commonservices dll 2.3
      private string formattedValueFor(double value)
      {
         // RETURN ALWAYS 0 for 0
         if (value == 0) return "0";

         string format;
         if (_numericFormatterOptions.DecimalPlace == 0)
            format = "0";
         else
         {
            format = "0.";
            for (int i = 0; i < _numericFormatterOptions.DecimalPlace; i++)
               format += "0";
         }

         if (_numericFormatterOptions.AllowsScientificNotation && Math.Abs(value) < 0.01)
            return value.ToString(format + "E-0", _numerFormatInfo);

         if (_numericFormatterOptions.AllowsScientificNotation && Math.Abs(value) > 1000000)
            return value.ToString(format + "E+0", _numerFormatInfo);

         return value.ToString(format, _numerFormatInfo);
      }
   }
}