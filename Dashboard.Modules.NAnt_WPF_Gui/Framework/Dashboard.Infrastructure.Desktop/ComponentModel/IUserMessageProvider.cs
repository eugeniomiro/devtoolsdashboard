#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-04-10 18:51:44Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
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
