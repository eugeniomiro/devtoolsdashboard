#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of Daniel Vaughan's base library

	Daniel Vaughan's base library is free software: you can redistribute it and/or modify
	it under the terms of the GNU Lesser General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Daniel Vaughan's base library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with DanielVaughan's base library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2011-01-10 16:27:11Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Techno_Fly.Tools.Dashboard;
using Techno_Fly.Tools.Dashboard.Collections;
using Techno_Fly.Tools.Dashboard.ComponentModel;

using System.Linq;

namespace DanielVaughan.Collections
{
	public class ObservableList<T> : ObservableCollection<T>, 
		ISuspendChangeNotification, IRangeOperations
	{
		public ObservableList()
		{
			/* Intentionally left blank. */
		}

#if WINDOWS_PHONE
		public ObservableList(IEnumerable<T> collection)
		{
			ArgumentValidator.AssertNotNull(collection, "collection");
			AddRangeCore(collection);
		}

		public ObservableList(List<T> collection)
		{
			ArgumentValidator.AssertNotNull(collection, "collection");
			AddRangeCore(collection);
		}
#else
		public ObservableList(IEnumerable<T> collection) : base(collection)
		{
			/* Intentionally left blank. */
		}

		public ObservableList(List<T> collection) : base(collection)
		{
			/* Intentionally left blank. */
		}
#endif

		bool changeNotificationSuspended;

		public bool ChangeNotificationSuspended
		{
			get
			{
				return changeNotificationSuspended;
			}
			set
			{
				changeNotificationSuspended = value;
			}
		}
		
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (!changeNotificationSuspended)
			{
				base.OnCollectionChanged(e);
			}
		}

		/* TODO: [DV] Comment. */
		public void AddRange(IEnumerable<T> items)
		{
			ArgumentValidator.AssertNotNull(items, "items");
			AddRangeCore(items);
		}

		void AddRangeCore(IEnumerable<T> items)
		{
			bool wasSuspended = changeNotificationSuspended;
			IList<T> newItems = new List<T>(items);
			
			try
			{
				changeNotificationSuspended = true;

				foreach (var item in newItems)
				{
					base.Add(item);
				}
			}
			finally
			{
				if (!wasSuspended) /* Avoid unsetting suspended, 
								 * if already explicitly set. This allow the user 
								 * to add more items without unsetting 
								 * ChangeNotificationSuspended again. */
				{
					changeNotificationSuspended = false;
				}
			}

			if (!wasSuspended)
			{
				/* Using NotifyCollectionChangedAction.Add 
				 * raises an exception on the desktop, 
				 * hence we use Reset for both. */
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset);
				OnCollectionChanged(eventArgs);
			}
		}

		/* TODO: [DV] Comment. */
		public void RemoveRange(IEnumerable<T> items)
		{
			ArgumentValidator.AssertNotNull(items, "items");
			RemoveRangeCore(items);
		}
		
		void RemoveRangeCore(IEnumerable<T> items)
		{
			bool wasSuspended = changeNotificationSuspended;

			changeNotificationSuspended = true;
			IList<T> removedItems = new List<T>();

			try
			{
				foreach (var item in items.ToList())
				{
					if (base.Remove(item))
					{
						removedItems.Add(item);
					}
				}
			}
			finally
			{
				if (!wasSuspended) /* Avoid unsetting suspended, 
								 * if already explicitly set. This allow the user 
								 * to add more items without unsetting 
								 * ChangeNotificationSuspended again. */
				{
					changeNotificationSuspended = false;
				}
			}
			if (!wasSuspended)
			{
				var eventArgs = new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Reset);
				OnCollectionChanged(eventArgs);
			}
		}

		public void AddRange(IEnumerable items)
		{
			ArgumentValidator.AssertNotNull(items, "items");
			AddRangeCore(items.Cast<T>());
		}

		public void RemoveRange(IEnumerable items)
		{
			ArgumentValidator.AssertNotNull(items, "items");
			RemoveRangeCore(items.Cast<T>());
		}
	}

}