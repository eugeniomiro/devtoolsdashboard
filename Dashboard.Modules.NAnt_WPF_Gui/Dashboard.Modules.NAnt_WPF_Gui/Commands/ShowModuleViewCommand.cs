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

            // Show Ribbon Tab
            var moduleRibbonTab = new Uri("DeployServersModuleRibbonTab", UriKind.Relative);
            regionManager.RequestNavigate("RibbonRegion", moduleRibbonTab);

            var targetsView = new Uri("TargetsView", UriKind.Relative);
            regionManager.RequestNavigate("LeftRegion", targetsView, NavigationCompleted);

            var propertiesView = new Uri("PropertiesView", UriKind.Relative);
            regionManager.RequestNavigate("LeftRegion", propertiesView, NavigationCompleted);

            var outputView = new Uri("OutputView", UriKind.Relative);
            regionManager.RequestNavigate("BottomRegion", outputView, NavigationCompleted);

            container.Resolve<MainController>();

            /* We invoke the NavigationCompleted() callback 
             * method in our final  navigation request. */
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
