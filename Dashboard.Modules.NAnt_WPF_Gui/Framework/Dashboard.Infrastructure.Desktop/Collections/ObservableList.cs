#region Copyleft and Copyright

// .NET Dev Tools Dashboard
// Copyright 2011 (C) Wim Van den Broeck - Techno-Fly
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Wim Van den Broeck (wim@techno-fly.net)

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

		public ObservableList(IEnumerable<T> collection) : base(collection)
		{
			/* Intentionally left blank. */
		}

		public ObservableList(List<T> collection) : base(collection)
		{
			/* Intentionally left blank. */
		}

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