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

using Microsoft.Windows.Controls.Ribbon;
using Microsoft.Practices.Prism.Commands;

namespace Techno_Fly.Tools.Dashboard.Shell.Views
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : RibbonWindow
    {
        public ShellWindow()
        {
            InitializeComponent();

            // Insert code required on object creation below this point.
            this.AboutCommand = new DelegateCommand<object>(AboutCommand_Executed);
            Techno_Fly.Tools.Dashboard.Commands.About.RegisterCommand(this.AboutCommand);
            this.AboutCommand.IsActive = true;
        }

        public DelegateCommand<object> AboutCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void AboutCommand_Executed(object parameter)
        {
            WPFAboutBox1 about = new WPFAboutBox1(this);
            about.ShowDialog();
        }
    }
}
