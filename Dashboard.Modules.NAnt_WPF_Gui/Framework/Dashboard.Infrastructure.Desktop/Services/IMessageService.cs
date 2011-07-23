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

using System.ServiceModel;

namespace Techno_Fly.Tools.Dashboard.Services
{
	/// <summary>
	/// The result from a user when prompted with a question 
	/// that has three options: Yes, No, and Cancel.
	/// </summary>
	public enum YesNoCancelQuestionResult
	{
		/// <summary>
		/// The user selected 'yes.'
		/// </summary>
		Yes,
		/// <summary>
		/// The user selected 'no.'
		/// </summary>
		No,
		/// <summary>
		/// The user selected 'cancel.'
		/// </summary>
		Cancel
	}

	/// <summary>
	/// The result from a user when prompted with a question 
	/// that has two options: Ok and Cancel.
	/// </summary>
	public enum OkCancelQuestionResult
	{
		/// <summary>
		/// The user selected 'Ok.'
		/// </summary>
		Ok,
		/// <summary>
		/// The user selected 'cancel.'
		/// </summary>
		Cancel
	}

	/// <summary>
	/// Is used to indicate the which messages are to shown 
	/// depending on the user's filter level.
	/// </summary>
	public enum MessageImportance
	{
		/// <summary>
		/// All messages always shown.
		/// </summary>
		Low = 0,
		/// <summary>
		/// Some messages shown.
		/// </summary>
		Medium = 2,
		/// <summary>
		/// Only essential message are shown.
		/// </summary>
		High = 4
	}

	/// <summary>
	/// This service allows feedback to be gathered from a user.
	/// In the implementation this will generally be done using a modal dialog box.
	/// </summary>
	[ServiceContract(Namespace = "http://www.techno-fly.net")]
	public interface IMessageService
	{
		/// <summary>
		/// Shows a message to a user that must be dismissed before continuing.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="caption">The caption to display on the dialog.</param>
		/// <param name="importanceThreshold">The minimum filter level 
		/// that a user must have in order to receive the message.</param>
		[OperationContract(IsOneWay = true, Name = "ShowMessageWithCaption")]
		void ShowMessage(string message,string caption,MessageImportance importanceThreshold,string details);

		/// <summary>
		/// Shows a warning message to a user that must be dismissed before continuing.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="caption">The caption to display on the dialog.</param>
		[OperationContract(IsOneWay = true, Name = "ShowWarningWithCaption")]
		void ShowWarning(string message, string caption, string details);

		/// <summary>
		/// Shows an error message to a user that must be dismissed before continuing.
		/// </summary>
		/// <param name="message">The message to display.</param>
		/// <param name="caption">The caption to display on the dialog.</param>
		[OperationContract(IsOneWay = true, Name = "ShowErrorWithCaption")]
		void ShowError(string message, string caption, string details);

		/// <summary>
		/// Asks the user a question where the valid response is either 'yes' or 'no'.
		/// </summary>
		/// <param name="question">The question text.</param>
		/// <param name="caption">The caption to display on the dialog.</param>
		/// <returns><code>true</code> if the user selected 'yes'; 
		/// <code>false</code> otherwise.</returns>
		[OperationContract(Name = "AskYesNoQuestionWithCaption")]
		bool AskYesNoQuestion(string question, string caption, string details);

		/// <summary>
		/// Asks the user a question where the valid response 
		/// is either 'yes', 'no', or 'cancel.'
		/// </summary>
		/// <param name="question">The question text.</param>
		/// <param name="caption">The caption to display on the dialog.</param>
		/// <returns>The answer from the user. 
		/// <see cref="YesNoCancelQuestionResult"/></returns>
		[OperationContract(Name = "AskYesNoCancelQuestionWithCaption")]
		YesNoCancelQuestionResult AskYesNoCancelQuestion(
			string question, string caption, string details);

		/// <summary>
		/// Asks the user a question where the valid response 
		/// is either 'ok' or 'cancel.'
		/// </summary>
		/// <param name="question">The question text.</param>
		/// <param name="caption">The caption to display on the dialog.</param>
		/// <returns><code>true</code> if the user selected 'ok'; 
		/// <code>false</code> otherwise.</returns>
		[OperationContract(Name = "AskOkCancelQuestionWithCaption")]
		bool AskOkCancelQuestion(string question, string caption, string details);

        //[OperationContract(Name = "AskQuestionWithTextResponse")]
        //TextResponse AskQuestionWithTextResponse(TextResponseQuestion textResponseQuestion);
	}
}
