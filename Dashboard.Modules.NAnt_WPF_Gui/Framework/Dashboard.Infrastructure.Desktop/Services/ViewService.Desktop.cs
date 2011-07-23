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

using System.Windows;
using Techno_Fly.Tools.Dashboard.Gui;

namespace Techno_Fly.Tools.Dashboard.Services
{
	public partial class ViewService
	{
		#region Implementation of IViewTracking

		/// <summary>
		/// Gets the active view. We find the active window
		/// and then ask it to supply us with the active view.
		/// </summary>
		/// <value>The active view within the entire application.</value>
		public IView ActiveView
		{
			get
			{
				var tracker = GetViewTracker();

				if (tracker != null)
				{
					return tracker.ActiveView;
				}
				return null;
			}
		}

		public IView MainView
		{
			get
			{
				var tracker = GetViewTracker();

				if (tracker != null)
				{
					return tracker.MainView;
				}
				return null;
			}
		}

		IViewTracking GetViewTracker()
		{
			Window activeWindow = null;
			foreach (Window window in Application.Current.Windows)
			{
				if (window.IsActive)
				{
					activeWindow = window;
					break;
				}
			}
			if (activeWindow == null)
			{
				return null;
			}

			var tracker = activeWindow as IViewTracking;

			/* If the window doesn't implement IViewTracking then we try the ViewModel. */
			if (tracker == null)
			{
				var view = activeWindow as IView;
				if (view != null)
				{
					tracker = view.ViewModel as IViewTracking;
				}
			}
			return tracker;
		}

		#endregion
	}
}
