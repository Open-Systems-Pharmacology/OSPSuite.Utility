namespace OSPSuite.Utility.Format
{
   public class DefaultFormatter<TObjectType> : DynamicFormatter<TObjectType>
   {
      public DefaultFormatter()
         : base(toStringFormat)
      {
      }

      private static string toStringFormat(TObjectType objectToFormat)
      {
         object obj = objectToFormat;
         return obj == null ? string.Empty : obj.ToString();
      }
   }
}