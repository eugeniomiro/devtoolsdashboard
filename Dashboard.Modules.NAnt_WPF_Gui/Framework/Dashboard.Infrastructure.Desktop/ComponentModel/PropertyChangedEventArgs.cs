#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-08-01 13:01:34Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

using System.ComponentModel;

namespace Techno_Fly.Tools.Dashboard.ComponentModel
{
	public interface IPropertyChangedEventArgs
	{
		string PropertyName { get; }
		object OldValue { get; }
		object NewValue { get; }
	}

	/// <summary>
	/// Provides data for the <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
	/// exposed via the <see cref="PropertyChangeNotifier"/>.
	/// </summary>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	public sealed class PropertyChangedEventArgs<TProperty> : PropertyChangedEventArgs, IPropertyChangedEventArgs
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

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangedEventArgs{TProperty}"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property that changed.</param>
		/// <param name="oldValue">The old value before the change occurred.</param>
		/// <param name="newValue">The new value after the change occurred.</param>
		public PropertyChangedEventArgs(
			string propertyName, TProperty oldValue, TProperty newValue) 
			: base(propertyName)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}