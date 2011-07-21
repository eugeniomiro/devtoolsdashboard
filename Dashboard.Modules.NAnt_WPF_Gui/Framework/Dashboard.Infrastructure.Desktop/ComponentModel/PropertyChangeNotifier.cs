#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY <copyright holder> ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-09-06 16:53:52Z</CreationDate>
</File>
*/
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

#if !SILVERLIGHT
		[field: NonSerialized]
#endif
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
//#if WINDOWS_PHONE
//                    var dispatcher = System.Windows.Deployment.Current.Dispatcher;
//                    dispatcher.InvokeIfRequired(() => propertyChanged(owner, e));
////#elif SILVERLIGHT
////                    var dispatcher = System.Windows.Deployment.Current.Dispatcher;
////                    dispatcher.InvokeIfRequired(() => propertyChanged(owner, e));
//#else
					if (blockWhenRaisingEvents)
					{
						/* TODO: change ui syncronization API to use Boolean blocking and non blocking. */
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
//#endif
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
#if WINDOWS_PHONE
			/* Hack for GeoCoordinate comparison bug. */
			if (EqualityComparer<TProperty>.Default.Equals(property, newValue))
			{
				return AssignmentResult.AlreadyAssigned;
			}
#else
			/* Boxing may occur here. We should consider 
			 * providing some overloads for primitives. */
			if (Equals(property, newValue)) 
			{
				return AssignmentResult.AlreadyAssigned;
			}
#endif
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
#if WINDOWS_PHONE
			if (EqualityComparer<TProperty>.Default.Equals(
					typedOldValue, newValue))
			{
				return AssignmentResult.AlreadyAssigned;
			}
#else
			/* Boxing may occur here. We should consider 
			 * providing some overloads for primitives. */
			if (Equals(typedOldValue, newValue))
			{
				return AssignmentResult.AlreadyAssigned;
			}
#endif
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
		
//		static MemberInfo GetMemberInfo<T, TResult>(Expression<Func<T, TResult>> expression)
//		{
//			var member = expression.Body as MemberExpression;
//			if (member != null)
//			{
//				return member.Member;
//			}
//
//			/* TODO: Make localizable resource. */
//			throw new ArgumentException("MemberExpression expected.", "expression");
//		}

		static MemberInfo GetMemberInfo<T>(Expression<Func<T>> expression)
		{
			var member = expression.Body as MemberExpression;
			if (member != null)
			{
				return member.Member;
			}

			/* TODO: Make localizable resource. */
			throw new ArgumentException("MemberExpression expected.", "expression");
		}

		#region INotifyPropertyChanging Implementation
#if !SILVERLIGHT
		[field: NonSerialized]
#endif
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

#if SILVERLIGHT
//		readonly object expressionsLock = new object();
//
//		string GetPropertyName<T, TResult>(Expression<Func<T, TResult>> expression)
//		{
//			string name;
//			lock (expressionsLock)
//			{
//				if (!expressions.TryGetValue(expression.ToString(), out name))
//				{
//					if (!expressions.TryGetValue(expression.ToString(), out name))
//					{
//						var memberInfo = GetMemberInfo(expression);
//						if (memberInfo == null)
//						{
//							/* TODO: Make localizable resource. */
//							throw new InvalidOperationException("MemberInfo not found.");
//						}
//						name = memberInfo.Name;
//						expressions.Add(expression.ToString(), name);
//					}
//				}
//			}
//
//			return name;
//		}
		readonly object expressionsLock = new object();

		string GetPropertyName<T>(Expression<Func<T>> expression)
		{
			string name;
			if (!expressions.TryGetValue(expression.ToString(), out name))
			{
				lock (expressionsLock)
				{
					if (!expressions.TryGetValue(expression.ToString(), out name))
					{
						var memberInfo = GetMemberInfo(expression);
						if (memberInfo == null)
						{
							/* TODO: Make localizable resource. */
							throw new InvalidOperationException("MemberInfo not found.");
						}
						name = memberInfo.Name;
						expressions.Add(expression.ToString(), name);
					}
				}
			}

			return name;
		}
#else
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
								/* TODO: Make localizable resource. */
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
#endif

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
#if !SILVERLIGHT
			var propertyDescriptor = TypeDescriptor.GetProperties(Owner)[propertyName];
			if (propertyDescriptor == null)
			{
				/* TODO: Make localizable resource. */
				throw new Exception(string.Format(
					"The property '{0}' does not exist.", propertyName));
			}
#endif
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

#if SILVERLIGHT
		readonly object propertyChangingEventArgsCacheLock = new object();

		PropertyChangingEventArgs RetrieveOrCreatePropertyChangingEventArgs(string propertyName)
		{
			var result = RetrieveOrCreateEventArgs(
				propertyName, 
				propertyChangingEventArgsCacheLock, 
				propertyChangingEventArgsCache, 
				x => new PropertyChangingEventArgs(x));

			return result;
		}

		readonly object propertyChangedEventArgsCacheLock = new object();

		PropertyChangedEventArgs RetrieveOrCreatePropertyChangedEventArgs(string propertyName)
		{
			var result = RetrieveOrCreateEventArgs(
				propertyName,
				propertyChangedEventArgsCacheLock,
				propertyChangedEventArgsCache,
				x => new PropertyChangedEventArgs(x));

			return result;
		}

		static TArgs RetrieveOrCreateEventArgs<TArgs>(
			string propertyName, object cacheLock, Dictionary<string, TArgs> argsCache, 
			Func<string, TArgs> createFunc)
		{
			ArgumentValidator.AssertNotNull(propertyName, "propertyName");
			TArgs result;

			lock (cacheLock)
			{
				if (argsCache.TryGetValue(propertyName, out result))
				{
					return result;
				}

				result = createFunc(propertyName);
				argsCache[propertyName] = result;
			}
			return result;
		}
#else
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
#endif

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