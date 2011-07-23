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

using System;

namespace Techno_Fly.Tools.Dashboard.IO
{

    /// <summary>
    /// 
    /// </summary>
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
