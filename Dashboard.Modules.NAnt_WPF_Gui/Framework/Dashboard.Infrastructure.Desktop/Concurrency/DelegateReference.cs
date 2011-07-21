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
	<CreationDate>2010-04-22 17:26:58Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Reflection;

namespace Techno_Fly.Tools.Dashboard.Concurrency
{
	/// <summary>
	/// Allows a <see cref="Delegate"/> to be referenced directly
	/// or using a <see cref="WeakReference"/>.
	/// </summary>
	public class DelegateReference
	{
		readonly Delegate delegateToReference;
		readonly WeakReference delegateWeakReference;
		readonly MethodInfo method;
		readonly Type delegateType;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateReference"/> class.
		/// </summary>
		/// <param name="delegateToReference">The target delegate.</param>
		/// <param name="useWeakReference">if set to <c>true</c> a weak reference 
		/// will be used for the target of the <see cref="Delegate"/>.</param>
		public DelegateReference(Delegate delegateToReference, bool useWeakReference)
		{
			ArgumentValidator.AssertNotNull(delegateToReference, "delegateToReference");

			if (useWeakReference)
			{
				delegateWeakReference = new WeakReference(delegateToReference.Target);
				method = delegateToReference.Method;
				delegateType = delegateToReference.GetType();
			}
			else
			{
				this.delegateToReference = delegateToReference;
			}
		}

		public Delegate Delegate
		{
			get
			{
				return delegateToReference ?? GetDelegate();
			}
		}

		Delegate GetDelegate()
		{
			Delegate result = null;
			if (method.IsStatic)
			{
				result = Delegate.CreateDelegate(delegateType, null, method);
			}
			else
			{
				object target = delegateWeakReference.Target;
				if (target != null)
				{
					result = Delegate.CreateDelegate(delegateType, target, method);
				}
			}
			return result;
		}
	}
}