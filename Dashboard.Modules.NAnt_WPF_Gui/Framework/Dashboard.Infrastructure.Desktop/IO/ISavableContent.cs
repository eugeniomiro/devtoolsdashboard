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

namespace Techno_Fly.Tools.Dashboard.IO
{
	/// <summary>
	/// Provides members used during file save operations.
	/// </summary>
	public interface ISavableContent
	{
		/// <summary>
		/// Gets a value indicating whether this view is capable 
		/// of being saved at any time.
		/// If <code>false</code> it indicates that the content 
		/// never has the ability to save itself.
		/// </summary>
		/// <value><c>true</c> if the content can be saved; otherwise, <c>false</c>.</value>
		bool CanSave { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ISavableContent"/> has been modified 
		/// and requires saving.
		/// </summary>
		/// <value><c>true</c> if dirty; otherwise, <c>false</c>.</value>
		bool Dirty { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="ISavableContent"/> has been saved. 
		/// That is, the file was previously saved. 
		/// </summary>
		/// <value><c>true</c> if saved; otherwise, <c>false</c>.</value>
		bool NewFile { get; }

		/// <summary>
		/// Saves the content.
		/// </summary>
		/// <returns>A value indicating the result of the save operation.</returns>
		FileOperationResult Save(FileErrorAction fileErrorAction);

		/// <summary>
		/// Prompts the user for a file name and saves the content.
		/// </summary>
		/// <returns>A value indicating the result of the save operation.</returns>
		FileOperationResult SaveAs(FileErrorAction fileErrorAction);

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		string FileName { get; set; }

		//		/// <summary>
		//		/// Gets a value indicating whether the content must be saved using a different file name.
		//		/// </summary>
		//		/// <value><c>true</c> if read only; otherwise, <c>false</c>.</value>
		//		bool ReadOnly { get; }
	}
}
