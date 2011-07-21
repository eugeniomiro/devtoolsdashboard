#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-05-09 20:45:03Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

using System.IO;
using System.Text;

namespace Techno_Fly.Tools.Dashboard.IO
{
	public static class FileSystem
	{
		public static void WriteFile(string fileName, string content, Encoding encoding)
		{
			using (var writer = new StreamWriter(fileName, false, encoding))
			{
				writer.Write(content);
			}
		}

		public static void WriteFile(string fileName, string content)
		{
			WriteFile(fileName, content, Encoding.UTF8);
		}
	}
}
