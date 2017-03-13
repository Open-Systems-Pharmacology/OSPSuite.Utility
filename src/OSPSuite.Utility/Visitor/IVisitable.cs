namespace OSPSuite.Utility.Visitor
{
   /// <summary>
   ///    provide a generic interface for visitable object.
   /// </summary>
   public interface IVisitable<in T> where T : IVisitor
   {
      void AcceptVisitor(T visitor);
   }
}