namespace OSPSuite.Utility
{
   public interface IMapper<in TInput, out TOutput>
   {
      /// <summary>
      ///    Map the given <paramref name="input" /> to an object of type <typeparamref name="TOutput" />
      /// </summary>
      /// <param name="input"></param>
      /// <returns></returns>
      TOutput MapFrom(TInput input);
   }
}