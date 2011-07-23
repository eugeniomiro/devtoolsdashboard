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
using System.Windows;
using AvalonDock;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views
{

    /// <summary>
    /// Represents a window for the properties window.
    /// </summary>
    [CLSCompliant(false)]
    public partial class PropertiesView : DockableContentView
    {
        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertiesView), new FrameworkPropertyMetadata(null));

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // OBJECT
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes an instance of the <c>PropertiesView</c> class.
        /// </summary>
        [CLSCompliant(false)]
        public PropertiesView()
        {
            InitializeComponent();
        }

        public object SelectedObject
        {
            get
            {
                return this.GetValue(SelectedObjectProperty);
            }
            set
            {
                this.SetValue(SelectedObjectProperty, value);
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
