using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Techno_Fly.Tools.Dashboard
{
    /// <summary>
    /// Represents the visual interface that a user interacts with.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets the view model for the view. 
        /// The ViewModel is usually the DataContext of the view.
        /// </summary>
        /// <value>The view model.</value>
        IViewModel ViewModel { get; }

        /// <summary>
        /// Occurs when the view has been loaded.
        /// </summary>
        event EventHandler<EventArgs> ViewLoaded;

        /* TODO: [DV] Comment. */
        bool Close(bool force);
    }
}
