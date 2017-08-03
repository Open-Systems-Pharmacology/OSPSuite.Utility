using System.Linq.Expressions;
using System.Reflection;

namespace OSPSuite.Utility.Reflection
{
    /// <summary>
    ///     Delegate to a property get info.
    ///     This is required when using reflection extensively to enhance performance
    /// </summary>
    public delegate object GetHandler(object target);

    /// <summary>
    ///     Delegate to a property get info.
    ///     This is required when using reflection extensively to enhance performance
    /// </summary>
    public delegate void SetHandler(object target, object value);

    public static class DelegateFactory
    {
        /// <summary>
        ///     Returns an handler to a property get for a property that is compiled=>access is much faster than using reflection
        ///     directly
        /// </summary>
        public static GetHandler CreateGet(PropertyInfo property)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");
            var member = Expression.Property(Expression.Convert(instanceParameter, property.DeclaringType), property);

            var lambda = Expression.Lambda<GetHandler>(
                Expression.Convert(member, typeof(object)),
                instanceParameter
            );

            return lambda.Compile();
        }

        /// <summary>
        ///     Returns an handler to a property get for a field that  is compiled=>access is much faster than using reflection
        ///     directly
        /// </summary>
        public static GetHandler CreateGet(FieldInfo field)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "target");
            var member = Expression.Field(Expression.Convert(instanceParameter, field.DeclaringType), field);

            var lambda = Expression.Lambda<GetHandler>(
                Expression.Convert(member, typeof(object)),
                instanceParameter
            );

            return lambda.Compile();
        }

        /// <summary>
        ///     Returns an handler to a property set for a field that is compiled=>access is much faster than using reflection
        ///     directly
        /// </summary>
        public static SetHandler CreateSet(FieldInfo field)
        {
            var targetExp = Expression.Parameter(typeof(object), "target");
            var valueExp = Expression.Parameter(typeof(object), "value");
            var fieldExp = Expression.Field(Expression.Convert(targetExp, field.DeclaringType), field);
            var body = Expression.Assign(fieldExp, Expression.Convert(valueExp, field.FieldType));

            var lambda = Expression.Lambda<SetHandler>(body, targetExp, valueExp);
            return lambda.Compile();
        }

        /// <summary>
        ///     Returns an handler to a property set for a property that is compiled=>access is much faster than using reflection
        ///     directly
        /// </summary>
        public static SetHandler CreateSet(PropertyInfo property)
        {
            //http://stackoverflow.com/questions/10760139/setting-property-without-knowing-target-type-at-compile-time
            var targetExp = Expression.Parameter(typeof(object), "target");
            var valueExp = Expression.Parameter(typeof(object), "value");
            var propertyExp = Expression.Property(Expression.Convert(targetExp, property.DeclaringType), property);
            var body = Expression.Assign(propertyExp, Expression.Convert(valueExp, property.PropertyType));

            var lambda = Expression.Lambda<SetHandler>(body, targetExp, valueExp);
            return lambda.Compile();
        }
    }
}