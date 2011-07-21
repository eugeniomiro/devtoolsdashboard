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
	<CreationDate>2010-10-15 15:27:08Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Techno_Fly.Tools.Dashboard.Collections
{
	/// <summary>
	/// An implementation of <see cref="IEqualityComparer{T}"/>
	/// that allows comparison of two items using a Func.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class KeyEqualityComparer<T> : IEqualityComparer<T>
	{
		readonly Func<T, object> resolveKeyFunc;

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="KeyEqualityComparer&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="resolveKeyFunc">
		/// A Func to resolve the key of the instance.</param>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified resolveKeyFunc is <c>null</c>.</exception>
		public KeyEqualityComparer(Func<T, object> resolveKeyFunc)
		{
			this.resolveKeyFunc = ArgumentValidator.AssertNotNull(resolveKeyFunc, "keyResolver");;
		}

		public bool Equals(T x, T y)
		{
			return resolveKeyFunc(x).Equals(resolveKeyFunc(y));
		}

		public int GetHashCode(T obj)
		{
			var key = resolveKeyFunc(obj);
			return key != null ? key.GetHashCode() : 0;
		}
	}

	/// <summary>
	/// Provides extension methods for sequences.
	/// </summary>
	public static class SequenceExtensions
	{
		/// <summary>
		/// Compares sequences using the specified Func to resolve the key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="firstSequence">The first sequence.</param>
		/// <param name="secondSequence">The second sequence.</param>
		/// <param name="resolveKeyFunc">The Func to resolve the key.</param>
		/// <returns><c>true</c> if the sequences have matching keys, 
		/// <c>false</c> otherwise.</returns>
		public static bool SequenceEqual<T>(
			this IEnumerable<T> firstSequence, 
			IEnumerable<T> secondSequence, 
			Func<T, object> resolveKeyFunc)
		{
			ArgumentValidator.AssertNotNull(firstSequence, "firstSequence");
			ArgumentValidator.AssertNotNull(secondSequence, "secondSequence");
			return firstSequence.SequenceEqual(secondSequence, new KeyEqualityComparer<T>(resolveKeyFunc));
		}
	}
}
