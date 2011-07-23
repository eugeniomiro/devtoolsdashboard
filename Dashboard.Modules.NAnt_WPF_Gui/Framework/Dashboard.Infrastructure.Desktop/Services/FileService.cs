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
using System.IO;
using System.Windows;
using System.Windows.Threading;

using Microsoft.Win32;

using log4net;
using Techno_Fly.Tools.Dashboard.ComponentModel;
using Techno_Fly.Tools.Dashboard.Gui;
using Techno_Fly.Tools.Dashboard.IO;

namespace Techno_Fly.Tools.Dashboard.Services
{
	/// <summary>
	/// Default implementation of <see cref="IFileService"/>.
	/// </summary>
	public class FileService : IFileService
	{
		static readonly ILog log = LogManager.GetLogger(typeof(FileService));

		public FileOperationResult SaveAs(string fileName, FileOperationHandler operationHandler,
			FileErrorAction fileErrorAction)
		{
			ArgumentValidator.AssertNotNull(operationHandler, "operationHandler");

			var shell = Dependency.Resolve<IMainWindow>("") as Window;
			var dialog = new SaveFileDialog();

			dialog.FileName = fileName;
			if (!dialog.ShowDialog(shell).Value)
			{
				return FileOperationResult.Cancelled;
			}
			return Save(dialog.FileName, operationHandler, fileErrorAction);
		}

		public FileOperationResult Save(string fileName, FileOperationHandler operationHandler, 
			FileErrorAction fileErrorAction)
		{
			ArgumentValidator.AssertNotNullOrEmpty(fileName, "fileName");
			ArgumentValidator.AssertNotNull(operationHandler, "operationHandler");

			FileErrorAction actualFileErrorAction = fileErrorAction;

			string userMessage = null;
			bool ioExceptionOccured = false;
			try
			{
				operationHandler(fileName);
				return FileOperationResult.Successful;
			}
			catch (IOException ex)
			{
				log.Info("Unable to save file: " + fileName, ex);
				userMessage = "A problem occured saving the file.";
				ioExceptionOccured = true;
			}
			catch (Exception ex)
			{
				log.Info("Unable to save file: " + fileName, ex);
				var userMessageException = ex as IUserMessageProvider;
				if (userMessageException != null && userMessageException.UserMessagePresent)
				{
					userMessage = userMessageException.UserMessage;
				}
			}

			if (!ioExceptionOccured)
			{
				if (fileErrorAction == FileErrorAction.UseAlternative)
				{	/* If this wasn't an IOException then we do not allow the user to select an alternative. */
					actualFileErrorAction = FileErrorAction.InformOnly;
				}
			}

			var result = FileOperationResult.Failed;

            //var messageService = Dependency.Resolve<IMessageService>("");

            //switch (actualFileErrorAction)
            //{
            //    case FileErrorAction.UseAlternative:
            //        messageService.ShowError(userMessage, StringResources.Services_FileService_UnableToSaveFile);
            //        do
            //        {
            //            result = SaveAs(fileName, operationHandler, FileErrorAction.InformOnly);
            //        } while (result != FileOperationResult.Successful && result != FileOperationResult.Cancelled);
            //        break;
            //    case FileErrorAction.InformOnly:
            //        messageService.ShowError(userMessage, StringResources.Services_FileService_UnableToSaveFile);
            //        break;
            //}
			return result;
		}

		public FileOperationResult Open(FileOperationHandler operationHandler,
			FileErrorAction fileErrorAction, string dialogFilter)
		{
			ArgumentValidator.AssertNotNull(operationHandler, "operationHandler");
			return Open(null, operationHandler, fileErrorAction, dialogFilter);
		}

		public FileOperationResult Open(string filePath, FileOperationHandler operationHandler,
			FileErrorAction fileErrorAction, string dialogFilter)
		{
			ArgumentValidator.AssertNotNull(operationHandler, "operationHandler");

			string fileNameToUse = filePath;
			if (!File.Exists(filePath))
			{
				var shell = Dependency.Resolve<IMainWindow>("") as Window;
				FileOperationResult result = FileOperationResult.Successful;
				var dispatcher = Dependency.Resolve<Dispatcher>("");
				dispatcher.InvokeIfRequired(delegate
				{
					var dialog = new OpenFileDialog();
					dialog.Title = "Choose file name";
					dialog.Filter = dialogFilter;
					if (!dialog.ShowDialog(shell).Value)
					{
						result = FileOperationResult.Cancelled;
						return;
					}
					fileNameToUse = dialog.FileName;
				});
				if (result != FileOperationResult.Successful)
				{
					return result;
				}
			}

			//string userMessage = null;
			try
			{
				operationHandler(fileNameToUse);
				return FileOperationResult.Successful;
			}
            //catch (MementoIncompatibilityException ex)
            //{
            //    userMessage = string.Format(StringResources.Services_FileService_FileCreatedInNewerVersion, fileNameToUse);
            //    log.Info("Unable to open file: " + filePath, ex);
            //}
            catch (Exception ex)
            {
                log.Info("Unable to open file: " + filePath, ex);
            }

            //if (userMessage == null)
            //{
            //    userMessage = string.Format(StringResources.Services_FileService_UnableToOpenFile, fileNameToUse);
            //}

            //var messageService = Dependency.Resolve<IMessageService>("");

            //switch (fileErrorAction)
            //{
            //    case FileErrorAction.UseAlternative:
            //        messageService.ShowWarning(userMessage);
            //        var result = Open(null, operationHandler, FileErrorAction.UseAlternative, dialogFilter);
            //        return result;
            //    case FileErrorAction.InformOnly:
            //        messageService.ShowWarning(userMessage);
            //        break;
            //}
			return FileOperationResult.Failed;
		}

	}
}
