using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAntGui.Framework;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    /// <summary>
    /// Summary description for BlankBuildScript.
    /// </summary>
    public class BlankBuildScript : IBuildScript
    {
        public string Description
        {
            get { return ""; }
        }

        public void Parse()
        {
            /* do nothing */
        }

        public string Name
        {
            get { return "Untitled"; }
        }

        [CLSCompliant(false)]
        public List<IBuildProperty> Properties
        {
            get { return new List<IBuildProperty>(); }
        }

        [CLSCompliant(false)]
        public List<IBuildTarget> Targets
        {
            get { return new List<IBuildTarget>(); }
        }
    }
}
