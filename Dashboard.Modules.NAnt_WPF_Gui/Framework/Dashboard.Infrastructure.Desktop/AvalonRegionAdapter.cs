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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using AvalonDock;
using Microsoft.Practices.Prism.Regions;

namespace Techno_Fly.Tools.Dashboard
{
    /// <summary>
    /// Region adapter for Pane's (specifically DocumentPane and DockablePane)
    /// </summary>
    public sealed class AvalonRegionAdapter : RegionAdapterBase<DocumentPane>
    {
        private IRegionManager _regionManager;

        public AvalonRegionAdapter(IRegionBehaviorFactory factory, IRegionManager regionManager)
            : base(factory)
        {
            _regionManager = regionManager;
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }

        protected override void Adapt(IRegion region, DocumentPane regionTarget)
        {
            region.Views.CollectionChanged += delegate(Object sender, NotifyCollectionChangedEventArgs e)
            {
                OnViewsCollectionChanged(sender, e, region, regionTarget);
            };
        }

        private void OnViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e, IRegion region, Pane regionTarget)
        {


            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (object item in e.NewItems)
                {
                    UIElement view = item as UIElement;

                    if (view is ITabViewInfo)
                    {
                        if (view != null)
                        {
                            DockableContent newContentPane = ((DockableContent)view);

                            newContentPane.Closing += (contentPaneSender, args) =>
                            {
                                Debug.WriteLine("Removing view from region", "Prism");
                                // Make sure a reference to the view is removed
                                ((DockableContent)contentPaneSender).Content = null;
                                region.Remove(item);
                            };

                            regionTarget.Items.Add(newContentPane);
                            newContentPane.Activate();
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object item in e.OldItems)
                {
                    if (regionTarget is DocumentPane)
                    {
                        for (int i = 0; i < regionTarget.Items.Count; i++)
                        {
                            if (((DockableContent)regionTarget.Items[i]).Content == item)
                            {
                                Debug.WriteLine("Region view removed; removing associated DockableContent", "Prism");
                                regionTarget.Items.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

            }
        }
    }
}
