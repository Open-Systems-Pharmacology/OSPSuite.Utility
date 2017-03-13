namespace OSPSuite.Utility
{
   public interface IMapper<in Input, out Output>
   {
      /// <summary>
      ///    Map the given <paramref name="input" /> to an object of type <typeparamref name="Output" />
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      Output MapFrom(Input input);
   }
}