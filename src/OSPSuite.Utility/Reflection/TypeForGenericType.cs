using System;

namespace OSPSuite.Utility.Reflection
{
   /// <summary>
   ///    Helper class used to collect the type used un a generic type
   ///    <example>
   ///       For the generic type IVisitor[IAnInterface] we would create a TypeForGenericType(typeof(IAnInterface),
   ///       typeof(IVisitor[IAnInterface]))
   ///    </example>
   /// </summary>
   public class TypeForGenericType
   {
      public TypeForGenericType(Type declaredType, Type genericType)
      {
         DeclaredType = declaredType;
         GenericType = genericType;
      }

      public Type DeclaredType { get; private set; }
      public Type GenericType { get; private set; }

      public override bool Equals(object obj)
      {
         var tg = obj as TypeForGenericType;
         return tg != null && tg.GenericType == GenericType;
      }

      public override int GetHashCode()
      {
         return GenericType.GetHashCode();
      }
   }
}