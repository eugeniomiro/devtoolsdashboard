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
	<CreationDate>2010-08-15 12:24:24Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

using Techno_Fly.Tools.Dashboard.InversionOfControl;
using Techno_Fly.Tools.Dashboard.InversionOfControl.Unity;

using Microsoft.Practices.ServiceLocation;

namespace Techno_Fly.Tools.Dashboard
{
	/// <summary>
	/// This class is used to retrieve object instances, using type associations,
	/// and to create associations between types and object instances.
	/// This class is set to replace the ServiceLocatorSingleton.
	/// </summary>
	public static class Dependency
	{
#if USE_DEPRECATED_DEPENDCY_API
		/// <summary>
		/// Gets the service with the specified type.
		/// </summary>
		/// <param name="serviceType">Type of the service to retrieve.</param>
		/// <returns>The service instance.</returns>
		public static object GetService(Type serviceType)
		{
			return ServiceLocator.Current.GetService(serviceType);
		}

		public static object GetInstance(Type serviceType)
		{
			return ServiceLocator.Current.GetInstance(serviceType);
		}

		public static object GetInstance(Type serviceType, string key)
		{
			return ServiceLocator.Current.GetInstance(serviceType, key);
		}

		public static IEnumerable<object> GetAllInstances(Type serviceType)
		{
			return ServiceLocator.Current.GetAllInstances(serviceType);
		}

		public static TService GetInstance<TService>()
		{
			return ServiceLocator.Current.GetInstance<TService>();
		}

		public static bool TryGetInstance<TService>(out TService service)
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			if (registrar.IsTypeRegistered(typeof(TService)))
			{
				service = ServiceLocator.Current.GetInstance<TService>();
				return true;
			}
			service = default(TService);
			return false;
		}

		[Obsolete("Use IsRegistered")]
		public static bool IsTypeRegistered<T>()
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			return registrar.IsTypeRegistered(typeof(T));
		}

		[Obsolete("Use IsRegistered")]
		public static bool IsTypeRegistered(Type type)
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			return registrar.IsTypeRegistered(type);
		}

		public static bool IsRegistered<T>()
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			return registrar.IsTypeRegistered(typeof(T));
		}

		public static bool IsRegistered(Type type)
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			return registrar.IsTypeRegistered(type);
		}

		/// <summary>
		/// Tries to retrieve the service with the specified instance type.
		/// </summary>
		/// <param name="instanceType">Type of the </param>
		/// <returns>The located instance, or <c>null</c> if not found.</returns>
		public static object TryGetInstance(Type instanceType)
		{
			ArgumentValidator.AssertNotNull(instanceType, "instanceType");

			var registrar = GetInstance<IDependencyRegistrar>();
			if (registrar.IsTypeRegistered(instanceType))
			{
				var service = ServiceLocator.Current.GetInstance(instanceType);
				return service;
			}
			return null;
		}

		public static TService GetInstance<TService, TDefaultImplementation>()
			where TDefaultImplementation : TService
		{
			TService service;
			var registrar = GetInstance<IDependencyRegistrar>();
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

		/// <summary>
		/// Attempts to retrieve the service with the specified type. 
		/// If not found, the specified default type will be used resolved,
		/// and it will be registered.
		/// </summary>
		/// <typeparam name="TService">The type of the service.</typeparam>
		/// <typeparam name="TDefaultImplementation">The type of the default implementation.</typeparam>
		/// <returns></returns>
		public static TService GetSingleton<TService, TDefaultImplementation>()
			where TDefaultImplementation : TService
		{
			TService service;
			var registrar = GetInstance<IDependencyRegistrar>();
			if (registrar.IsTypeRegistered(typeof(TService)))
			{
				service = ServiceLocator.Current.GetInstance<TService>();
			}
			else
			{
				service = ServiceLocator.Current.GetInstance<TDefaultImplementation>();
				RegisterInstance<TService>(service);
			}
			return service;
		}

		public static TService GetInstance<TService>(string key)
		{
			return ServiceLocator.Current.GetInstance<TService>(key);
		}

		public static IEnumerable<TService> GetAllInstances<TService>()
		{
			return ServiceLocator.Current.GetAllInstances<TService>();
		}

		public static void RegisterInstance<TService>(TService service)
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			registrar.RegisterInstance<TService>(service);
		}

		public static void RegisterType<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			registrar.RegisterType<TFrom, TTo>();
		}

		public static void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			registrar.RegisterSingleton<TFrom, TTo>();
		}

		public static void RegisterSingleton(Type fromType, Type toType)
		{
			var registrar = GetInstance<IDependencyRegistrar>();
			registrar.RegisterSingleton(fromType, toType);
		}
