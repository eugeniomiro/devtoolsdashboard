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
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using NAntGui.Framework;
using System.Diagnostics;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views;
using System.Collections.Generic;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    [CLSCompliant(false)]
	public class PropertiesViewModel : DockingWindowViewModelBase<PropertiesView> {

        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        public PropertiesViewModel(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(container, regionManager,eventAggregator)
        {
            _container = container;
            _regionManager = regionManager;

			//Create a view
            this.View = new PropertiesView { DataContext = this };

		}



	    public override void WriteTraceMessage(string message)
	    {
	        Debug.Print(message);
	    }

	    protected override string ViewName { 
			get {
				return "Properties";
			}
		}

        internal void AddProperties(List<IBuildProperty> properties)
        {
            //View._propertyGrid.SelectedObject = new PropertyShelf(properties);
        }

        internal void Clear()
        {
            //View._propertyGrid.SelectedObject = null;
        }

	}
}
