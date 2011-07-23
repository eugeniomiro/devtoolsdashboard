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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace Techno_Fly.Tools.Dashboard.ComponentModel
{
	public enum AssignmentResult
	{
		Success, Cancelled, AlreadyAssigned, OwnerDisposed
	}

	/// <summary>
	/// This class provides an implementation of the <see cref="INotifyPropertyChanged"/>
	/// and <see cref="INotifyPropertyChanging"/> interfaces. 
	/// Extended <see cref="PropertyChangedEventArgs"/> and <see cref="PropertyChangingEventArgs"/>
	/// are used to provides the old value and new value for the property. 
	/// <seealso cref="PropertyChangedEventArgs{TProperty}"/>
	/// <seealso cref="PropertyChangingEventArgs{TProperty}"/>
	/// </summary>
	[Serializable]
	public sealed class PropertyChangeNotifier : INotifyPropertyChanged, INotifyPropertyChanging
	{
		readonly WeakReference ownerWeakReference;

		/// <summary>
		/// Gets the owner for testing purposes.
		/// </summary>
		/// <value>The owner.</value>
		internal object Owner
		{
			get
			{
				if (ownerWeakReference != null && ownerWeakReference.Target != null)
				{
					return ownerWeakReference.Target;
				}
				return null;
			}
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangeNotifier"/> class.
		/// </summary>
		/// <param name="owner">The intended sender 
		/// of the <code>PropertyChanged</code> event.</param>
		public PropertyChangeNotifier(object owner) : this(owner, true)
		{
			/* Intentionally left blank. */
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangeNotifier"/> class.
		/// </summary>
		/// <param name="owner">The intended sender 
		/// <param name="useExtendedEventArgs">If <c>true</c> the
		/// generic <see cref="PropertyChangedEventArgs{TProperty}"/>
		/// and <see cref="PropertyChangingEventArgs{TProperty}"/> 
		/// are used when raising events. 
		/// Otherwise, the non-generic types are used, and they are cached 
		/// to decrease heap fragmentation.</param>
		/// of the <code>PropertyChanged</code> event.</param>
		public PropertyChangeNotifier(object owner, bool useExtendedEventArgs)
		{
			ArgumentValidator.AssertNotNull(owner, "owner");
			ownerWeakReference = new WeakReference(owner);
			this.useExtendedEventArgs = useExtendedEventArgs;
		}

		#region event PropertyChanged

		[field: NonSerialized]
		event PropertyChangedEventHandler propertyChanged;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (OwnerDisposed)
				{
					return;
				}
				propertyChanged += value;
			}
			remove
			{
				if (OwnerDisposed)
				{
					return;
				}
				propertyChanged -= value;
			}
		}

		#region Experimental Explicit UI Thread

		bool maintainThreadAffinity = true;

		/// <summary>
		/// Gets or sets a value indicating whether events will be raised 
		/// on the thread of subscription (either the UI or ViewModel layer).
		/// <c>true</c> by default.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if raising events on the thread 
		/// of subscription; otherwise, <c>false</c>.
		/// </value>
		public bool MaintainThreadAffinity
		{
			get
			{
				return maintainThreadAffinity;
			}
			set
			{
				maintainThreadAffinity = value;
			}
		}

		#endregion

		bool blockWhenRaisingEvents = true;

		public bool BlockWhenRaisingEvents
		{
			get
			{
				return blockWhenRaisingEvents;
			}
			set
			{
				blockWhenRaisingEvents = value;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:PropertyChanged"/> event.
		/// If the owner has been GC'd then the event will not be raised.
		/// </summary>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> 
		/// instance containing the event data.</param>
		void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var owner = ownerWeakReference.Target;
			if (owner != null && propertyChanged != null)
			{
				if (maintainThreadAffinity)
				{
					Exception exception = null;
					if (blockWhenRaisingEvents)
					{
						UISynchronizationContext.Instance.InvokeAndBlockUntilCompletion(
							delegate
								{
									try
									{
										propertyChanged(owner, e);
									}
									catch (Exception ex)
									{
										exception = ex;
									}
								});
					}
					else
					{
						UISynchronizationContext.Instance.InvokeWithoutBlocking(
							delegate
							{
								try
								{
									propertyChanged(owner, e);
								}
								catch (Exception ex)
								{
									exception = ex;
								}
							});
					}
					if (exception != null)
					{
						throw exception;
					}
				}
				else
				{
					propertyChanged(owner, e);
				}
			}
		}

		#endregion

		/// <summary>
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">Name of the property. Can not be null.</param>
		/// <param name="property">A reference to the property that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public AssignmentResult Assign<TProperty>(
			string propertyName, ref TProperty property, TProperty newValue)
		{
			if (OwnerDisposed)
			{
				return AssignmentResult.OwnerDisposed;
			}

			ArgumentValidator.AssertNotNullOrEmpty(propertyName, "propertyName");
			ValidatePropertyName(propertyName);

			return AssignWithNotification(propertyName, ref property, newValue);
		}

//		/// <summary>
//		/// Assigns the specified newValue to the specified property
//		/// and then notifies listeners that the property has changed.
//		/// Assignment nor notification will occur if the specified
//		/// property and newValue are equal. 
//		/// Uses an <see cref="Expression"/> to determine the property name, 
//		/// which is slower than using the string property name overload.
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <typeparam name="TProperty">The type of the property.</typeparam>
//		/// <param name="expression">The expression that is used to derive the property name.
//		/// Should not be <code>null</code>.</param>
//		/// <param name="property">A reference to the property that is to be assigned.</param>
//		/// <param name="newValue">The value to assign the property.</param>
//		/// <exception cref="ArgumentNullException">
//		/// Occurs if the specified propertyName is <code>null</code>.</exception>
//		/// <exception cref="ArgumentException">
//		/// Occurs if the specified propertyName is an empty string.</exception>
//		public AssignmentResult Assign<T, TProperty>(
//			Expression<Func<T, TProperty>> expression, 
//			ref TProperty property, TProperty newValue)
//		{
//			if (OwnerDisposed)
//			{
//				return AssignmentResult.OwnerDisposed;
//			}
//
//			string propertyName = GetPropertyName(expression);
//			return AssignWithNotification(propertyName, ref property, newValue);
//		}

		/// <summary>
		/// Slow. Not recommended.
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="expression">The expression that is used to derive the property name.
		/// Should not be <code>null</code>.</param>
		/// <param name="property">A reference to the property that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public AssignmentResult Assign<TProperty>(
			Expression<Func<TProperty>> expression, ref TProperty property, TProperty newValue)
		{
			if (OwnerDisposed)
			{
				return AssignmentResult.OwnerDisposed;
			}

			string propertyName = GetPropertyName(expression);
			return AssignWithNotification(propertyName, ref property, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified WeakReference field ref
		/// and then notifies listeners that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// Uses an <see cref="Expression"/> to determine the property name, 
		/// which is slower than using the string property name overload.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="expression">The expression that is used to derive the property name.
		/// Should not be <code>null</code>.</param>
		/// <param name="property">A reference to the property that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public AssignmentResult Assign<TProperty>(
			Expression<Func<TProperty>> expression, 
			ref WeakReference property, TProperty newValue) where TProperty : class
		{
			if (OwnerDisposed)
			{
				return AssignmentResult.OwnerDisposed;
			}

			string propertyName = GetPropertyName(expression);
			return AssignWithNotification(propertyName, ref property, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified WeakReference field ref
		/// and then notifies listeners that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">The name of the property being changed.</param>
		/// <param name="property">A reference to the property 
		/// that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public AssignmentResult Assign<TProperty>(
			string propertyName, ref WeakReference property, TProperty newValue) 
			where TProperty : class
		{
			if (OwnerDisposed)
			{
				return AssignmentResult.OwnerDisposed;
			}

			ArgumentValidator.AssertNotNullOrEmpty(propertyName, "propertyName");
			ValidatePropertyName(propertyName);

			return AssignWithNotification(propertyName, ref property, newValue);
		}

		AssignmentResult AssignWithNotification<TProperty>(
			string propertyName, ref TProperty property, TProperty newValue)
		{
			/* Boxing may occur here. We should consider 
			 * providing some overloads for primitives. */
			if (Equals(property, newValue)) 
			{
				return AssignmentResult.AlreadyAssigned;
			}
			if (useExtendedEventArgs)
			{
				var args = new PropertyChangingEventArgs<TProperty>(propertyName, property, newValue);

				OnPropertyChanging(args);
				if (args.Cancelled)
				{
					return AssignmentResult.Cancelled;
				}

				var oldValue = property;
				property = newValue;
				OnPropertyChanged(new PropertyChangedEventArgs<TProperty>(
					propertyName, oldValue, newValue));
			}
			else
			{
				var args = RetrieveOrCreatePropertyChangingEventArgs(propertyName);
				OnPropertyChanging(args);

				var changedArgs = RetrieveOrCreatePropertyChangedEventArgs(propertyName);
				OnPropertyChanged(changedArgs);
			}

			return AssignmentResult.Success;
		}

		AssignmentResult AssignWithNotification<TProperty>(
			string propertyName, ref WeakReference field, TProperty newValue)
			where TProperty : class
		{
			var typedOldValue = field != null ? (TProperty)field.Target : null;
			/* Boxing may occur here. We should consider 
			 * providing some overloads for primitives. */
			if (Equals(typedOldValue, newValue))
			{
				return AssignmentResult.AlreadyAssigned;
			}
			if (useExtendedEventArgs)
			{
				var args = new PropertyChangingEventArgs<TProperty>(
						propertyName, typedOldValue, newValue);

				OnPropertyChanging(args);
				if (args.Cancelled)
				{
					return AssignmentResult.Cancelled;
				}

				field = newValue != null ? new WeakReference(newValue) : null;
				OnPropertyChanged(new PropertyChangedEventArgs<TProperty>(
					propertyName, typedOldValue, newValue));
			}
			else
			{
				var args = RetrieveOrCreatePropertyChangingEventArgs(propertyName);
				OnPropertyChanging(args);

				var changedArgs = RetrieveOrCreatePropertyChangedEventArgs(propertyName);
				OnPropertyChanged(changedArgs);
			}

			return AssignmentResult.Success;
		}

		readonly Dictionary<string, string> expressions = new Dictionary<string, string>();

		/// <summary>
		/// Notifies listeners that the specified property has changed.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">Name of the property. Can not be null.</param>
		/// <param name="oldValue">The old value before the change occured.</param>
		/// <param name="newValue">The new value after the change occured.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public void NotifyChanged<TProperty>(
			string propertyName, TProperty oldValue, TProperty newValue)
		{
			if (OwnerDisposed)
			{
				return;
			}
			ArgumentValidator.AssertNotNullOrEmpty(propertyName, "propertyName");
			ValidatePropertyName(propertyName);

			if (ReferenceEquals(oldValue, newValue))
			{
				return;
			}

			var args = useExtendedEventArgs
				? new PropertyChangedEventArgs<TProperty>(propertyName, oldValue, newValue)
				: RetrieveOrCreatePropertyChangedEventArgs(propertyName);

			OnPropertyChanged(args);
		}

		/// <summary>
		/// Slow. Not recommended.
		/// Notifies listeners that the property has changed.
		/// Notification will occur if the specified
		/// property and newValue are equal. 
		/// </summary>
		/// <param name="expression">The expression that is used to derive the property name.
		/// Should not be <code>null</code>.</param>
		/// <param name="oldValue">The old value of the property before it was changed.</param>
		/// <param name="newValue">The new value of the property after it was changed.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		public void NotifyChanged<TProperty>(
			Expression<Func<TProperty>> expression, TProperty oldValue, TProperty newValue)
		{
			if (OwnerDisposed)
			{
				return;
			}

			ArgumentValidator.AssertNotNull(expression, "expression");

			string name = GetPropertyName(expression);
			NotifyChanged(name, oldValue, newValue);
		}
		

		static MemberInfo GetMemberInfo<T>(Expression<Func<T>> expression)
		{
			var member = expression.Body as MemberExpression;
			if (member != null)
			{
				return member.Member;
			}

			throw new ArgumentException("MemberExpression expected.", "expression");
		}

		#region INotifyPropertyChanging Implementation

		[field: NonSerialized]
		event PropertyChangingEventHandler propertyChanging;

		public event PropertyChangingEventHandler PropertyChanging
		{
			add
			{
				if (OwnerDisposed)
				{
					return;
				}
				propertyChanging += value;
			}
			remove
			{
				if (OwnerDisposed)
				{
					return;
				}
				propertyChanging -= value;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:PropertyChanging"/> event.
		/// If the owner has been GC'd then the event will not be raised.
		/// </summary>
		/// <param name="e">The <see cref="System.ComponentModel.PropertyChangingEventArgs"/> 
		/// instance containing the event data.</param>
		void OnPropertyChanging(PropertyChangingEventArgs e)
		{
			var owner = ownerWeakReference.Target;
			if (owner != null && propertyChanging != null)
			{
				propertyChanging(owner, e);
			}
		}
		#endregion

		readonly ReaderWriterLockSlim expressionsLock = new ReaderWriterLockSlim();

		string GetPropertyName<TProperty>(Expression<Func<TProperty>> expression)
		{
			string name;
			expressionsLock.EnterUpgradeableReadLock();
			try
			{
				if (!expressions.TryGetValue(expression.ToString(), out name))
				{
					expressionsLock.EnterWriteLock();
					try
					{
						if (!expressions.TryGetValue(expression.ToString(), out name))
						{
							var memberInfo = GetMemberInfo(expression);
							if (memberInfo == null)
							{
								throw new InvalidOperationException("MemberInfo not found.");
							}
							name = memberInfo.Name;
							expressions.Add(expression.ToString(), name);
						}
					}
					finally
					{
						expressionsLock.ExitWriteLock();
					}
				}
			}
			finally
			{
				expressionsLock.ExitUpgradeableReadLock();
			}
			return name;
		}

		bool cleanupOccured;

		bool OwnerDisposed
		{
			get
			{
				/* We slightly improve performance here 
				 * by avoiding multiple Owner property calls 
				 * after the Owner has been disposed. */
				if (cleanupOccured)
				{
					return true;
				}
				var owner = Owner;
				if (owner != null)
				{
					return false;
				}
				cleanupOccured = true;
				var changedSubscribers = propertyChanged.GetInvocationList();
				foreach (var subscriber in changedSubscribers)
				{
					propertyChanged -= (PropertyChangedEventHandler)subscriber;
				}
				var changingSubscribers = propertyChanging.GetInvocationList();
				foreach (var subscriber in changingSubscribers)
				{
					propertyChanging -= (PropertyChangingEventHandler)subscriber;
				}

				/* Events should be null at this point. Nevertheless... */
				propertyChanged = null;
				propertyChanging = null;
				propertyChangedEventArgsCache.Clear();
				propertyChangingEventArgsCache.Clear();

				return true;
			}
		}

		[Conditional("DEBUG")]
		void ValidatePropertyName(string propertyName)
		{
			var propertyDescriptor = TypeDescriptor.GetProperties(Owner)[propertyName];
			if (propertyDescriptor == null)
			{
				throw new Exception(string.Format("The property '{0}' does not exist.", propertyName));
			}
		}

		bool useExtendedEventArgs;

		public bool UseExtendedEventArgs
		{
			get
			{
				return useExtendedEventArgs;
			}
			set
			{
				useExtendedEventArgs = value;
			}
		}

		readonly Dictionary<string, PropertyChangedEventArgs> propertyChangedEventArgsCache = new Dictionary<string, PropertyChangedEventArgs>();
		readonly Dictionary<string, PropertyChangingEventArgs> propertyChangingEventArgsCache = new Dictionary<string, PropertyChangingEventArgs>();

		readonly ReaderWriterLockSlim propertyChangedEventArgsCacheLock = new ReaderWriterLockSlim();
		
		PropertyChangedEventArgs RetrieveOrCreatePropertyChangedEventArgs(string propertyName)
		{
			ArgumentValidator.AssertNotNull(propertyName, "propertyName");
			var result = RetrieveOrCreateArgs(
				propertyName,
				propertyChangedEventArgsCache,
				propertyChangedEventArgsCacheLock,
				x => new PropertyChangedEventArgs(x));

			return result;
		}

		readonly ReaderWriterLockSlim propertyChangingEventArgsCacheLock = new ReaderWriterLockSlim();

		static TArgs RetrieveOrCreateArgs<TArgs>(string propertyName, Dictionary<string, TArgs> argsCache,
			ReaderWriterLockSlim lockSlim, Func<string, TArgs> createFunc)
		{
			ArgumentValidator.AssertNotNull(propertyName, "propertyName");
			TArgs result;
			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (argsCache.TryGetValue(propertyName, out result))
				{
					return result;
				}
				lockSlim.EnterWriteLock();
				try
				{
					if (argsCache.TryGetValue(propertyName, out result))
					{
						return result;
					}
					result = createFunc(propertyName);
					argsCache[propertyName] = result;
					return result;
				}
				finally
				{
					lockSlim.ExitWriteLock();
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}
		}

		PropertyChangingEventArgs RetrieveOrCreatePropertyChangingEventArgs(string propertyName)
		{
			ArgumentValidator.AssertNotNull(propertyName, "propertyName");
			var result = RetrieveOrCreateArgs(
				propertyName,
				propertyChangingEventArgsCache,
				propertyChangingEventArgsCacheLock,
				x => new PropertyChangingEventArgs(x));

			return result;
		}

		public void NotifyChanged(string propertyName)
		{
			var args = RetrieveOrCreatePropertyChangedEventArgs(propertyName);
			OnPropertyChanged(args);
		}

		public void NotifyChanged<TProperty>(
			Expression<Func<TProperty>> expression)
		{
			string propertyName = GetPropertyName(expression);
			var args = RetrieveOrCreatePropertyChangedEventArgs(propertyName);
			OnPropertyChanged(args);
		}
	}
}