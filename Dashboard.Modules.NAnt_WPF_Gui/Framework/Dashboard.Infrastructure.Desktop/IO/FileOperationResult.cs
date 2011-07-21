#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-04-15 21:43:39Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

namespace Techno_Fly.Tools.Dashboard.IO
{
	/// <summary>
	/// The result of an operation carried out on a file.
	/// </summary>
	public enum FileOperationResult
	{
		/// <summary>
		/// The operation completed successfully.
		/// </summary>
		Successful,
		/// <summary>
		/// The operation failed.
		/// </summary>
		Failed,
		/// <summary>
		/// The operation was cancelled by the user.
		/// </summary>
		Cancelled
	}
}
