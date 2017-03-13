namespace OSPSuite.Utility
{
   public interface ISpecification<in T>
   {
      bool IsSatisfiedBy(T item);
   }
}