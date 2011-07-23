using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Techno_Fly.Tools.Dashboard.Content
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContent"></typeparam>
    public interface IContentProvider<TContent> where TContent : class
    {
        TContent Content
        {
            get;
        }
    }
}
