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

using Microsoft.Practices.Prism;
using Techno_Fly.Tools.Dashboard;

namespace Techno_Fly.Tools.Dashboard
{
    /// <summary>
    /// Provides for advanced presentation behaviour in a <see cref="IViewModel"/>s.
    /// </summary>
    public interface IViewAware
    {
        /// <summary>
        /// Attaches the specified active aware instance so that changes in the <see cref="IActiveAware.IsActive"/>
        /// state can be monitored.
        /// </summary>
        /// <param name="activeAware">The active aware.</param>
        void Attach(IActiveAware activeAware);

        /// <summary>
        /// Detaches the active aware instance. Changes in the <see cref="IActiveAware.IsActive"/>
        /// state will no longer be monitored.
        /// </summary>
        void DetachActiveAware();

        //		/// <summary>
        //		/// Gets a unique key for the view which can be used in conjuction 
        //		/// with such things as the ITaskService
        //		/// to provide a context undo redo tasks.
        //		/// </summary>
        //		/// <value>The view key.</value>
        //		object ViewKey { get; }
    }
}