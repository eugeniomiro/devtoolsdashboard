#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of Daniel Vaughan's Core Library

    Daniel Vaughan's Core Library is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Daniel Vaughan's Core Library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Daniel Vaughan's Core Library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-10-14 11:58:42Z</CreationDate>
</File>
*/
#endregion

using Microsoft.Practices.ServiceLocation;

namespace Techno_Fly.Tools.Dashboard.InversionOfControl
{
	public static class ServiceLocatorExtensions
	{
#if !USE_DEPRECATED_DEPENDCY_API
		public static void RegisterInstance<TService>(this IServiceLocator serviceLocator, TService service)
		{
			var registrar = Dependency.Resolve<IDependencyRegistrar>("");
			registrar.Register<TService>(service,"");
		}

		public static void RegisterType<TFrom, TTo>(this IServiceLocator serviceLocator) where TTo : TFrom
		{
			var registrar = Dependency.Resolve<IDependencyRegistrar>("");
			registrar.Register<TFrom, TTo>(false,"");
		}
#else
		public static void RegisterInstance<TService>(this IServiceLocator serviceLocator, TService service)
		{
			var registrar = ServiceLocatorSingleton.Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterInstance<TService>(service);
		}

		public static void RegisterType<TFrom, TTo>(this IServiceLocator serviceLocator) where TTo : TFrom
		{
			var registrar = ServiceLocatorSingleton.Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterType<TFrom, TTo>();
		}
#endif
	}
}
