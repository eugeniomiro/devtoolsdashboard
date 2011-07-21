#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2008-12-28 19:52:25Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Techno_Fly.Tools.Dashboard.ComponentModel;

namespace Techno_Fly.Tools.Dashboard.Collections
{
	public static class Extensions
	{
		/// <summary>
		/// Adds the items from one set of items to the specified list.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="toList">The destination list.</param>
		/// <param name="fromList">The source list.</param>
		public static void AddRange<T>(this IList<T> toList, IEnumerable<T> fromList)
		{
			ArgumentValidator.AssertNotNull(toList, "toList");
			ArgumentValidator.AssertNotNull(fromList, "fromList");

			IRangeOperations collection = toList as IRangeOperations;
			if (collection != null)
			{
				collection.AddRange(fromList);
				return;
			}

			var suspendableList = toList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}

				foreach (var item in fromList)
				{
					toList.Add(item);
				}
			}
			finally
			{
				if (suspendableList != null && !wasSuspended)
				{
					try
					{
						suspendableList.ChangeNotificationSuspended = false;
					}
					catch (Exception)
					{
						/* Suppress. */
					}
				}
			}
		}

		/// <summary>
		/// Adds the items from one set of items to the specified list.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="toList">The destination list.</param>
		/// <param name="fromList">The source list.</param>
		public static void AddRange(this IList toList, IEnumerable fromList)
		{
			ArgumentValidator.AssertNotNull(toList, "toList");
			ArgumentValidator.AssertNotNull(fromList, "fromList");

			IRangeOperations collection = toList as IRangeOperations;
			if (collection != null)
			{
				collection.AddRange(fromList);
				return;
			}

			var suspendableList = toList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}
				foreach (var item in fromList)
				{
					toList.Add(item);
				}
			}
			finally
			{
				if (suspendableList != null && !wasSuspended)
				{
					try
					{
						suspendableList.ChangeNotificationSuspended = false;
					}
					catch (Exception)
					{
						/* Suppress. */
					}
				}
			}
		}

		/// <summary>
		/// Removes the specified items from the specified list.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="fromList">The list from which items will be removed.</param>
		/// <param name="removeItems">The list of items to remove.</param>
		public static void RemoveRange<T>(this IList<T> fromList, IEnumerable<T> removeItems)
		{
			ArgumentValidator.AssertNotNull(fromList, "fromList");
			ArgumentValidator.AssertNotNull(removeItems, "removeItems");

			IRangeOperations collection = fromList as IRangeOperations;
			if (collection != null)
			{
				collection.RemoveRange(removeItems);
				return;
			}

			var suspendableList = fromList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}
				foreach (var item in removeItems)
				{
					fromList.Remove(item);
				}
			}
			finally
			{
				if (suspendableList != null && !wasSuspended)
				{
					try
					{
						suspendableList.ChangeNotificationSuspended = false;
					}
					catch (Exception)
					{
						/* Suppress. */
					}
				}
			}
		}

		/// <summary>
		/// Removes the specified items from the specified list.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="fromList">The list from which items will be removed.</param>
		/// <param name="removeItems">The list of items to remove.</param>
		public static void RemoveRange(this IList fromList, IEnumerable removeItems)
		{
			ArgumentValidator.AssertNotNull(fromList, "fromList");
			ArgumentValidator.AssertNotNull(removeItems, "removeItems");

			IRangeOperations collection = fromList as IRangeOperations;
			if (collection != null)
			{
				collection.RemoveRange(removeItems);
				return;
			}

			var suspendableList = fromList as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}
				foreach (var item in removeItems)
				{
					fromList.Remove(item);
				}
			}
			finally
			{
				if (suspendableList != null && !wasSuspended)
				{
					try
					{
						suspendableList.ChangeNotificationSuspended = false;
					}
					catch (Exception)
					{
						/* Suppress. */
					}
				}
			}
		}

		/// <summary>
		/// Determines whether the collection is null or contains no elements.
		/// </summary>
		/// <typeparam name="T">The IEnumerable type.</typeparam>
		/// <param name="enumerable">The enumerable, which may be null or empty.</param>
		/// <returns>
		/// 	<c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable == null)
			{
				return true;
			}
			/* If this is a list, use the Count property. 
			 * The Count property is O(1) while IEnumerable.Count() is O(N). */
			var collection = enumerable as ICollection<T>;
			if (collection != null)
			{
				return collection.Count < 1;
			}
			return enumerable.Any();
		}

		/// <summary>
		/// Determines whether the collection is null or contains no elements.
		/// </summary>
		/// <typeparam name="T">The IEnumerable type.</typeparam>
		/// <param name="collection">The collection, which may be null or empty.</param>
		/// <returns>
		/// 	<c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
		{
			if (collection == null)
			{
				return true;
			}
			return collection.Count < 1;
		}

		/// <summary>
		/// Examines each item in the double collection 
		/// and returns the index of the greatest value.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns></returns>
		public static int GetIndexOfGreatest(this IEnumerable<double> values)
		{
			ArgumentValidator.AssertNotNull(values, "values");

			int result = -1;
			int count = 0;
			double highest = -1;
			foreach (var d in values)
			{
				if (d > highest)
				{
					highest = d;
					result = count;
				}
				count++;
			}
			return result;
		}

		/// <summary>
		/// Removes all items from the list that do not pass the filter condition.
		/// </summary>
		/// <typeparam name="T">The generic type of the list.</typeparam>
		/// <param name="list">The list to remove from.</param>
		/// <param name="filter">The filter to evaluate each item with.</param>
		/// <returns>The removed items.</returns>
		public static IEnumerable<T> RemoveAllAndReturnItems<T>(this IList<T> list, Func<T, bool> filter)
		{
			var suspendableList = list as ISuspendChangeNotification;
			bool wasSuspended = false;

			try
			{
				if (suspendableList != null)
				{
					wasSuspended = suspendableList.ChangeNotificationSuspended;
					suspendableList.ChangeNotificationSuspended = true;
				}

				for (int i = 0; i < list.Count; i++)
				{
					if (filter(list[i]))
					{
						var item = list[i];
						list.Remove(list[i]);
						yield return item;
					}
				}
			}
			finally
			{
				if (suspendableList != null && !wasSuspended)
				{
					try
					{
						suspendableList.ChangeNotificationSuspended = false;
					}
					catch (Exception)
					{
						/* Suppress. */
					}
				}
			}
		}
	}
}
