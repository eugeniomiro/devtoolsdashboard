using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

namespace Techno_Fly.Tools.Dashboard.InversionOfControl
{
	public class ServiceLocatorAdapter
	{
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

		public TService GetInstance<TService>(string key)
		{
			return ServiceLocator.Current.GetInstance<TService>(key);
		}

		public IEnumerable<TService> GetAllInstances<TService>()
		{
			return ServiceLocator.Current.GetAllInstances<TService>();
		}

#if !USE_DEPRECATED_DEPENDCY_API
		public void RegisterInstance<TFrom>(TFrom service)
		{
			var registrar = Dependency.Resolve<IDependencyRegistrar>("");
			registrar.Register<TFrom>(service,"");
		}

		public void RegisterType<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = Dependency.Resolve<IDependencyRegistrar>("");
			registrar.Register<TFrom, TTo>(false,"");
		}
#else
		public void RegisterInstance<TService>(TService service)
		{
			var registrar = ServiceLocatorSingleton.Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterInstance<TService>(service);
		}

		public void RegisterType<TFrom, TTo>() where TTo : TFrom
		{
			var registrar = ServiceLocatorSingleton.Instance.GetInstance<IDependencyRegistrar>();
			registrar.RegisterType<TFrom, TTo>();
		}

#endif

		public object ServiceLocatorActual
		{
			get
			{
				return ServiceLocator.Current;
			}
		}
	}
}
