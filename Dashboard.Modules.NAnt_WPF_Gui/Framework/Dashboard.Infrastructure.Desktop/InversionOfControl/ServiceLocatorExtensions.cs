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
