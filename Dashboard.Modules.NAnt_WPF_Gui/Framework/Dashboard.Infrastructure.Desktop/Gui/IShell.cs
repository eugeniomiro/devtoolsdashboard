using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Techno_Fly.Tools.Dashboard.Gui
{
    public interface IShell : IViewTracking
    {
        //bool CloseView(IView view, bool force);
        bool BannerVisible { get; set; }
        bool LogoVisible { get; set; }

        string Title { get; set; }
        string TitleFormat { get; set; }
    }
}
