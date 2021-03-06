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

namespace Techno_Fly.Tools.Dashboard
{
	public class LockedOperations
	{
		/// <summary>
		/// Tries to set the specified value to <c>true</c> 
		/// if it is not already <c>true</c>. Uses the specified valueLock
		/// as a memory barrier if the specified value is <c>false</c>.
		/// </summary>
		/// <param name="value">If set to <c>true</c>, the value will not be changed.</param>
		/// <param name="valueLock">The value lock.</param>
		/// <returns><c>true</c> if the value 
		/// was set to <c>true</c>; otherwise <c>false</c>.</returns>
		public static bool TrySetTrue(ref bool value, object valueLock)
		{
			ArgumentValidator.AssertNotNull(valueLock, "valueLock");
			if (!value)
			{
				lock (valueLock)
				{
					if (!value)
					{
						value = true;
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Tries to set the specified value to <c>false</c> 
		/// if it is not already <c>false</c>. Uses the specified valueLock
		/// as a memory barrier if the specified value is <c>true</c>.
		/// </summary>
		/// <param name="value">If set to <c>false</c>, the value will not be changed.</param>
		/// <param name="valueLock">The value lock.</param>
		/// <returns><c>true</c> if the value 
		/// was set to <c>false</c>; otherwise <c>false</c>.</returns>
		public static bool TrySetFalse(ref bool value, object valueLock)
		{
			ArgumentValidator.AssertNotNull(valueLock, "valueLock");
			if (value)
			{
				lock (valueLock)
				{
					if (value)
					{
						value = false;
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Performs the specified <c>Action</c> if the specified value
		/// is <c>true</c>. Uses the specified valueLock
		/// as a memory barrier, when performing the action.
		/// </summary>
		/// <param name="value">if set to <c>true</c> 
		/// then the specified Action will be performed.</param>
		/// <param name="valueLock">Used for thread safety when performing 
		/// the specified Action.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns><c>true</c> if the action was performed; 
		/// otherwise <c>false</c> is returned.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the specified action is <c>null</c>.</exception>
		/// <exception cref="Exception">
		/// Occurs if the specified action throws an exception.</exception>
		public static bool DoIfTrue(ref bool value, object valueLock, Action action)
		{
			ArgumentValidator.AssertNotNull(action, "action");
			ArgumentValidator.AssertNotNull(valueLock, "valueLock");
			if (value)
			{
				lock (valueLock)
				{
					if (value)
					{
						action();
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Performs the specified <c>Action</c> if the specified value
		/// is <c>true</c>. Uses the specified valueLock
		/// as a memory barrier, when performing the action.
		/// </summary>
		/// <param name="value">if set to <c>true</c> 
		/// then the specified Action will be performed.</param>
		/// <param name="valueLock">Used for thread safety when performing 
		/// the specified Action.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns><c>true</c> if the action was performed; 
		/// otherwise <c>false</c> is returned.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the specified action is <c>null</c>.</exception>
		/// <exception cref="Exception">
		/// Occurs if the specified action throws an exception.</exception>
		public static bool DoIfFalse(ref bool value, object valueLock, Action action)
		{
			ArgumentValidator.AssertNotNull(action, "action");
			ArgumentValidator.AssertNotNull(valueLock, "valueLock");
			if (!value)
			{
				lock (valueLock)
				{
					if (!value)
					{
						action();
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Does if true.
		/// </summary>
		/// <param name="getIfTrueFunc">The func to determine 
		/// whether to perform the specified action.</param>
		/// <param name="valueLock">Used for thread safety when performing 
		/// the specified Action.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns><c>true</c> if the action was performed; 
		/// otherwise <c>false</c> is returned.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if either action or getIfTrueFunc is <c>null</c>.</exception>
		/// <exception cref="Exception">
		/// Occurs if the specified action throws an exception.</exception>
		public static bool DoIfTrue(
			Func<bool> getIfTrueFunc, object valueLock, Action action)
		{
			ArgumentValidator.AssertNotNull(getIfTrueFunc, "getIfTrueFunc");
			ArgumentValidator.AssertNotNull(action, "action");

			if (getIfTrueFunc())
			{
				lock (valueLock)
				{
					if (getIfTrueFunc())
					{
						action();
						return true;
					}
				}
			}
			return false;
		}
	}
}
