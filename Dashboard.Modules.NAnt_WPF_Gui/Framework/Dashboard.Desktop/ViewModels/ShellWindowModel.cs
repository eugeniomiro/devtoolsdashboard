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
using System.ComponentModel;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Techno_Fly.Tools.Dashboard.Gui;

namespace Techno_Fly.Tools.Dashboard.Shell.ViewModels
{
    /// <summary>
    /// The <see cref="ViewModelBase"/> implementation for the <see cref="IShell"/>.
    /// </summary>
    public class ShellWindowModel : ViewModelBase, IShell
    {
        IShellWindow shellView;

        public ShellWindowModel(IShellWindow shellView)
        {
            ArgumentValidator.AssertNotNull(shellView, "shellView");
            this.shellView = shellView;
            Title = "Dev Tools Dashboard";

            //var eventAggregator = Dependency.Resolve<IEventAggregator>();
            //var viewChangedEvent = eventAggregator.GetEvent<ActiveViewChangedInShellEvent>();
            //viewChangedEvent.Subscribe(delegate(IView obj) { ActiveView = obj; });
        }

        string _titleFormat = "{0} - Dev Tools Dashboard";

        [Description("A name displayed as a title.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        string IShell.Title
        {
            get
            {
                return Title != null ? Title.ToString() : null;
            }
            set
            {
                Title = value;
            }
        }

        [Description("A string format used when constructing the title.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleFormat
        {
            get
            {
                return _titleFormat;
            }
            set
            {
                Notifier.Assign("TitleFormat", ref _titleFormat, value);
            }
        }

        bool _bannerVisible = true;

        [Description("If true the banner will be shown, otherwise it will be hidden.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool BannerVisible
        {
            get
            {
                return _bannerVisible;
            }
            set
            {
                Notifier.Assign("BannerVisible", ref _bannerVisible, value);
            }
        }

        bool _logoVisible = true;

        [Description("If true the logo will be shown, otherwise it will be hidden.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool LogoVisible
        {
            get
            {
                return _logoVisible;
            }
            set
            {
                Notifier.Assign("LogoVisible", ref _logoVisible, value);
            }
        }

        #region Implementation of IViewTracking

        public IView ActiveView
        {
            get
            {
                return shellView.ActiveView;
            }
        }

        public IView MainView
        {
            get
            {
                return shellView.MainView;
            }
        }

        #endregion
    }
}



