#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of DanielVaughan's base library

	DanielVaughan's base library is free software: you can redistribute it and/or modify
	it under the terms of the GNU Lesser General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	DanielVaughan's base library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with DanielVaughan's base library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-04-10 20:33:35Z</CreationDate>
</File>
*/
#endregion

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Techno_Fly.Tools.Dashboard.ComponentModel
{
	/// <summary>
	/// A base class for property change notification.
	/// <seealso cref="PropertyChangeNotifier"/>.
	/// </summary>
	[Serializable]
	public abstract class NotifyPropertyChangeBase : INotifyPropertyChanged, INotifyPropertyChanging
	{
		[field: NonSerialized]
		PropertyChangeNotifier notifier;

		readonly bool useExtendedEventArgs = true;

		/// <summary>
		/// Gets the notifier. It is lazy loaded.
		/// </summary>
		/// <value>The notifier.</value>
		protected PropertyChangeNotifier Notifier
		{
			get
			{
				/* We use lazy instantiation because hooking up the events 
				 * for many instances is expensive. */
				if (notifier == null)
				{
					InitializePropertyChangeNotifier();
				}
				return notifier;
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		void InitializePropertyChangeNotifier()
		{
			if (notifier == null)
			{
				notifier = new PropertyChangeNotifier(this, useExtendedEventArgs);
			}
		}

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			Notifier.NotifyChanged(propertyName);
		}

		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="propertyExpression">An expression to retrieve the property. E.g., () => Property </param>
		/// <example>OnPropertyChanged(() => Name);</example>
		protected virtual void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
		{
			Notifier.NotifyChanged(propertyExpression);
		}

//		[OnDeserializing]
//		void OnDeserializing(StreamingContext context)
//		{
//			Initialize();
//		}

		/// <summary>
		/// Assigns the specified newValue to the specified property
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
		/// <param name="field">A reference to the field that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		protected AssignmentResult Assign<TProperty>(
			Expression<Func<TProperty>> expression,
			ref TProperty field, TProperty newValue)
		{
			return Notifier.Assign(expression, ref field, newValue);
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
		/// <param name="fieldReference">A reference to the field <see cref="WeakReference"/> 
		/// that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		protected AssignmentResult Assign<TProperty>(
			Expression<Func<TProperty>> expression,
			ref WeakReference fieldReference, TProperty newValue) where TProperty : class
		{
			return Notifier.Assign(expression, ref fieldReference, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified property
		/// and then notifies listeners that the property has changed.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">Name of the property. Can not be null.</param>
		/// <param name="field">A reference to the property that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		protected AssignmentResult Assign<TProperty>(
			string propertyName, ref TProperty field, TProperty newValue)
		{
			return Notifier.Assign(propertyName, ref field, newValue);
		}

		/// <summary>
		/// Assigns the specified newValue to the specified WeakReference field ref
		/// and then notifies listeners that the property has changed.
		/// Assignment nor notification will occur if the specified
		/// property and newValue are equal. 
		/// Uses an <see cref="Expression"/> to determine the property name, 
		/// which is slower than using the string property name overload.
		/// </summary>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="propertyName">The name of the property being changed.</param>
		/// <param name="fieldReference">A reference to the field <see cref="WeakReference"/> 
		/// that is to be assigned.</param>
		/// <param name="newValue">The value to assign the property.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified propertyName is <code>null</code>.</exception>
		/// <exception cref="ArgumentException">
		/// Occurs if the specified propertyName is an empty string.</exception>
		protected AssignmentResult Assign<TProperty>(
			string propertyName, ref WeakReference fieldReference, TProperty newValue)
			where TProperty : class
		{
			return Notifier.Assign(propertyName, ref fieldReference, newValue);
		}

		/// <summary>
		/// When deserialization occurs fields are not instantiated,
		/// therefore we must instantiate the notifier.
		/// </summary>
//		[MethodImpl(MethodImplOptions.Synchronized)]
//		void Initialize()
//		{
//		}

		public NotifyPropertyChangeBase()
		{
//			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotifyPropertyChangeBase"/> class.
		/// </summary>
		/// <param name="useExtendedEventArgs">if set to <c>true</c> 
		/// the PropertyChangeNotifier will use extended event args.</param>
		protected NotifyPropertyChangeBase(bool useExtendedEventArgs)
		{
			this.useExtendedEventArgs = useExtendedEventArgs;
		}

		#region Property change notification

		/// <summary>
		/// Occurs when a property value changes.
		/// <seealso cref="PropertyChangeNotifier"/>
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				Notifier.PropertyChanged += value;
			}
			remove
			{
				Notifier.PropertyChanged -= value;
			}
		}

		/// <summary>
		/// Occurs when a property value is changing.
		/// <seealso cref="PropertyChangeNotifier"/>
		/// </summary>
		public event PropertyChangingEventHandler PropertyChanging
		{
			add
			{
				Notifier.PropertyChanging += value;
			}
			remove
			{
				Notifier.PropertyChanging -= value;
			}
		}

		#endregion
	}
}
