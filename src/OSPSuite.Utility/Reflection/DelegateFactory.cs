using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace OSPSuite.Utility.Reflection
{
   /// <summary>
   ///    Delegate to a property get info.
   ///    This is required when using reflection extensively to enhance performance
   /// </summary>
   public delegate object GetHandler(object target);

   /// <summary>
   ///    Delegate to a property get info.
   ///    This is required when using reflection extensively to enhance performance
   /// </summary>
   public delegate void SetHandler(object target, object value);

   public static class DelegateFactory
   {
      /// <summary>
      ///    Returns an handler to a property get for a property that is compiled=>access is much faster than using reflection
      ///    directly
      /// </summary>
      public static GetHandler CreateGet(PropertyInfo property)
      {
         var instanceParameter = Expression.Parameter(typeof(object), "target");
         var member = Expression.Property(Expression.Convert(instanceParameter, property.DeclaringType), property);

         Expression<GetHandler> lambda = Expression.Lambda<GetHandler>(
            Expression.Convert(member, typeof(object)),
            instanceParameter
         );

         return lambda.Compile();
      }

      /// <summary>
      ///    Returns an handler to a property get for a field that  is compiled=>access is much faster than using reflection
      ///    directly
      /// </summary>
      public static GetHandler CreateGet(FieldInfo field)
      {
         var instanceParameter = Expression.Parameter(typeof(object), "target");
         var member = Expression.Field(Expression.Convert(instanceParameter, field.DeclaringType), field);

         Expression<GetHandler> lambda = Expression.Lambda<GetHandler>(
            Expression.Convert(member, typeof(object)),
            instanceParameter
         );

         return lambda.Compile();
      }

      /// <summary>
      ///    Returns an handler to a property set for a field that is compiled=>access is much faster than using reflection
      ///    directly
      /// </summary>
      /// <remarks>This is emulating the creation of IL code, because it is not possible so far to compile property set</remarks>
      public static SetHandler CreateSet(FieldInfo field)
      {
//         var target = Expression.Parameter(typeof (object), "target");
//         var value = Expression.Parameter(typeof (object), "value");
//         var member = Expression.Field(Expression.Convert(target, field.DeclaringType), field);
//
//         var lambda = Expression.Lambda<SetHandler>(
//            member, target, value);
//
//         return lambda.Compile();

         var sourceType = field.DeclaringType;
         var method = new DynamicMethod("Set" + field.Name, null, new[] {typeof(object), typeof(object)}, true);
         var gen = method.GetILGenerator();

         gen.Emit(OpCodes.Ldarg_0); // Load input to stack
         gen.Emit(OpCodes.Castclass, sourceType); // Cast to source type
         gen.Emit(OpCodes.Ldarg_1); // Load value to stack
         gen.Emit(OpCodes.Unbox_Any, field.FieldType); // Unbox the value to its proper value type
         gen.Emit(OpCodes.Stfld, field); // Set the value to the input field
         gen.Emit(OpCodes.Ret);

         var callback = (SetHandler) method.CreateDelegate(typeof(SetHandler));

         return callback;
      }

      /// <summary>
      ///    Returns an handler to a property set for a property that is compiled=>access is much faster than using reflection
      ///    directly
      /// </summary>
      /// <remarks>This is emulating the creation of IL code, because it is not possible so far to compile property set</remarks>
      public static SetHandler CreateSet(PropertyInfo property)
      {
         //http://stackoverflow.com/questions/10760139/setting-property-without-knowing-target-type-at-compile-time
         var target = Expression.Parameter(typeof(object), "target");
         var value = Expression.Parameter(typeof(object), "value");

         var body = Expression.Assign(
            Expression.Property(Expression.Convert(target, property.DeclaringType), property),
            Expression.Convert(value, property.PropertyType));

         var lambda = Expression.Lambda<SetHandler>(body, target, value);
         return lambda.Compile();
      }
   }
}