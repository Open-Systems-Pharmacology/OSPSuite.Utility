using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace OSPSuite.Utility.Collections
{
   public interface INotifyList<TObject> : IList<TObject>, INotifyCollectionChanged
   {
      INotifyList<TObject> FindAll(Func<TObject, bool> predicate);
      INotifyList<TObject> FindAllThatSatisfy(ISpecification<TObject> specification);
      INotifyList<TObject> SortUsing(IComparer<TObject> comparer);
   }

   public class NotifyList<TObject> : ObservableCollection<TObject>, INotifyList<TObject>
   {
      public NotifyList()
      {
      }

      public NotifyList(IEnumerable<TObject> collection) : base(collection)
      {
      }

      public NotifyList(List<TObject> list) : base(list)
      {
         PropertyChanged += delegate { };
         CollectionChanged += delegate { };
      }

      public INotifyList<TObject> FindAll(Func<TObject, bool> predicate)
      {
         return new NotifyList<TObject>(this.Where(predicate));
      }

      public INotifyList<TObject> FindAllThatSatisfy(ISpecification<TObject> specification)
      {
         return FindAll(specification.IsSatisfiedBy);
      }

      public INotifyList<TObject> SortUsing(IComparer<TObject> comparer)
      {
         var sorted = new List<TObject>(this);
         sorted.Sort(comparer);
         return new NotifyList<TObject>(sorted);
      }
   }
}