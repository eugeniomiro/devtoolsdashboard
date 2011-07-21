using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Techno_Fly.Tools.Dashboard.IO;

namespace Techno_Fly.Tools.Dashboard.Content
{
    /// <summary>
    /// This is interface will be retired when we have generic support in XAML bindings. 
    /// We will use <see cref="IContentProvider{ISavableContent}"/>
    /// 
    /// Provides an instance of an <see cref="ISavableContent"/>.
    /// Is used to indirectly implement the <see cref="ISavableContent"/>
    /// using a strategy.
    /// </summary>
    public interface ISavableContentProvider
    {
        /// <summary>
        /// Gets the savable content.
        /// </summary>
        /// <value>The savable content.</value>
        ISavableContent Content { get; }
    }
}
