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

using System.Windows;

namespace Techno_Fly.Tools.Dashboard.Concurrency
{
	/// <summary>
	/// The default implementation for an <see cref="IProvider{T}"/> 
	/// providing an <see cref="ISynchronizationContext"/> instance.
	/// </summary>
	public class DualContextProvider : IProvider<ISynchronizationContext>
	{
		public ISynchronizationContext ProvidedItem
		{
			get
			{
#if SILVERLIGHT
				if (Deployment.Current.Dispatcher.CheckAccess())
				{
					return UISynchronizationContext.Instance;
				}
				return ModelSynchronizationContext.Instance;
#else
				if (Application.Current.Dispatcher.CheckAccess())
				{
					return UISynchronizationContext.Instance;
				}
				return ModelSynchronizationContext.Instance;
#endif
			}
		}
	}
}
