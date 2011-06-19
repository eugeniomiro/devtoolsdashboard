using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using NAntGui.Core;
using NAntGui.Framework;
using System.Windows;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{

    internal class FileNameEventArgs : EventArgs
    {
        private readonly string _fileName;
        private readonly Point _point;

        internal FileNameEventArgs(string fileName, Point p)
        {
            _fileName = fileName;
            _point = p;
        }

        internal string FileName
        {
            get { return _fileName; }
        }

        internal Point Point
        {
            get { return _point; }
        }
    }

    internal class RecentItemsEventArgs : EventArgs
    {
        private readonly string _item;

        internal RecentItemsEventArgs(string item)
        {
            _item = item;
        }

        internal string Item
        {
            get { return _item; }
        }
    }

    internal class RunEventArgs : EventArgs
    {
        internal RunEventArgs(IBuildTarget target)
        {
            Target = target;
        }

        internal IBuildTarget Target { get; private set; }
    }

    /// <summary>
    /// Description of NewProjectEventArgs.
    /// </summary>
    internal class NewProjectEventArgs : EventArgs
    {
        internal NewProjectEventArgs(ProjectInfo info)
        {
            Assert.NotNull(info, "info");
            Info = info;
        }

        internal ProjectInfo Info { get; private set; }
    }
   
}
