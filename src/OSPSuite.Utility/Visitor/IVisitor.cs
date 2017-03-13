namespace OSPSuite.Utility.Visitor
{
   /// <summary>
   ///    base interface for visitor used to group different visitors into for example one list of visitors.
   /// </summary>
   public interface IVisitor
   {
   }

   /// <summary>
   ///    Strict visitor is used for a visitor for which all visited type needs to be found
   ///    while resolvign a type to visit dynamically
   /// </summary>
   public interface IStrictVisitor : IVisitor
   {
   }

   /// <summary>
   ///    Provides an interface for visitors.
   /// </summary>
   /// <typeparam name="TItemToVisit">The type of objects to be visited.</typeparam>
   public interface IVisitor<TItemToVisit> : IVisitor
   {
      /// <summary>
      ///    Visit the specified object.
      /// </summary>
      /// <param name="objToVisit">The object to visit.</param>
      void Visit(TItemToVisit objToVisit);
   }
}