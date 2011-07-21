using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/* From: http://devplanet.com/blogs/brianr/archive/2008/09/26/thread-safe-dictionary-in-net.aspx */

namespace Techno_Fly.Tools.Dashboard.Collections
{
	public interface IReadWriteSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		/// <summary>
		/// Merge is similar to the SQL merge or upsert statement.  
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="newValue">New Value</param>
		void MergeSafe(TKey key, TValue newValue);


		/// <summary>
		/// This is a blind remove. Prevents the need to check for existence first.
		/// </summary>
		/// <param name="key">Key to Remove</param>
		void RemoveSafe(TKey key);
	}


	[Serializable]
	public class ReadWriteSafeDictionary<TKey, TValue> : IReadWriteSafeDictionary<TKey, TValue>
	{
		//This is the internal dictionary that we are wrapping
		readonly IDictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

		[NonSerialized] 
		readonly ReaderWriterLockSlim dictionaryLock = Locks.GetLockInstance(LockRecursionPolicy.NoRecursion);

		/// <summary>
		/// This is a blind remove. Prevents the need to check for existence first.
		/// </summary>
		/// <param name="key">Key to remove</param>
		public void RemoveSafe(TKey key)
		{
			using (new ReadLock(dictionaryLock))
			{
				if (dictionary.ContainsKey(key))
				{
					using (new WriteLock(dictionaryLock))
					{
						dictionary.Remove(key);
					}
				}
			}
		}
        
		/// <summary>
		/// Merge does a blind remove, and then add.  Basically a blind Upsert.  
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="newValue">New Value</param>
		public void MergeSafe(TKey key, TValue newValue)
		{
			using (new WriteLock(dictionaryLock)) // take a writelock immediately since we will always be writing
			{
				if (dictionary.ContainsKey(key))
				{
					dictionary.Remove(key);
				}

				dictionary.Add(key, newValue);
			}
		}
        
		public virtual bool Remove(TKey key)
		{
			using (new WriteLock(dictionaryLock))
			{
				return dictionary.Remove(key);
			}
		}
        
		public virtual bool ContainsKey(TKey key)
		{
			using (new ReadOnlyLock(dictionaryLock))
			{
				return dictionary.ContainsKey(key);
			}
		}

        public virtual bool TryGetValue(TKey key, out TValue value)
		{
			using (new ReadOnlyLock(dictionaryLock))
			{
				return dictionary.TryGetValue(key, out value);
			}
		}

        public virtual TValue this[TKey key]
		{
			get
			{
				using (new ReadOnlyLock(dictionaryLock))
				{
					return dictionary[key];
				}
			}
			set
			{
				using (new WriteLock(dictionaryLock))
				{
					dictionary[key] = value;
				}
			}
		}
        
		public virtual ICollection<TKey> Keys
		{
			get
			{
				using (new ReadOnlyLock(dictionaryLock))
				{
					return new List<TKey>(dictionary.Keys);
				}
			}
		}

        public virtual ICollection<TValue> Values
		{
			get
			{
				using (new ReadOnlyLock(dictionaryLock))
				{
					return new List<TValue>(dictionary.Values);
				}
			}
		}
        
		public virtual void Clear()
		{
			using (new WriteLock(dictionaryLock))
			{
				dictionary.Clear();
			}
		}
        
		public virtual int Count
		{
			get
			{
				using (new ReadOnlyLock(dictionaryLock))
				{
					return dictionary.Count;
				}
			}
		}
        
		public virtual bool Contains(KeyValuePair<TKey, TValue> item)
		{
			using (new ReadOnlyLock(dictionaryLock))
			{
				return dictionary.Contains(item);
			}
		}
        
		public virtual void Add(KeyValuePair<TKey, TValue> item)
		{
			using (new WriteLock(dictionaryLock))
			{
				dictionary.Add(item);
			}
		}
        
		public virtual void Add(TKey key, TValue value)
		{
			using (new WriteLock(dictionaryLock))
			{
				dictionary.Add(key, value);
			}
		}
        
		public virtual bool Remove(KeyValuePair<TKey, TValue> item)
		{
			using (new WriteLock(dictionaryLock))
			{
				return dictionary.Remove(item);
			}
		}
        
		public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			using (new ReadOnlyLock(dictionaryLock))
			{
				dictionary.CopyTo(array, arrayIndex);
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				using (new ReadOnlyLock(dictionaryLock))
				{
					return dictionary.IsReadOnly;
				}
			}
		}
        
		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			throw new NotSupportedException(
				"Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection");
