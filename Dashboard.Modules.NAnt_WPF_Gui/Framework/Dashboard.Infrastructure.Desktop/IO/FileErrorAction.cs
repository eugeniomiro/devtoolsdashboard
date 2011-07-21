#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-04-15 21:44:42Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

namespace Techno_Fly.Tools.Dashboard.IO
{
	/// <summary>
	/// What action to take if a file operation 
	/// throws an exception.
	/// </summary>
	public enum FileErrorAction
	{
		/// <summary>
		/// Inform the user, but do not perform any other action.
		/// </summary>
		InformOnly,
		/// <summary>
		/// Fail silently.
		/// </summary>
		None,
		/// <summary>
		/// Request an alternate file location.
		/// </summary>
		UseAlternative
	}
}
