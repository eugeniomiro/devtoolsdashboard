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
using System.Windows.Controls;
using System.Windows.Threading;
using AvalonDock;
using NAntGui.Framework;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views
{

    /// <summary>
    /// Represents a window for the properties window.
    /// </summary>
    [CLSCompliant(false)]
    public partial class OutputWindow : DockableContent, ILogsMessage, IEditCommands
    {

        //public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertiesView), new FrameworkPropertyMetadata(null));

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // OBJECT
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes an instance of the <c>PropertiesView</c> class.
        /// </summary>
        public OutputWindow()
        {
            InitializeComponent();
        }

        #region Implementation of ILogsMessage

        public void LogMessage(string message)
        {
            eventsListBox.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = message;
                eventsListBox.Items.Add(item);
                eventsListBox.SelectedItem = item;
                eventsListBox.ScrollIntoView(item);
            }));
        }

        #endregion

        #region Implementation of IEditCommands

        public void Cut()
        {
            throw new NotImplementedException();
        }

        public void Copy()
        {
            throw new NotImplementedException();
        }

        public void Paste()
        {
            throw new NotImplementedException();
        }

        public void SelectAll()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
