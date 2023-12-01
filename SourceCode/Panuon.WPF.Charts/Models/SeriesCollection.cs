using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Panuon.WPF.Charts
{
    public class SeriesCollection
        : Collection<SeriesBase>
    {
        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Overrides
        protected override void InsertItem(int index, SeriesBase item)
        {
            base.InsertItem(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        protected override void SetItem(int index, SeriesBase item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                item,
                oldItem));
        }
        #endregion
    }
}