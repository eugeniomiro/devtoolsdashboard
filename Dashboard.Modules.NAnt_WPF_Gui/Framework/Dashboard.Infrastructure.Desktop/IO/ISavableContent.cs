#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-05-03 16:19:03Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
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
