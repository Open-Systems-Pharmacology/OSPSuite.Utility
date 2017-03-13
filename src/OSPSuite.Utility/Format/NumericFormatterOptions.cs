namespace OSPSuite.Utility.Format
{
   public interface INumericFormatterOptions
   {
      uint DecimalPlace { get; set; }
      bool AllowsScientificNotation { get; set; }
   }

   public class NumericFormatterOptions : INumericFormatterOptions
   {
      public uint DecimalPlace { get; set; }
      public bool AllowsScientificNotation { get; set; }

      /// <summary>
      ///    Single instance of NumericFormatterOptionsthat will be use to return a singleton
      /// </summary>
      public static readonly INumericFormatterOptions Instance = new NumericFormatterOptions();

      public NumericFormatterOptions()
      {
         DecimalPlace = 2;
         AllowsScientificNotation = true;
      }
   }
}