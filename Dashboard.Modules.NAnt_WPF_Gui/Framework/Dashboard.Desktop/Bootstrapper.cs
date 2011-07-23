using System;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Windows.Controls.Ribbon;
using Techno_Fly.Tools.Dashboard.Gui;
using Techno_Fly.Tools.Dashboard.Services;
using Techno_Fly.Tools.Dashboard.Shell.Views;

namespace Techno_Fly.Tools.Dashboard.Shell
{
    public class Bootstrapper : UnityBootstrapper
    {
        #region Method Overrides

        /// <summary>
        /// Populates the Module Catalog.
        /// </summary>
        /// <returns>A new Module Catalog.</returns>
        /// <remarks>
        /// This method uses the Module Discovery method of populating the Module Catalog. It requires
        /// a post-build event in each module to place the module assembly in the module catalog
        /// directory.
        /// </remarks>
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var moduleCatalog = new DirectoryModuleCatalog();
            moduleCatalog.ModulePath = @".\Modules";
            //return moduleCatalog;

            ////Modules laden in subfolder modules en toevoegen, die worden na succesvolle build naar daar gecopiëerd
            //string modulePath = @".\Modules";
            ////if (Properties.Settings.Default["ModulePath"] != null) modulePath = Properties.Settings.Default["ModulePath"].ToString();
            //ModuleCatalog directoryModuleCatalog = new DirectoryModuleCatalog() { ModulePath = modulePath };

            //Primary module catalog in dit project laden
            //directoryModuleCatalog.AddModule(typeof(Techno_Fly.Tools.Dashboard.Modules.Primary.PrimaryModule));
            return moduleCatalog;

        }

        /// <summary>
        /// Configures the default region adapter mappings to use in the application, in order 
        /// to adapt UI controls defined in XAML to use a region and register it automatically.
        /// </summary>
        /// <returns>The RegionAdapterMappings instance containing all the mappings.</returns>
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            // Call base method
            var mappings = base.ConfigureRegionAdapterMappings();
            if (mappings == null) return null;

            // Add custom mappings
            var ribbonRegionAdapter = ServiceLocator.Current.GetInstance<RibbonRegionAdapter>();
            mappings.RegisterMapping(typeof(Ribbon), ribbonRegionAdapter);

            var ribbonGroupRegionAdapter = ServiceLocator.Current.GetInstance<RibbonGroupRegionAdapter>();
            mappings.RegisterMapping(typeof(RibbonGroup), ribbonGroupRegionAdapter);

            var avalonRegionAdapter = ServiceLocator.Current.GetInstance<AvalonRegionAdapter>();
            mappings.RegisterMapping(typeof(AvalonDock.DocumentPane), avalonRegionAdapter);

            // Set return value
            return mappings;
        }

        /// <summary>
        /// Instantiates the Shell window.
        /// </summary>
        /// <returns>A new ShellWindow window.</returns>
        protected override DependencyObject CreateShell()
        {
            /* This method sets the UnityBootstrapper.Shell property to the ShellWindow
             * we declared elsewhere in this project. Note that the UnityBootstrapper base 
             * class will attach an instance of the RegionManager to the new Shell window. */

            //return new ShellWindow();

            //var themeRegisterationTask = new DefaultThemeRegistrationTask();
            //var taskService = Container.Resolve<ITaskService>();
            //taskService.PerformTask(themeRegisterationTask, null, null);

            var commandService = Container.TryResolve<ICommandService>();
            var shell = Container.TryResolve<IShell>();
            if (shell == null)
            {
                ShellWindow shellWindow = new ShellWindow();
                /* Hack to cause ControlTemplate to be applied. */
                //shellView.BeginInit();
                //shellView.EndInit();
                //shellView.ApplyTemplate();

                /* Retrieve viewmodel from view and register it. */
                shell = (IShell)shellWindow.ViewModel;
                Container.RegisterInstance<IShell>(shell);
                Container.RegisterInstance<IMainWindow>(shellWindow);

                if (commandService == null)
                {
                    /* The shell implementation acts as the Command Service. */
                    Container.RegisterInstance<ICommandService>((ICommandService)shellWindow);
                }
                return shellWindow;
            }

            var mainWindow = Container.TryResolve<IMainWindow>();
            if (mainWindow == null)
            {
                mainWindow = shell as IMainWindow;
                if (mainWindow != null)
                {
                    if (commandService == null)
                    {
                        /* The shell implementation acts as the Command Service. */
                        Container.RegisterInstance<ICommandService>((ICommandService)shell);
                    }
                    /* Required by independent services. E.g. FileService. */
                    Container.RegisterInstance<IMainWindow>(mainWindow);
                    return (DependencyObject)mainWindow;
                }
            }
            throw new Exception("IMainWindow is not registered with the IOC container.");
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // By registering the UI thread dispatcher we are able to invoke controls from anywhere
            RegisterInstanceIfMissing<Dispatcher>(Dispatcher.CurrentDispatcher);

            // IViewService will hide and show visual elements depending on workspace content
            RegisterTypeIfMissing<IViewService, ViewService>(true);

            // File Service
            RegisterTypeIfMissing<IFileService, FileService>(true);

        }

        void RegisterInstanceIfMissing<TFrom>(TFrom instance)
        {
            if (!Container.IsTypeRegistered(typeof(TFrom)))
            {
                Container.RegisterInstance<TFrom>(instance);
            }
        }

        void RegisterTypeIfMissing<TFrom, TTo>(bool registerAsSingleton) where TTo : TFrom
        {
            if (!Container.IsTypeRegistered(typeof(TFrom)))
            {
                if (registerAsSingleton)
                {
                    Container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
                }
                else
                {
                    Container.RegisterType<TFrom, TTo>();
                }
            }
        }

        /// <summary>
        /// Displays the Shell window to the user.
        /// </summary>
        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

        #endregion
    }
}