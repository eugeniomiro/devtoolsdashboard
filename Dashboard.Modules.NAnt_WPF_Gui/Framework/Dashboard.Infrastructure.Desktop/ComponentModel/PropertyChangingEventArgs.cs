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
using System.ComponentModel;

namespace Techno_Fly.Tools.Dashboard.ComponentModel
{
	public interface IPropertyChangingEventArgs : IPropertyChangedEventArgs
	{
		void Cancel();
	}

	/// <summary>
	/// Provides data for the <see cref="INotifyPropertyChanging.PropertyChanging"/> event,
	/// exposed via the <see cref="PropertyChangeNotifier"/>.
	/// </summary>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	public sealed class PropertyChangingEventArgs<TProperty> : PropertyChangingEventArgs, IPropertyChangingEventArgs
	{
		/// <summary>
		/// Gets the value of the property before it was changed.
		/// </summary>
		/// <value>The old value.</value>
		public TProperty OldValue { get; private set; }

		/// <summary>
		/// Gets the new value of the property after it was changed.
		/// </summary>
		/// <value>The new value.</value>
		public TProperty NewValue { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// this <see cref="PropertyChangingEventArgs{TProperty}"/> has been cancelled.
		/// </summary>
		/// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
		public bool Cancelled { get; private set; }

		/// <summary>
		/// Cancels this instance so that the change will not occur.
		/// </summary>
		public void Cancel()
		{
			Cancelled = true;
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangedEventArgs{TProperty}"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property that changed.</param>
		/// <param name="oldValue">The old value before the change occurred.</param>
		/// <param name="newValue">The new value after the change occurred.</param>
		public PropertyChangingEventArgs(
			string propertyName, TProperty oldValue, TProperty newValue)
			: base(propertyName)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		#region IPropertyChangingEventArgs Members

		string IPropertyChangedEventArgs.PropertyName
		{
			get
			{
				return PropertyName;
			}
		}

		object IPropertyChangedEventArgs.OldValue
		{
			get
			{
				return OldValue;
			}
		}

		object IPropertyChangedEventArgs.NewValue
		{
			get
			{
				return NewValue;
			}
		}

		void IPropertyChangingEventArgs.Cancel()
		{
			Cancel();
		}

		#endregion
	}
}