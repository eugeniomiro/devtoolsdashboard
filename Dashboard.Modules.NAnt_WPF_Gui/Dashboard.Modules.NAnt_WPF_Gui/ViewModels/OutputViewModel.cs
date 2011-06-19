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
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    [CLSCompliant(false)]
	public class OutputViewModel : DockingWindowViewModelBase<OutputWindow> {

        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public OutputViewModel(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(container, regionManager,eventAggregator)
        {
            _container = container;
            _regionManager = regionManager;

			// Create a view
			this.View = new OutputWindow { DataContext = this };

            // Create Menu Bindings
            // Initialize commands
            this.NewCommand = new DelegateCommand<object>(NewCommand_Executed);
            this.OpenCommand = new DelegateCommand<object>(OpenCommand_Executed, Command_CanExecute);
            Techno_Fly.Tools.Dashboard.Commands.New.RegisterCommand(this.NewCommand);
            Techno_Fly.Tools.Dashboard.Commands.Open.RegisterCommand(this.OpenCommand);

            this.NewCommand.IsActive = true;
            this.OpenCommand.IsActive = true;
		}

        public DelegateCommand<object> NewCommand { get; private set; }
        public DelegateCommand<object> OpenCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void NewCommand_Executed(object parameter)
        {
            //this.View.OnClearExecute();
        }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void OpenCommand_Executed(object parameter)
        {

        }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> needs to determine whether it can execute.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>
        /// <c>true</c> if the command can execute; otherwise, <c>false</c>/.
        /// </returns>
        private bool Command_CanExecute(object parameter)
        {
            return true;
            //return this.View.OnClearCommandCanExecute();
        }

	    public override void WriteTraceMessage(string message)
	    {
	        Debug.Print(message);
	        View.LogMessage(message);
	    }

	    protected override string ViewName { 
			get {
				return "Output";
			}
		}
	}
}
