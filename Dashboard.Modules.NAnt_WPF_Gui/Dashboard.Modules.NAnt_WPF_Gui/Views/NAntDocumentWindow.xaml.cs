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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AvalonDock;
using NAntGui;
using NAntGui.Framework;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views
{
    /// <summary>
    /// Interaction logic for NantDocumentWindow2.xaml
    /// </summary>
    public partial class NAntDocumentWindow : DockableContentView, ITabViewInfo
    {
        private CommandLineOptions _options = new CommandLineOptions();

        public NAntDocumentWindow(ILogsMessage logger, CommandLineOptions options)
        {
            InitializeComponent();
            DataContext = ViewModel = new NAntDocumentWindowModel(logger, options);
        }

        public NAntDocumentWindow(string fileName, ILogsMessage logger, CommandLineOptions options)
        {
            InitializeComponent();
            DataContext = ViewModel = new NAntDocumentWindowModel(fileName, logger, options);
        }

        public NAntDocumentWindowModel NAntDocumentWindowModel
        {
            get
            {
                return (NAntDocumentWindowModel)DataContext;
            }
        }

    }
}
