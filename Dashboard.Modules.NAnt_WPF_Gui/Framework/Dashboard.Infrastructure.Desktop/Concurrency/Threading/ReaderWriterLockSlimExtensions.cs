#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of DanielVaughan's base library

    DanielVaughan's base library is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    DanielVaughan's base library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with DanielVaughan's base library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-08-12 14:13:55Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading;

namespace Techno_Fly.Tools.Dashboard.Concurrency
{
	public static class ReaderWriterLockSlimExtensions
	{
		public static void PerformUsingReadLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
		{
			ArgumentValidator.AssertNotNull(readerWriterLockSlim, "readerWriterLockSlim");
			ArgumentValidator.AssertNotNull(action, "action");
			try
			{
				readerWriterLockSlim.EnterReadLock();
				action();
			}
			finally
			{
				readerWriterLockSlim.ExitReadLock();
			}
		}

		public static T PerformUsingReadLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
		{
			ArgumentValidator.AssertNotNull(readerWriterLockSlim, "readerWriterLockSlim");
			ArgumentValidator.AssertNotNull(action, "action");
			try
			{
				readerWriterLockSlim.EnterReadLock();
				return action();
			}
			finally
			{
				readerWriterLockSlim.ExitReadLock();
			}
		}

		public static void PerformUsingWriteLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
		{
			ArgumentValidator.AssertNotNull(readerWriterLockSlim, "readerWriterLockSlim");
			ArgumentValidator.AssertNotNull(action, "action");
			try
			{
				readerWriterLockSlim.EnterWriteLock();
				action();
			}
			finally
			{
				readerWriterLockSlim.ExitWriteLock();
			}
		}

		public static T PerformUsingWriteLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
		{
			ArgumentValidator.AssertNotNull(readerWriterLockSlim, "readerWriterLockSlim");
			ArgumentValidator.AssertNotNull(action, "action");
			try
			{
				readerWriterLockSlim.EnterWriteLock();
				return action();
			}
			finally
			{
				readerWriterLockSlim.ExitWriteLock();
			}
		}

		public static void PerformUsingUpgradeableReadLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
		{
			ArgumentValidator.AssertNotNull(readerWriterLockSlim, "readerWriterLockSlim");
			ArgumentValidator.AssertNotNull(action, "action");
			try
			{
				readerWriterLockSlim.EnterUpgradeableReadLock();
				action();
			}
			finally
			{
				readerWriterLockSlim.ExitUpgradeableReadLock();
			}
		}

		public static T PerformUsingUpgradeableReadLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
		{
			ArgumentValidator.AssertNotNull(readerWriterLockSlim, "readerWriterLockSlim");
			ArgumentValidator.AssertNotNull(action, "action");
			try
			{
				readerWriterLockSlim.EnterUpgradeableReadLock();
				return action();
			}
			finally
			{
				readerWriterLockSlim.ExitUpgradeableReadLock();
			}
		}
	}
}