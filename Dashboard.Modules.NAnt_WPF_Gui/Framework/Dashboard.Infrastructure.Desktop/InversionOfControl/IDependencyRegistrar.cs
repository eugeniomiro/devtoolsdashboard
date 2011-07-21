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
	<CreationDate>2009-10-14 16:34:20Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Techno_Fly.Tools.Dashboard.InversionOfControl
{
	/// <summary>
	/// Allows setting of type to type, or type to instance associations
	/// in the service container.
	/// </summary>
	public interface IDependencyRegistrar
	{
		bool IsTypeRegistered(Type type);
#if USE_DEPRECATED_DEPENDCY_API
		[Obsolete]
		void RegisterInstance<TService>(TService service);
		[Obsolete]
		void RegisterInstance(Type serviceType, object service);
		[Obsolete]
		void RegisterType<TFrom, TTo>() where TTo : TFrom;
		[Obsolete]
		void RegisterSingleton<TFrom, TTo>() where TTo : TFrom;
		[Obsolete]
		void RegisterSingleton(Type fromType, Type toType);
#else
        //void Register<TFrom, TTo>(bool singleton = false, string key = null) where TTo : TFrom;
        //void Register(Type fromType, Type toType, bool singleton = false, string key = null);
        //void Register<TFrom>(TFrom instance, string key = null);
        //void Register(Type type, object instance, string key = null);
        void Register<TFrom, TTo>(bool singleton, string key) where TTo : TFrom;
		void Register(Type fromType, Type toType, bool singleton, string key);
		void Register<TFrom>(TFrom instance, string key);
		void Register(Type type, object instance, string key);
#endif
	}
}
