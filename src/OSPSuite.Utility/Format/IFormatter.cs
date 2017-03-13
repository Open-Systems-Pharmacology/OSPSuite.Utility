namespace OSPSuite.Utility.Format
{
   public interface IFormatter<in TObjectType>
   {
      string Format(TObjectType valueToFormat);
   }
}