using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Techno_Fly.Tools.Dashboard;
using Techno_Fly.Tools.Dashboard.Concurrency;

namespace DanielVaughan.Collections
{
	/// <summary>
	/// Provides <see cref="INotifyCollectionChanged"/> events on the subscription thread 
	/// using an <see cref="ISynchronizationContext"/>.
	/// </summary>
	/// <typeparam name="T">The type of items in the collection.</typeparam>
	public class SynchronizedObservableCollection<T> : Collection<T>, 
		INotifyCollectionChanged, INotifyPropertyChanged
	{
		bool busy;
		readonly DelegateManager collectionChangedManager;
		//readonly ISynchronizationContext uiContext;
		IProvider<ISynchronizationContext> contextProvider;

		/// <summary>
		/// Occurs when the items list of the collection has changed, 
		/// or the collection is reset.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				collectionChangedManager.Add(value);
			}
			remove
			{
				collectionChangedManager.Remove(value);
			}
		}

		PropertyChangedEventHandler propertyChanged;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				propertyChanged += value;
			}
			remove
			{
				propertyChanged -= value;
			}
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="SynchronizedObservableCollection&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="contextProvider">The synchronization context provider, 
		/// which is used to determine on what thread a handler is invoked.</param>
		public SynchronizedObservableCollection(IProvider<ISynchronizationContext> contextProvider)
		{
			this.contextProvider = contextProvider;
			collectionChangedManager = new DelegateManager(
				true, false, DelegateInvocationMode.Blocking, contextProvider);
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="SynchronizedObservableCollection&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="collection">The collection to copy.</param>
		/// <param name="contextProvider">The synchronization context provider, 
		/// which is used to determine on what thread a handler is invoked.</param>
		public SynchronizedObservableCollection(IEnumerable<T> collection, 
			IProvider<ISynchronizationContext> contextProvider) : this(contextProvider)
		{
			ArgumentValidator.AssertNotNull(collection, "collection");
			CopyFrom(collection);
		}

		ISynchronizationContext Context
		{
			get
			{
				if (contextProvider != null)
				{
					return contextProvider.ProvidedItem;
				}
				return UISynchronizationContext.Instance;
			}
		}

		public SynchronizedObservableCollection(List<T> list, 
			IProvider<ISynchronizationContext> contextProvider) 
			: base(list != null ? new List<T>(list.Count) : list)
		{
			this.contextProvider = contextProvider;
			collectionChangedManager = new DelegateManager(
				true, false, DelegateInvocationMode.Blocking, contextProvider);
			CopyFrom(list);
		}

		void PreventReentrancy()
		{
			if (busy)
			{
				throw new InvalidOperationException(
					"Cannot Change SynchronizedObservableCollection");
			}
		}

		protected override void ClearItems()
		{
			Context.InvokeAndBlockUntilCompletion(delegate{ClearItemsDelegate();});
		}

	    protected void ClearItemsDelegate()
	    {
	        PreventReentrancy();
	        base.ClearItems();
	        OnPropertyChanged("Count");
	        OnPropertyChanged("Item[]");
	        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
	    }

	    void CopyFrom(IEnumerable<T> collection)
		{
			Context.InvokeAndBlockUntilCompletion(
				delegate
				{
					IList<T> items = Items;
					if (collection != null && items != null)
					{
						using (IEnumerator<T> enumerator = collection.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								items.Add(enumerator.Current);
							}
						}
					}
				});
		}

		protected override void InsertItem(int index, T item)
		{
			Context.InvokeAndBlockUntilCompletion(delegate{InsertItemDelegate(index, item);});
		}

	    protected void InsertItemDelegate(int index, T item)
	    {
	        base.InsertItem(index, item);
	        OnPropertyChanged("Count");
	        OnPropertyChanged("Item[]");
	        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
	    }

	    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			busy = true;
			try
			{
				collectionChangedManager.InvokeDelegates(null, e);
			}
			finally
			{
				busy = false;
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (propertyChanged != null)
			{
				busy = true;
				try
				{
					propertyChanged(this, e);
				}
				finally
				{
					busy = false;
				}
			}
		}

		void OnPropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}
		
		protected override void RemoveItem(int index)
		{
			Context.InvokeAndBlockUntilCompletion(delegate{RemoveItemDelegate(index);});
		}

	    protected void RemoveItemDelegate(int index)
	    {
	        PreventReentrancy();
	        T changedItem = base[index];
	        base.RemoveItem(index);
	        OnPropertyChanged("Count");
	        OnPropertyChanged("Item[]");
	        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItem, index));
	    }

	    protected override void SetItem(int index, T item)
		{
			Context.InvokeAndBlockUntilCompletion(delegate{SetItemDelegate(index, item);});
		}

	    protected void SetItemDelegate(int index, T item)
	    {
	        PreventReentrancy();
	        T oldItem = base[index];
	        base.SetItem(index, item);
	        OnPropertyChanged("Item[]");
	        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
	    }
	}
}
