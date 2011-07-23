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

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

using Techno_Fly.Tools.Dashboard.Concurrency;

namespace Techno_Fly.Tools.Dashboard
{
	/// <summary>
	/// Singleton class providing the default implementation 
	/// for the <see cref="ISynchronizationContext"/>, specifically for the UI thread.
	/// </summary>
	public partial class UISynchronizationContext : ISynchronizationContext
	{
		DispatcherSynchronizationContext context;
		Dispatcher dispatcher;
		
		#region Singleton implementation

		static readonly UISynchronizationContext instance = new UISynchronizationContext();
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		/// <value>The singleton instance.</value>
		public static ISynchronizationContext Instance
		{
			get
			{
				return instance;
			}
		}

		UISynchronizationContext()
		{
			/* Intentionally left blank. */
		}

		#endregion

		public void Initialize()
		{
			EnsureInitialized();
		}

		readonly object initializationLock = new object();

		void EnsureInitialized()
		{
			if (dispatcher != null && context != null)
			{
				return;
			}

			lock (initializationLock)
			{
				if (dispatcher != null && context != null)
				{
					return;
				}

				try
				{
					dispatcher = Dispatcher.CurrentDispatcher;
					context = new DispatcherSynchronizationContext(dispatcher);
				}
				catch (InvalidOperationException)
				{
					throw new ConcurrencyException("Initialised called from non-UI thread."); 
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Initialize(Dispatcher dispatcher)
		{
			ArgumentValidator.AssertNotNull(dispatcher, "dispatcher");
			lock (initializationLock)
			{
				this.dispatcher = dispatcher;
				context = new DispatcherSynchronizationContext(dispatcher);
			}
		}

		public void InvokeWithoutBlocking(SendOrPostCallback callback, object state)
		{
			ArgumentValidator.AssertNotNull(callback, "callback");
			EnsureInitialized();

			context.Post(callback, state);
			//dispatcher.BeginInvoke(callback);
		}

		public void InvokeWithoutBlocking(Action action)
		{
			ArgumentValidator.AssertNotNull(action, "action");
			EnsureInitialized();

//			context.Post(state => action(), null);
			dispatcher.BeginInvoke(action);
//			if (dispatcher.CheckAccess())
//			{
//				action();
//			}
//			else
//			{
//				dispatcher.BeginInvoke(action);
//			}
		}

		public void InvokeAndBlockUntilCompletion(SendOrPostCallback callback, object state)
		{
			ArgumentValidator.AssertNotNull(callback, "callback");
			EnsureInitialized();

			context.Send(callback, state);
		}

		public void InvokeAndBlockUntilCompletion(Action action)
		{
			ArgumentValidator.AssertNotNull(action, "action");
			EnsureInitialized();

			if (dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				context.Send(delegate { action(); }, null);
			}
		}

		public bool InvokeRequired
		{
			get
			{
				EnsureInitialized();
				return !dispatcher.CheckAccess();
			}
		}
	}
}
