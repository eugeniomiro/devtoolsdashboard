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
