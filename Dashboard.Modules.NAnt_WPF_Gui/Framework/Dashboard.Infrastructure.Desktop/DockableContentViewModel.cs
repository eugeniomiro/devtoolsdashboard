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

using AvalonDock;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Techno_Fly.Tools.Dashboard 
{

	public abstract class DockableContentViewModel<T> : ViewModelBase, IDockableContentViewModel where T: DockableContent {

        //protected DockableContentViewModel() : base()
        //{

        //}
		
        //public void Activate(bool focus) {
        //    if (this.View == null)
        //        return;

        //    IRegion dockSiteRegion = this.RegionManager.Regions[RegionNames.DockSiteRegion];
        //    this.Open();
        //    if (focus)
        //        dockSiteRegion.Activate(this.View);
        //    else
        //        this.View.Activate();
        //}

        //public void Close() {
        //    if (this.View == null)
        //        return;

        //    IRegion dockSiteRegion = this.RegionManager.Regions[RegionNames.DockSiteRegion];
        //    if (dockSiteRegion.Views.Contains(this.View))
        //        dockSiteRegion.Remove(this.View);
        //}
		
        //public void Dock()
        //{
            //ToolWindow toolWindow = this.View as ToolWindow;
            //if (toolWindow != null) {
            //    IRegion dockSiteRegion = this.RegionManager.Regions[RegionNames.DockSiteRegion];
            //    if (!dockSiteRegion.Views.Contains(this.View))
            //        dockSiteRegion.Add(this.View, this.ViewName);

            //    toolWindow.Dock(target, direction);
            //}
        //}

        //public void Open() {
        //    if (this.View == null)
        //        return;

        //    IRegion dockSiteRegion = this.RegionManager.Regions[RegionNames.DockSiteRegion];
        //    if (!dockSiteRegion.Views.Contains(this.View))
        //        dockSiteRegion.Add(this.View, this.ViewName);
        //}

	    public abstract void WriteTraceMessage(string message);

        //public new T View
        //{
        //    get { return base.View as T; }
        //    set { base.View = value; }
        //}

        //protected abstract string ViewName { get; }

	}

}
