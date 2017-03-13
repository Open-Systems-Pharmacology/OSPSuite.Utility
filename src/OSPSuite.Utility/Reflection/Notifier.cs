using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace OSPSuite.Utility.Reflection
{
   public interface INotifier : INotifyPropertyChanged
   {
      event Action<object> Changed;
   }

   public abstract class Notifier : INotifier
   {
      public event PropertyChangedEventHandler PropertyChanged = delegate { };
      public event Action<object> Changed = delegate { };

      protected virtual void OnChanged()
      {
         Changed(this);
      }

      protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
      {
         RaisePropertyChanged(propertyName);
      }

      protected void OnPropertyChanged<TPropertyType>(Expression<Func<TPropertyType>> exp)
      {
         var memberExpression = ReflectionHelper.GetMemberExpression(exp);
         var propertyName = memberExpression.Member.Name;

         RaisePropertyChanged(propertyName);
      }

      protected virtual void RaisePropertyChanged(string propertyName)
      {
         PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         OnChanged();
      }
   }
}