//			using (new ReadOnlyLock(dictionaryLock))
//			{
//				return (new List<KeyValuePair<TKey, TValue>>(dictionary)).GetEnumerator();
//			}
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException(
				"Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection");
//			using (new ReadOnlyLock(dictionaryLock))
//			{
//				return (new List<KeyValuePair<TKey, TValue>>(dictionary)).GetEnumerator();
//			}
		}
	}


	public static class Locks
	{
		public static void GetReadLock(ReaderWriterLockSlim locks)
		{
			bool lockAcquired = false;
			while (!lockAcquired)
			{
				lockAcquired = locks.TryEnterUpgradeableReadLock(100);
			}
		}
        
		public static void GetReadOnlyLock(ReaderWriterLockSlim locks)
		{
			bool lockAcquired = false;
			while (!lockAcquired)
			{
				lockAcquired = locks.TryEnterReadLock(1);
			}
		}
        
		public static void GetWriteLock(ReaderWriterLockSlim locks)
		{
			bool lockAcquired = false;
			while (!lockAcquired)
			{
				lockAcquired = locks.TryEnterWriteLock(1);
			}
		}
        
		public static void ReleaseReadOnlyLock(ReaderWriterLockSlim locks)
		{
			if (locks.IsReadLockHeld)
			{
				locks.ExitReadLock();
			}
		}
        
		public static void ReleaseReadLock(ReaderWriterLockSlim locks)
		{
			if (locks.IsUpgradeableReadLockHeld)
			{
				locks.ExitUpgradeableReadLock();
			}
		}
        
		public static void ReleaseWriteLock(ReaderWriterLockSlim locks)
		{
			if (locks.IsWriteLockHeld)
			{
				locks.ExitWriteLock();
			}
		}
        
		public static void ReleaseLock(ReaderWriterLockSlim locks)
		{
			ReleaseWriteLock(locks);
			ReleaseReadLock(locks);
			ReleaseReadOnlyLock(locks);
		}

		public static ReaderWriterLockSlim GetLockInstance()
		{
			return GetLockInstance(LockRecursionPolicy.SupportsRecursion);
		}
        
		public static ReaderWriterLockSlim GetLockInstance(LockRecursionPolicy recursionPolicy)
		{
			return new ReaderWriterLockSlim(recursionPolicy);
		}
	}


	public abstract class BaseLock : IDisposable
	{
		protected ReaderWriterLockSlim lockSlims;

		protected BaseLock(ReaderWriterLockSlim lockSlims)
		{
			this.lockSlims = lockSlims;
		}
        
		public abstract void Dispose();
	}


	public class ReadLock : BaseLock
	{
		public ReadLock(ReaderWriterLockSlim lockSlims)
			: base(lockSlims)
		{
			Locks.GetReadLock(this.lockSlims);
		}
        
		public override void Dispose()
		{
			Locks.ReleaseReadLock(lockSlims);
		}
	}


	public class ReadOnlyLock : BaseLock
	{
		public ReadOnlyLock(ReaderWriterLockSlim locks)
			: base(locks)
		{
			Locks.GetReadOnlyLock(lockSlims);
		}
        
		public override void Dispose()
		{
			Locks.ReleaseReadOnlyLock(lockSlims);
		}
	}


	public class WriteLock : BaseLock
	{
		public WriteLock(ReaderWriterLockSlim locks)
			: base(locks)
		{
			Locks.GetWriteLock(lockSlims);
		}
        
		public override void Dispose()
		{
			Locks.ReleaseWriteLock(lockSlims);
		}
	}
}
