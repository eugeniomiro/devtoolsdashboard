#region File and License Information
/*
<File>
	<Copyright>Copyright © 2010, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY <copyright holder> ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-02-03 18:46:40Z</CreationDate>
</File>
*/
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
