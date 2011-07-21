#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2008-11-03 23:19:09Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

using System;

namespace Techno_Fly.Tools.Dashboard.Concurrency
{
	/// <summary>
	/// The exception that is thrown when a thread violates
	/// a concurrency constraint, such as causing 
	/// a prohibited cross thread operation.
	/// </summary>
	public class ConcurrencyException : Exception
	{
		public ConcurrencyException()
		{
		}

		public ConcurrencyException(string message) : base(message)
		{
		}

		public ConcurrencyException(string message, Exception ex)
			: base(message, ex)
		{
		}

	}
}
