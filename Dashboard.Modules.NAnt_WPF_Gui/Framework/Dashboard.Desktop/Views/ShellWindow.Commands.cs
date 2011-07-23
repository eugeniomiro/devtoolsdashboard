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

using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Techno_Fly.Tools.Dashboard.Events;
using Techno_Fly.Tools.Dashboard.IO;

namespace Techno_Fly.Tools.Dashboard.Shell.Views
{
    public partial class ShellWindow
    {
        public static RoutedUICommand SaveAll = new RoutedUICommand("Save All", "SaveAll", typeof(ShellWindow));

        void AttachCommandBindings()
        {
            var commandList = new List<CommandBinding>
		                  	{
		                  		new CommandBinding(ApplicationCommands.Close,	OnCloseCommandExecute,	CloseCommandCanExecute),
		                  		new CommandBinding(ApplicationCommands.Save,	OnSave,					OnCanSave),
		                  		new CommandBinding(ApplicationCommands.SaveAs,	OnSaveAs,				OnCanSaveAs),
								new CommandBinding(SaveAll,						OnSaveAll,				OnCanSaveAll)
		                  	};
            CommandBindings.AddRange(commandList);
        }

        void OnCanSaveAll(object sender, CanExecuteRoutedEventArgs e)
        {
            var allSavableContent = GetAllSavableContent();
            foreach (var content in allSavableContent)
            {
                if (content.CanSave && content.Dirty)
                {
                    e.CanExecute = true;
                    e.Handled = true;
                }
            }
        }

        void OnSaveAll(object sender, ExecutedRoutedEventArgs e)
        {
            var allSavableContent = GetAllSavableContent();
            foreach (var content in allSavableContent)
            {
                if (content.CanSave && content.Dirty)
                {
                    var operationResult = content.Save(FileErrorAction.UseAlternative);
                    if (operationResult == FileOperationResult.Cancelled)
                    {
                        break;
                    }
                }
            }
            e.Handled = true;
        }

        void OnCanSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            var savableContent = GetSavableContent();

            if (savableContent != null)
            {
                e.CanExecute = savableContent.CanSave;
            }
        }

        void OnSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            var savableContent = GetSavableContent();

            if (savableContent != null)
            {
                savableContent.SaveAs(FileErrorAction.UseAlternative);
                e.Handled = true;
            }
        }

        void OnCanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            var savableContent = GetSavableContent();

            if (savableContent != null)
            {
                e.CanExecute = savableContent.CanSave;
                e.Handled = true;
            }
        }

        void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            var savableContent = GetSavableContent();

            if (savableContent != null)
            {
                if (savableContent.NewFile)
                {
                    savableContent.SaveAs(FileErrorAction.UseAlternative);
                }
                else
                {
                    savableContent.Save(FileErrorAction.UseAlternative);
                }
                e.Handled = true;
            }
        }

        #region Close
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            bool wasCancelled;
            ExitApplication(out wasCancelled);
            if (wasCancelled)
            {
                e.Cancel = true;
                return;
            }
            base.OnClosing(e);
        }

        void ExitApplication(out bool cancel)
        {
            /* We allow subscribers to the ApplicationExitEvent to cancel the close. */
            var eventAggregator = Dependency.Resolve<IEventAggregator>("");
            var exitEvent = eventAggregator.GetEvent<ApplicationExitEvent>();
            var eventArgs = new CancelableEventArgs<object>(null);
            exitEvent.Publish(eventArgs);
            if (eventArgs.Cancel) /* If a subscriber has cancelled the exit then we abort. */
            {
                cancel = true;
                return;
            }
            cancel = false;
            return;
        }

        void OnCloseCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            var view = e.Parameter as IView ?? MainView;
            if (view != null)
            {
                e.Handled = true;
                view.Close(false);
            }
        }

        void CloseCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion
    }
}

