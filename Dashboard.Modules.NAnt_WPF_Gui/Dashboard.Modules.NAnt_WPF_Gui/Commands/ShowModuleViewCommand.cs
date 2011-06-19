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
using Microsoft.Practices.Unity;
using Techno_Fly.Tools.Dashboard.Events;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Commands
{
    [CLSCompliant(false)]
    public class ShowModuleViewCommand : ICommand
    {
        #region Fields

        // Member variables
        private ModuleTaskButtonViewModel _moduleTaskButtonViewModel;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShowModuleViewCommand(ModuleTaskButtonViewModel viewModel)
        {
            _moduleTaskButtonViewModel = viewModel;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Whether the ShowModuleViewCommand is enabled.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Actions to take when CanExecute() changes.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Executes the ShowModuleViewCommand
        /// </summary>
        public void Execute(object parameter)
        {
            // Initialize
            var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            var container = ServiceLocator.Current.GetInstance<IUnityContainer>();

            //regionManager.AddToRegion("RibbonRegion", new ModuleRibbonTab().Ribbon);
            
            //regionManager.Regions["RibbonRegion"].Add(container.Resolve<ModuleRibbonTab>().Ribbon);
            //regionManager.Regions["LeftRegion"].Add(container.Resolve<PropertiesViewModel>().View);
            //regionManager.Regions["LeftRegion"].Add(container.Resolve<TargetsViewModel>().View);
            //regionManager.Regions["BottomRegion"].Add(container.Resolve<OutputViewModel>().View);

            var moduleRibbonTab = new Uri("ModuleRibbonTab", UriKind.Relative);
            regionManager.RequestNavigate("RibbonRegion", moduleRibbonTab);

            //var propertiesView = new Uri("PropertiesView", UriKind.Relative);
            //regionManager.RequestNavigate("LeftRegion", propertiesView);

            //var targetsView = new Uri("TargetsView", UriKind.Relative);
            //regionManager.RequestNavigate("LeftRegion", propertiesView);

            PropertiesView propertiesView = container.Resolve<PropertiesViewModel>().View;
            if (!regionManager.Regions["LeftRegion"].Views.Contains(propertiesView)) regionManager.Regions["LeftRegion"].Add(container.Resolve<PropertiesViewModel>().View);

            TargetsView targetsView = container.Resolve<TargetsViewModel>().View;
            if (!regionManager.Regions["LeftRegion"].Views.Contains(targetsView)) regionManager.Regions["LeftRegion"].Add(container.Resolve<TargetsViewModel>().View);

            OutputWindow outputWindow = container.Resolve<OutputViewModel>().View;
            if (!regionManager.Regions["BottomRegion"].Views.Contains(outputWindow)) regionManager.Regions["BottomRegion"].Add(container.Resolve<OutputViewModel>().View);

            container.Resolve<MainController>();

            /* We invoke the NavigationCompleted() callback 
             * method in our final  navigation request. */

            // Show Workspace
            //var moduleWorkspace = new Uri("ModuleWorkspace", UriKind.Relative);
            //regionManager.RequestNavigate("WorkspaceRegion", moduleWorkspace, NavigationCompleted);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Callback method invoked when navigation has completed.
        /// </summary>
        /// <param name="result">Provides information about the result of the navigation.</param>
        private void NavigationCompleted(NavigationResult result)
        {
            // Exit if navigation was not successful
            if (result.Result != true) return;

            // Publish ViewRequestedEvent
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            var navigationCompletedEvent = eventAggregator.GetEvent<NavigationCompletedEvent>();
            navigationCompletedEvent.Publish("NAnt_WPF_Gui");
        }

        #endregion
    }
}
