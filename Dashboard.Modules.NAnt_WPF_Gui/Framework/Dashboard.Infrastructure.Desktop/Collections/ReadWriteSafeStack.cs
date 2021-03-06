﻿#region Copyleft and Copyright

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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Techno_Fly.Tools.Dashboard.Collections
{
	/// <summary>
	/// A <see cref="Stack{T}"/> that allows for reading and writing concurrently
	/// using a <see cref="System.Threading.ReaderWriterLockSlim"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ReadWriteSafeStack<T> : ICollection, IEnumerable, IEnumerable<T>
	{
		readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
		readonly Stack<T> stack = new Stack<T>();

		public IEnumerator<T> GetEnumerator()
		{
			lockSlim.EnterReadLock();
			try
			{
				return stack.ToList().GetEnumerator();
			}
			finally
			{
				lockSlim.ExitReadLock();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lockSlim.EnterReadLock();
			try
			{
				return stack.ToList().GetEnumerator();
			}
			finally
			{
				lockSlim.ExitReadLock();
			}
		}

		public void CopyTo(Array array, int index)
		{
			lockSlim.EnterWriteLock();
			try
			{
				stack.CopyTo((T[])array, index);
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		public int Count
		{
			get
			{
				lockSlim.EnterReadLock();
				try
				{
					return stack.Count;
				}
				finally
				{
					lockSlim.ExitReadLock();
				}
			}
		}

		[NonSerialized]
		private object syncRoot;


		public object SyncRoot
		{
			get
			{
				if (syncRoot == null)
				{
					Interlocked.CompareExchange(ref syncRoot, new object(), null);
				}
				return syncRoot;
			}
		}

		/// <summary>
		/// Gets the reader writer lock slim that 
		/// is used to synchronize access to the collection. [DV]
		/// </summary>
		/// <value>The reader writer lock slim.</value>
		public ReaderWriterLockSlim ReaderWriterLockSlim
		{
			get
			{
				return lockSlim;
			}
		}

		/// <summary>
		/// Gets a value indicating whether access 
		/// to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
		/// </summary>
		/// <value></value>
		/// <returns>true if access to the <see cref="T:System.Collections.ICollection"/> 
		/// is synchronized (thread safe); otherwise, false.
		/// </returns>
		public bool IsSynchronized
		{
			get
			{
				return true;
			}
		}

		public void Clear()
		{
			lockSlim.EnterWriteLock();
			try
			{
				stack.Clear();
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		public void Push(T item)
		{
			lockSlim.EnterWriteLock();
			try
			{
				stack.Push(item);
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		public T Pop()
		{
			lockSlim.EnterWriteLock();
			try
			{
				return stack.Pop();
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		public T Peek()
		{
			lockSlim.EnterWriteLock();
			try
			{
				return stack.Peek();
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}
	}
}
