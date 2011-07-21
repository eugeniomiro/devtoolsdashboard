using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Techno_Fly.Tools.Dashboard.Gui
{
    public interface IShellWindow : IView
    {
        IView ActiveView { get; }
        IView MainView { get; }
    }
}
