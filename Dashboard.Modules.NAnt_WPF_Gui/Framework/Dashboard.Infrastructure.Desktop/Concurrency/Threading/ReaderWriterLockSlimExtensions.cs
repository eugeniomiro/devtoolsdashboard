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