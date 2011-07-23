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
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Diagnostics;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views;
using NAntGui.Framework;
using System.Collections.Generic;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    [CLSCompliant(false)]
    public class TargetsViewModel : DockableContentViewModel<TargetsView>
    {

        private string _projectName = "";

        public TargetsViewModel()
        {
			// Create a view
			this.View = new TargetsView { DataContext = this };

            // Create Menu Bindings
            // Initialize commands
            this.NewCommand = new DelegateCommand<object>(NewCommand_Executed);
            this.OpenCommand = new DelegateCommand<object>(OpenCommand_Executed, Command_CanExecute);
            Techno_Fly.Tools.Dashboard.Commands.New.RegisterCommand(this.NewCommand);
            Techno_Fly.Tools.Dashboard.Commands.Open.RegisterCommand(this.OpenCommand);

            this.NewCommand.IsActive = true;
            this.OpenCommand.IsActive = true;
		}

        public DelegateCommand<object> NewCommand { get; private set; }
        public DelegateCommand<object> OpenCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void NewCommand_Executed(object parameter)
        {
            //this.View.OnClearExecute();
        }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void OpenCommand_Executed(object parameter)
        {

        }


        /// <summary>
        /// Occurs when the <see cref="ICommand"/> needs to determine whether it can execute.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>
        /// <c>true</c> if the command can execute; otherwise, <c>false</c>/.
        /// </returns>
        private bool Command_CanExecute(object parameter)
        {
            return true;
            //return this.View.OnClearCommandCanExecute();
        }



	    public override void WriteTraceMessage(string message)
	    {
	        Debug.Print(message);
	    }

        internal List<IBuildTarget> SelectedTargets
        {
            get
            {
                List<IBuildTarget> targets = new List<IBuildTarget>();
                //foreach (TreeNode node in _treeView.Nodes[0].Nodes)
                //{
                //    if (node.Checked)
                //    {
                //        IBuildTarget target = node.Tag as IBuildTarget;
                //        targets.Add(target);
                //    }
                //}

                return targets;
            }
            set
            {
                //foreach (TreeNode node in _treeView.Nodes[0].Nodes)
                //    node.Checked = false;

                //foreach (IBuildTarget target in value)
                //    SelectTarget(target);
            }
        }

        internal event EventHandler<RunEventArgs> RunTarget;

        internal void Clear()
        {
            //_treeView.Nodes.Clear();
        }

        internal void SetTargets(List<IBuildTarget> targets)
        {
            Targets = targets;
            //_treeView.Nodes.Clear();

            //_treeView.Nodes.Add(new TreeNode(_projectName));

            //foreach (IBuildTarget target in targets)
            //{
            //    AddTargetTreeNode(target);
            //}

            //_treeView.ExpandAll();
        }

        internal string ProjectName
        {
            set { _projectName = value; }
        }

        #region Event Handlers

        private void TreeViewAfterCheck(object sender, EventArgs e)
        {
            //foreach (TreeNode node in e.Node.Nodes)
            //{
            //    node.Checked = e.Node.Checked;
            //}
        }

        private void TreeViewMouseMove(object sender, MouseEventArgs e)
        {
            //TreeNode node = _treeView.GetNodeAt(e.X, e.Y);
            //if (node == null || node.Parent == null)
            //{
            //    _toolTip.SetToolTip(_treeView, "");
            //}
            //else
            //{
            //    IBuildTarget target = node.Tag as IBuildTarget;
            //    if (target != null && _toolTip.GetToolTip(_treeView) != target.Description)
            //        _toolTip.SetToolTip(_treeView, target.Description);
            //}
        }

        private void RunMenuItemClick(object sender, EventArgs e)
        {
            //if (_contextNode != null)
            //    OnRunTarget(_contextNode.Tag as IBuildTarget);
        }

        private void _treeView_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            //{
            //    TreeNode node = _treeView.GetNodeAt(e.X, e.Y);
            //    if (node != null && node.Parent != null)
            //    {
            //        _contextNode = node;
            //    }
            //}
        }

        #endregion

        #region Private Methods

        private void AddTargetTreeNode(IBuildTarget target)
        {
            //if (!Settings.Default.HideTargetsWithoutDescription || HasDescription(target.Description))
            //{
            //    string targetName = FormatTargetName(target.Name, target.Description);
            //    TreeNode node = new TreeNode(targetName);
            //    node.Checked = target.Default;
            //    node.Tag = target;
            //    _treeView.Nodes[0].Nodes.Add(node);
            //}
        }

        private static string FormatTargetName(string name, string description)
        {
            const string format = "{0}";
            return HasDescription(description) ? string.Format(format, name, description) : name;
        }

        private static bool HasDescription(string description)
        {
            return description.Length > 0;
        }

        private void OnRunTarget(IBuildTarget target)
        {
            // need to figure out which target to run
            if (RunTarget != null)
                RunTarget(this, new RunEventArgs(target));
        }

        private void SelectTarget(IBuildTarget target)
        {
            //foreach (TreeNode node in _treeView.Nodes[0].Nodes)
            //    if (node.Text == target.Name)
            //        node.Checked = true;
        }

        #endregion

        private List<IBuildTarget> _targets;
        public List<IBuildTarget> Targets
        {
            get
            {
                return _targets;
            }
            set
            {
                _targets = value;
                Notifier.NotifyChanged("Targets");
                //RaisePropertyChangedEvent("Targets");
            }
        }
	}


}
