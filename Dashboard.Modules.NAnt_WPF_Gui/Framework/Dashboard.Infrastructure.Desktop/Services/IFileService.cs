#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2009-04-15 21:44:59Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
#endregion

using System;

using Techno_Fly.Tools.Dashboard.IO;

namespace Techno_Fly.Tools.Dashboard.IO
{
	/// <summary>
	/// Used to invoke a file operation that works upon a file
	/// with the specified <code>fileName</code>.
	/// </summary>
	public delegate void FileOperationHandler(string fileName);
}

namespace Techno_Fly.Tools.Dashboard.Services
{
	/// <summary>
	/// Allows for the loading and saving of file content
	/// using delegates for invocation.
	/// </summary>
	public interface IFileService
	{
		/// <summary>
		/// Prompts the user for a file name and attempts 
		/// to invoke the specified operationHandler.
		/// <seealso cref="Save"/>
		/// </summary>
		/// <param name="fileName">Default name of the file to suggest to the user. 
		/// Can be null.</param>
		/// <param name="operationHandler">The operation handler 
		/// that carries out the actual save operation.</param>
		/// <param name="fileErrorAction">The file error action. What to do if the operation fails. 
		/// <see cref="FileErrorAction"/></param>
		/// <returns>The result of the operation indicating whether the Save was successful etc.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified <code>operationHandler</code> is null.</exception>
		FileOperationResult SaveAs(string fileName, FileOperationHandler operationHandler,
								   FileErrorAction fileErrorAction);

		/// <summary>
		/// Invokes the specified FileOperationHandler using the specified fileName. 
		/// If the specified <code>FileErrorAction</code> is <code>UseAlternative</code>
		/// and the operation fails then the user will be prompted once 
		/// to select another file. 
		/// </summary>
		/// <param name="fileName">The name that is passed to the specified operationHandler.</param>
		/// <param name="operationHandler">The handler that carries out the actual save operation.</param>
		/// <param name="fileErrorAction">The file error action. 
		/// That is, what to do if the operation fails.</param>
		/// <returns>The result of the operation indicating whether the Save was successful etc.</returns>
		/// <exception cref="ArgumentNullException">
		/// Occurs if the specified <code>operationHandler</code> is <code>null</code>.</exception>
		FileOperationResult Save(string fileName, FileOperationHandler operationHandler,
								 FileErrorAction fileErrorAction);

		/// <summary>
		/// Opens a file using the specified operation handler.
		/// </summary>
		/// <param name="operationHandler">The operation handler. 
		/// The handler that carries out the actual opening of the file.</param>
		/// <param name="fileErrorAction">The file error action. 
		/// This indicates what to do if an exception is thrown while opening the file.</param>
		/// <param name="dialogFilter">The dialog filter to display in the open file dialog.</param>
		/// <returns>The result of the operation indicating whether opening 
		/// of the file was successful etc.</returns>
		FileOperationResult Open(FileOperationHandler operationHandler,
								 FileErrorAction fileErrorAction, string dialogFilter);

		/// <summary>
		/// Opens a file using the specified operation handler.
		/// </summary>
		/// <param name="fileName">The path to the file that is to be opened.</param>
		/// <param name="operationHandler">The operation handler. 
		/// The handler that carries out the actual opening of the file.</param>
		/// <param name="fileErrorAction">The file error action. 
		/// This indicates what to do if an exception is thrown while opening the file.</param>
		/// <param name="dialogFilter">The dialog filter to display in the open file dialog.</param>
		/// <returns>The result of the operation indicating whether opening 
		/// of the file was successful etc.</returns>
		FileOperationResult Open(string fileName, FileOperationHandler operationHandler,
			FileErrorAction fileErrorAction, string dialogFilter);
	}
}
