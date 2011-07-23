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
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Techno_Fly.Tools.Dashboard.Events;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Commands;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    [CLSCompliant(false)]
    public class ModuleTaskButtonViewModel : ViewModelBase, INavigationAware
    {
        #region Fields

        private bool? _isChecked;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ModuleTaskButtonViewModel()
        {
            this.Initialize();
        }

        #endregion

        #region INavigationAware Members

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Command Properties

        /// <summary>
        /// Loads the view for Module ...
        /// </summary>
        public ICommand ShowModuleView { get; set; }   

        #endregion

        #region Administrative Properties

        /// <summary>
        /// Whether the button is checked (selected).
        /// </summary>
        public bool? IsChecked
        {
            get { return _isChecked; }

            set
            {
                //Notifier.NotifyChanging("IsChecked");
                _isChecked = value;
                Notifier.NotifyChanged("IsChecked");
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Sets the IsChecked state of the Task Button when navigation is completed.
        /// </summary>
        /// <param name="publisher">The publisher of the event.</param>
        private void OnNavigationCompleted(string publisher)
        {
            // Exit if this module published the event
            if (publisher == "NAnt_WPF_Gui") return;

            // Otherwise, uncheck this button
            this.IsChecked = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes the view model.
        /// </summary>
        private void Initialize()
        {
            // Initialize command properties
            this.ShowModuleView = new ShowModuleViewCommand(this);

            // Initialize administrative properties
            this.IsChecked = false;

            // Subscribe to Composite Presentation Events
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            var navigationCompletedEvent = eventAggregator.GetEvent<NavigationCompletedEvent>();
            navigationCompletedEvent.Subscribe(OnNavigationCompleted, ThreadOption.UIThread);
        }

        #endregion
    }
}