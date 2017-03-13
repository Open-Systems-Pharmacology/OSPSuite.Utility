namespace OSPSuite.Utility.Visitor
{
   public static class VisitorExtensions
   {
      public static void Visit<T>(this IVisitor visitor, T objectToVisit)
      {
         VisitorInvoker.InvokeVisit(visitor, objectToVisit);
      }
   }
}