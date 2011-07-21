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
	<CreationDate>2009-09-06 16:54:30Z</CreationDate>
</File>
*/
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