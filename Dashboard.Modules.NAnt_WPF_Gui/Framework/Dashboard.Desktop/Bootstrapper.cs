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

using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Windows.Controls.Ribbon;
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
            //Modules laden in subfolder modules en toevoegen, die worden na succesvolle build naar daar gecopiëerd
            string modulePath = @".\Modules";
            ModuleCatalog directoryModuleCatalog = new DirectoryModuleCatalog() { ModulePath = modulePath };
            return directoryModuleCatalog;
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
             * we declared elsewhere in this project. The UnityBootstrapper base 
             * class will attach an instance of the RegionManager to the new Shell window. */

            return new ShellWindow();
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