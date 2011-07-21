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
	<CreationDate>2009-10-14 16:43:29Z</CreationDate>
</File>
*/
#endregion

using System;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Techno_Fly.Tools.Dashboard.InversionOfControl.Unity
{
	/// <summary>
	/// Default implementation of <see cref="IDependencyRegistrar"/>
	/// </summary>
	public sealed class UnityDependencyRegistrar : UnityContainerExtension, IDependencyRegistrar
	{
		new IUnityContainer Container
		{
			get
			{
				return ServiceLocator.Current.GetInstance<IUnityContainer>();
			}
		}
#if USE_DEPRECATED_DEPENDCY_API
		public void RegisterInstance<TService>(TService service)
		{
			Container.RegisterInstance<TService>(service);
		}

		public void RegisterInstance(Type serviceType, object service)
		{
			Container.RegisterInstance(serviceType, service);
		}

		public void RegisterType<TFrom, TTo>() where TTo : TFrom
		{
			Container.RegisterType<TFrom, TTo>();
		}

		public bool IsTypeRegistered(Type type)
		{
			Container.AddExtension(this);
			var policy = Context.Policies.Get<IBuildKeyMappingPolicy>(new NamedTypeBuildKey(type));
			return policy != null;
		}

		public void RegisterSingleton<TFrom, TTo>() where TTo : TFrom
		{
			Container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
		}

		public void RegisterSingleton(Type fromType, Type toType)
		{
			Container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
		}
#else
		public bool IsTypeRegistered(Type type)
		{
			return Container.IsRegistered(type);
		}

		public void Register<TFrom, TTo>(bool singleton, string key) where TTo : TFrom
		{
			if (singleton)
			{
				Container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
			}
			else
			{
				Container.RegisterType<TFrom, TTo>();
			}
		}

		public void Register(Type fromType, Type toType, bool singleton, string key)
		{
			if (singleton)
			{
				Container.RegisterType(fromType, toType, key, new ContainerControlledLifetimeManager());
			}
			else
			{
				Container.RegisterType(fromType, toType, key);
			}
			
		}

		public void Register<TFrom>(TFrom instance, string key)
		{
			Container.RegisterInstance<TFrom>(key, instance);
		}

		public void Register(Type type, object instance, string key)
		{
			Container.RegisterInstance(type, key, instance);
		}
#endif
		protected override void Initialize()
		{
			/* Intentionally left blank. */
		}
	}
}
