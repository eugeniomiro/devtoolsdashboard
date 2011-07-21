#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-03-29 23:57:55Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Techno_Fly.Tools.Dashboard.InversionOfControl.Unity
{
	public static class UnityExtensions
	{
		static UnityServiceLocator unityServiceLocator;
		internal static bool Initialized { get; set; }

		[Obsolete("Use parameterless overload instead.")]
		public static void InitializeServiceLocator(this ServiceLocatorSingleton adapter, IUnityContainer container)
		{
			ArgumentValidator.AssertNotNull(container, "container");
			unityServiceLocator = new UnityServiceLocator(container);
			container.RegisterInstance<IUnityContainer>(container);
			container.RegisterInstance<IDependencyRegistrar>(new UnityDependencyRegistrar());
			ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
			Initialized = true;
		}

		public static void InitializeServiceLocator(this IUnityContainer container)
		{
			ArgumentValidator.AssertNotNull(container, "container");
			unityServiceLocator = new UnityServiceLocator(container);
			container.RegisterInstance<IUnityContainer>(container);
			container.RegisterInstance<IDependencyRegistrar>(new UnityDependencyRegistrar());
			ServiceLocator.SetLocatorProvider(() => unityServiceLocator);
			Initialized = true;
		}

		[Obsolete]
		public static bool IsInitialized(this ServiceLocatorSingleton adapter)
		{
			/* We can improve this by associating UnityServiceLocators and initialized values with each container. 
			 * For now, this will do. If you need this to be done, let me know. [DV] */
			return Initialized;
		}

//		public static void RegisterInstanceIfMissing<TFrom>(this IUnityContainer container, TFrom instance)
//		{
//			if (!container.IsTypeRegistered(typeof(TFrom)))
//			{
//				container.RegisterInstance<TFrom>(instance);
//			}
//		}
//
//		public static void RegisterTypeIfMissing<TFrom, TTo>(this IUnityContainer container, bool registerAsSingleton) where TTo : TFrom
//		{
//			if (!container.IsTypeRegistered(typeof(TFrom)))
//			{
//				if (registerAsSingleton)
//				{
//					container.RegisterType(typeof(TFrom), typeof(TTo), new ContainerControlledLifetimeManager());
//				}
//				else
//				{
//					container.RegisterType<TFrom, TTo>();
//				}
//			}
//		}

//		public static void RegisterDefault<T>(this IServiceRegistrar container) where T : class
//		{
//			var types = Assembly.GetExecutingAssembly().GetTypes();
//			RegisterDefault<T>(container, types, false);
//		}
//
//		public static void RegisterDefaultSingleton<T>(this IServiceRegistrar container) where T : class
//		{
//			var types = Assembly.GetExecutingAssembly().GetTypes();
//			RegisterDefault<T>(container, types, true);
//		}
//
//		static void RegisterDefault<T>(IServiceRegistrar container, IEnumerable<Type> types, bool useSingleton)
//		{
//			foreach (Type type in types)
//			{
//				Type tType = typeof(T);
//				if (!tType.IsAssignableFrom(type) || type.IsInterface)
//				{
//					continue;
//				}
//				string className = tType.Name.Substring(1);
//				if (type.Name.StartsWith("Default") || type.Name == className)
//				{
//					if (useSingleton)
//					{
//						container.RegisterInstance(typeof(T), type);
//					}
//					else
//					{
//						container.RegisterType(typeof(T), type);
//					}
//				}
//			}
//		}
	}
}
