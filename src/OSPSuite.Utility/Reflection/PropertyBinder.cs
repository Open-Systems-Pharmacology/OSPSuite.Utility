using System.Reflection;

namespace OSPSuite.Utility.Reflection
{
   public interface IPropertyBinder<in TObject, TPropertyType>
   {
      string PropertyName { get; }
      TPropertyType GetValue(TObject target);
      void SetValue(TObject target, TPropertyType value);
      bool CanSetValueTo(TObject target);
   }

   public class PropertyBinder<TObject, TPropertyType> : IPropertyBinder<TObject, TPropertyType>
   {
      protected readonly PropertyInfo _propertyInfo;

      public string PropertyName
      {
         get { return _propertyInfo.Name; }
      }

      public PropertyBinder(PropertyInfo propertyInfo)
      {
         _propertyInfo = propertyInfo;
      }

      public virtual TPropertyType GetValue(TObject target)
      {
         if (target == null) return default(TPropertyType);
         return (TPropertyType) _propertyInfo.GetValue(target, null);
      }

      public virtual void SetValue(TObject target, TPropertyType value)
      {
         if (CanSetValueTo(target) == false) return;
         try
         {
            _propertyInfo.SetValue(target, value, null);
         }
         catch (TargetInvocationException e)
         {
            //exception is thrown when for instance set value throws an exception. In that case, rethrow the inner excpetion
            if (e.InnerException != null)
               throw e.InnerException;

            //simply rethrow exception 
            throw;
         }
      }

      public bool CanSetValueTo(TObject target)
      {
         if (target == null) return false;
         return _propertyInfo.CanWrite;
      }
   }
}