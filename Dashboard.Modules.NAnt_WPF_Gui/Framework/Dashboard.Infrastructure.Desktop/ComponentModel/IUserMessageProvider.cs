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

namespace Techno_Fly.Tools.Dashboard.ComponentModel
{
	/// <summary>
	/// Allows an instance that implements this interface
	/// to provide information suitable for displaying directly 
	/// to a lay user.
	/// </summary>
	public interface IUserMessageProvider
	{
		/// <summary>
		/// Gets or sets the localized user message, that is appropriate 
		/// for displaying to a user.
		/// </summary>
		/// <value>The user message.</value>
		string UserMessage
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the UserMessage is <code>null</code>.
		/// </summary>
		/// <value><c>true</c> if the UserMessage is not <code>null</code>; 
		/// otherwise, <c>false</c>.</value>
		bool UserMessagePresent
		{
			get;
		}
	}
}