#else
		static IDependencyRegistrar Registrar
		{
			get
			{
				return Resolve<IDependencyRegistrar>("");
			}
		}

		/// <summary>
		/// Creates a type association between one type TTo, to another type TFrom;
		/// so that when the TFrom type is requested using e.g., <c>Resolve</c>, 
		/// an instance of the TTo type is returned. 
		/// </summary>
		/// <typeparam name="TFrom">The type forming the whole or partial key 
		/// for resolving the TTo type.</typeparam>
		/// <typeparam name="TTo">The associated type.</typeparam>
		/// <param name="key">The key. Can be <c>null</c>.</param>
		/// <param name="singleton">if set to <c>true</c> 
		/// only one instance will be created of the TTo type.</param>
		public static void Register<TFrom, TTo>(bool singleton, string key) where TTo : TFrom
		{
			Registrar.Register<TFrom, TTo>(singleton, key);
		}
		
		public static void Register<TFrom>(TFrom instance, string key)
		{
			Registrar.Register<TFrom>(instance, key);
		}

		public static void Register(Type fromType, Type toType, bool singleton, string key)
		{
			Registrar.Register(fromType, toType, singleton, key);
		}

		public static void Register(Type fromType, object instance, string key)
		{
			Registrar.Register(fromType, instance, key);
		}

		public static bool IsRegistered<T>()
		{
			Type fromType = typeof(T);
			return Registrar.IsTypeRegistered(fromType);
		}

		public static bool IsRegistered(Type fromType)
		{
			return Registrar.IsTypeRegistered(fromType);
		}

		public static T Resolve<T>(string key)
		{
			return ServiceLocator.Current.GetInstance<T>(key);
		}

		public static object Resolve(Type type)
		{
			ArgumentValidator.AssertNotNull(type, "type");
			return ServiceLocator.Current.GetInstance(type);
		}

		public static object Resolve(Type type, string key)
		{
			ArgumentValidator.AssertNotNull(type, "type");
			if (key != null)
			{
				return ServiceLocator.Current.GetInstance(type, key);
			}
			return ServiceLocator.Current.GetInstance(type);
		}

		public static IEnumerable<TFrom> ResolveAll<TFrom>()
		{
			return ServiceLocator.Current.GetAllInstances<TFrom>();
		}

		public static TFrom Resolve<TFrom, TDefaultImplementation>()
			where TDefaultImplementation : TFrom
		{
			TFrom instance;
			if (Registrar.IsTypeRegistered(typeof(TFrom)))
			{
				instance = ServiceLocator.Current.GetInstance<TFrom>();
			}
			else
			{
				instance = ServiceLocator.Current.GetInstance<TDefaultImplementation>();
				Register<TFrom>(instance,"");
			}
			return instance;
		}

		public static TFrom Resolve<TFrom>(TFrom defaultImplementation)
		{
			TFrom instance;
			if (Registrar.IsTypeRegistered(typeof(TFrom)))
			{
				instance = ServiceLocator.Current.GetInstance<TFrom>();
			}
			else
			{
				instance = defaultImplementation;
				Register<TFrom>(defaultImplementation,"");
			}
			return instance;
		}

		public static bool TryResolve<T>(out T result, string key)
			where T : class
		{
			try
			{
				result = Resolve<T>(key);
			}
			catch (Exception) /* Unable to be more specific because 
							   * we don't know the container implementation. */
			{
				result = null;
			}
			return result != null;
		}

		public static bool TryResolve(Type type, out object result, string key)
		{
			try
			{
				result = Resolve(type, key);
			}
			catch (Exception) /* Unable to be more specific because 
							   * we don't know the container implementation. */
			{
				result = null;
			}
			return result != null;
		}

#if !WINDOWS_PHONE
		public static bool Initialized
		{
			get
			{
				return UnityExtensions.Initialized;
			}
			set
			{
				UnityExtensions.Initialized = value;
			}
		}
#else
		static bool initialized;

		public static bool Initialized
		{
			get
			{
				return initialized;
			}
			internal set
			{
				initialized = value;
			}
		}
#endif

#endif
	}
}
