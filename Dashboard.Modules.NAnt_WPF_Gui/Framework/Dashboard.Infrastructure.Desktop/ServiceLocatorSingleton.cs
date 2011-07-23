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

using System;
using System.Collections.Generic;

using Techno_Fly.Tools.Dashboard.InversionOfControl;

using Microsoft.Practices.ServiceLocation;

namespace Techno_Fly.Tools.Dashboard
{
	[Obsolete("Use Dependency instead.")]
	/// <summary>
	/// Helper class to resolve a Unity container
	/// that must be initialized once.
	/// </summary>
	public sealed class ServiceLocatorSingleton
	{
		#region Singleton implementation

		ServiceLocatorSingleton()
		{
		}

		public static ServiceLocatorSingleton Instance
		{
			get
			{
				return Nested.instance;
			}
		}

		class Nested
		{
			// Explicit static constructor to tell C# compiler
			// not to mark type as beforefieldinit
			static Nested()
			{
			}

			internal static readonly ServiceLocatorSingleton instance = new ServiceLocatorSingleton();
		}

		#endregion
 
		public object GetService(Type serviceType)
		{
			return ServiceLocator.Current.GetService(serviceType);
		}

		public object GetInstance(Type serviceType)
		{
			return ServiceLocator.Current.GetInstance(serviceType);
		}

		public object GetInstance(Type serviceType, string key)
		{
			return ServiceLocator.Current.GetInstance(serviceType, key);
		}

		public IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return ServiceLocator.Current.GetAllInstances(serviceType);
		}

		public TService GetInstance<TService>()
		{
			return ServiceLocator.Current.GetInstance<TService>();
		}

		public bool TryGetInstance<TService>(out TService service)
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			if (registrar.IsTypeRegistered(typeof(TService)))
			{
				service = ServiceLocator.Current.GetInstance<TService>();
				return true;
			}
			service = default(TService);
			return false;
		}

		public bool IsTypeRegistered<T>()
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			return registrar.IsTypeRegistered(typeof(T));
		}

		public bool IsTypeRegistered(Type type)
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			return registrar.IsTypeRegistered(type);
		}

		/// <summary>
		/// Tries to retrieve the service with the specified instance type.
		/// </summary>
		/// <param name="instanceType">Type of the instance.</param>
		/// <returns>The located instance, or <c>null</c> if not found.</returns>
		public object TryGetInstance(Type instanceType)
		{
			ArgumentValidator.AssertNotNull(instanceType, "instanceType");

			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			if (registrar.IsTypeRegistered(instanceType))
			{
				var service = ServiceLocator.Current.GetInstance(instanceType);
				return service;
			}
			return null;
		}

		public TService GetInstance<TService, TDefaultImplementation>() 
			where TDefaultImplementation : TService
		{
			TService service;
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			if (registrar.IsTypeRegistered(typeof(TService)))
			{
				service = ServiceLocator.Current.GetInstance<TService>();
			}
			else
			{
				service = ServiceLocator.Current.GetInstance<TDefaultImplementation>();
			}
			return service;
		}

		public TService GetInstance<TService>(string key)
		{
			return ServiceLocator.Current.GetInstance<TService>(key);
		}

		public IEnumerable<TService> GetAllInstances<TService>()
		{
			return ServiceLocator.Current.GetAllInstances<TService>();
		}

#if USE_DEPRECATED_DEPENDCY_API
		public void RegisterInstance<TService>(TService service)
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterInstance<TService>(service);
		}

		public void RegisterType<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterType<TFrom, TTo>();
		}

		public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterSingleton<TFrom, TTo>();
		}

		public void RegisterSingleton(Type fromType, Type toType)
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterSingleton(fromType, toType);
		}
#else
		public void RegisterInstance<TService>(TService service)
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.Register<TService>(service,"");
		}

		public void RegisterType<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.Register<TFrom, TTo>(false,"");
		}

		public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.Register<TFrom, TTo>(true,"");
		}

		public void RegisterSingleton(Type fromType, Type toType)
		{
			var registrar = Instance.GetInstance<IDependencyRegistrar>();
			registrar.Register(fromType, toType, true,"");
		}
#endif
	}
}
