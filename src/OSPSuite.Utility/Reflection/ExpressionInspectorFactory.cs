namespace OSPSuite.Utility.Reflection
{
   public interface IExpressionInspectorFactory
   {
      IExpressionInspector<TypeToInspect> Create<TypeToInspect>();
   }

   public class ExpressionInspectorFactory : IExpressionInspectorFactory
   {
      public IExpressionInspector<TypeToInspect> Create<TypeToInspect>()
      {
         return new ExpressionInspector<TypeToInspect>();
      }
   }
}