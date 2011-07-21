#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-04-11 21:29:09Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion


namespace Techno_Fly.Tools.Dashboard.IO
{
	public enum CommandLineStyle
	{
		Normal,
		Warn,
		Error,
		Title,
		MenuItem
	}

	public class CommandLineText
	{
		public string Message { get; set; }
		public CommandLineStyle Style { get; set; }

		public CommandLineText(string message, CommandLineStyle style)
		{
			ArgumentValidator.AssertNotNull(message, "message");
			Message = message;
			Style = style;
		}

		public CommandLineText(string message)
		{
			ArgumentValidator.AssertNotNull(message, "message");
			Message = message;
			Style = CommandLineStyle.Normal;
		}
	}
	/* TODO: [DV] Comment. */
	public interface ICommandLine
	{
		object SyncLock { get; }
		void Write(CommandLineText text);
		void WriteLine(CommandLineText text);
		string ReadLine();
		char ReadKey();
		void ClearScreen();
	}
}
