#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of DanielVaughan's Core Library

    DanielVaughan's Core Library is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    DanielVaughan's Core Library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with DanielVaughan's Core Library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-10-14 16:30:13Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Techno_Fly.Tools.Dashboard.InversionOfControl.Unity
{
	/// <summary>
	/// Unity implemenation of the <see cref="ServiceLocatorImplBase" />
	/// </summary>
	public class UnityServiceLocator : ServiceLocatorImplBase
    {
        readonly IUnityContainer unityContainer;

		public UnityServiceLocator(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return unityContainer.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return unityContainer.ResolveAll(serviceType);
        }
    }
}