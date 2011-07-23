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
using System.Threading;
using System.Windows.Threading;

namespace Techno_Fly.Tools.Dashboard.Concurrency
{
	/// <summary>
	/// Used to allow calls to be marshaled to a particular thread.
	/// <seealso cref="SynchronizationContext"/>
	/// </summary>
	public interface ISynchronizationContext
	{
		/// <summary>
		/// Invokes the specified callback asynchronously.
		/// Method returns immediately upon queuing the request.
		/// </summary>
		/// <param name="callback">The delegate to invoke.</param>
		/// <param name="state">The state to pass to the invocation.</param>
		void InvokeWithoutBlocking(SendOrPostCallback callback, object state);

		/// <summary>
		/// Invokes the specified callback asynchronously.
		/// Method returns immediately upon queuing the request.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		void InvokeWithoutBlocking(Action action);

		/// <summary>
		/// Invokes the specified callback synchronously.
		/// Method blocks until the specified callback completes.
		/// </summary>
		/// <param name="callback">The delegate to invoke.</param>
		/// <param name="state">The state to pass to the invocation.</param>
		void InvokeAndBlockUntilCompletion(SendOrPostCallback callback, object state);

		/// <summary>
		/// Invokes the specified callback synchronously.
		/// Method blocks until the specified callback completes.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		void InvokeAndBlockUntilCompletion(Action action);

		/// <summary>
		/// Gets a value indicating whether invocation is required.
		/// That is, it determines whether the call was made from a thread other 
		/// than the one that the current instance was created on.
		/// </summary>
		/// <value><c>true</c> if the calling thread was not the thread that the current instance 
		/// was initialized on; otherwise, <c>false</c>.</value>
		bool InvokeRequired { get; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Initializes this instance using the specified dispatcher.
		/// </summary>
		/// <param name="dispatcher">The dispatcher used to provide synchronization.</param>
		void Initialize(Dispatcher dispatcher);
	}
}
