using System;
using System.Collections.Generic;
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
         RaisePropertyChanged(nameFor(exp));
      }

      private static string nameFor<TPropertyType>(Expression<Func<TPropertyType>> exp)
      {
         var memberExpression = ReflectionHelper.GetMemberExpression(exp);
         return memberExpression.Member.Name;
      }

      protected virtual void RaisePropertyChanged(string propertyName)
      {
         PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         OnChanged();
      }

      protected bool SetProperty<T>(ref T backingField, T value, Expression<Func<T>> exp)
      {
         return setProperty(ref backingField, value, nameFor(exp));
      }

      protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
      {
         return setProperty(ref backingField, value, propertyName);
      }

      private bool setProperty<T>(ref T backingField, T value, string propertyName)
      {
         if (EqualityComparer<T>.Default.Equals(backingField, value))
            return false;

         backingField = value;
         RaisePropertyChanged(propertyName);

         return true;
      }
   }
}