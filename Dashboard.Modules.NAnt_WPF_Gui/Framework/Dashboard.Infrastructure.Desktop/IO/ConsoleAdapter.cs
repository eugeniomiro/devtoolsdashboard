#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-04-11 21:35:16Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

using System;

namespace Techno_Fly.Tools.Dashboard.IO
{
	/* TODO: [DV] Comment. */
	public class ConsoleAdapter : ICommandLine
	{
		static readonly object consoleLock = new object();

		public object SyncLock
		{
			get
			{
				return consoleLock;
			}
		}

		public void Write(CommandLineText text)
		{
			ArgumentValidator.AssertNotNull(text, "text");
			lock (consoleLock)
			{
				SetConsoleStyle(text.Style);
				Console.Write(text.Message);
				SetConsoleStyle(CommandLineStyle.Normal);
			}
		}

		public void WriteLine(CommandLineText text)
		{
			ArgumentValidator.AssertNotNull(text, "text");
			lock (consoleLock)
			{
				SetConsoleStyle(text.Style);
				Console.WriteLine(text.Message);
				SetConsoleStyle(CommandLineStyle.Normal);
			}
		}

		static void SetConsoleStyle(CommandLineStyle style)
		{
			switch (style)
			{
				case CommandLineStyle.Normal:
					Console.ResetColor();
					break;
				case CommandLineStyle.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case CommandLineStyle.Warn:
					Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
				case CommandLineStyle.Title:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case CommandLineStyle.MenuItem:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				default:
					Console.ResetColor();
					break;
			}
		}

		public string ReadLine()
		{
			lock (consoleLock)
			{
				return Console.ReadLine();
			}
		}

		public char ReadKey()
		{
			lock (consoleLock)
			{
				var info = Console.ReadKey();
				return info.KeyChar;
			}
		}

		public void ClearScreen()
		{
			lock (consoleLock)
			{
				Console.Clear();
			}
		}
	}
}
