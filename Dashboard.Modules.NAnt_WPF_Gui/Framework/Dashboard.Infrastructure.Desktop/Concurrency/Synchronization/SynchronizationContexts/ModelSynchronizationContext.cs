#region File and License Information
/*
<File>
	<Copyright>Copyright © 2009, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
	This file is part of DanielVaughan's base library

	DanielVaughan's base library is free software: you can redistribute it and/or modify
	it under the terms of the GNU Lesser General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	DanielVaughan's base library is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Lesser General Public License for more details.

	You should have received a copy of the GNU Lesser General Public License
	along with DanielVaughan's base library.  If not, see http://www.gnu.org/licenses/.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2010-04-22 17:26:58Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

using Techno_Fly.Tools.Dashboard.Concurrency;
//using Meta = DanielVaughan.Metadata.ModelSynchronizationContextMetadata;

namespace Techno_Fly.Tools.Dashboard
{
	/// <summary>
	/// Model layer implementation of <see cref="ISynchronizationContext"/>.
	/// Performs handlers on a dedicated thread for the model layer.
	/// </summary>
	public class ModelSynchronizationContext : ISynchronizationContext
	{
		volatile bool stopRunning;
		readonly Queue<CallbackReference> callbacks = new Queue<CallbackReference>();
		readonly object callbacksLock = new object();
		readonly AutoResetEvent consumerThreadResetEvent = new AutoResetEvent(false);
#if !SILVERLIGHT
		public static ApartmentState? ApartmentState { get; set; }
#endif

		internal void Stop()
		{
			stopRunning = true;
		}

		class CallbackReference
		{
			public SendOrPostCallback Callback { get; set; }
			public object State { get; set; }
		}

		#region Singleton implementation

		ModelSynchronizationContext()
		{
			/* Consumer thread initialization. */
			threadOfCreation = new Thread(
					delegate(object o)
					{
						while (!stopRunning)
						{
							ConsumeQueue();
							consumerThreadResetEvent.WaitOne();
						}
					});
#if !SILVERLIGHT
			if (ApartmentState.HasValue)
			{
				threadOfCreation.SetApartmentState(ApartmentState.Value);
			}
#endif
			threadOfCreation.Start();
		}

		public static ModelSynchronizationContext Instance
		{
			get
			{
				return Nested.instance;
			}
		}

		/// <summary>
		/// Inner class for full lazy loading of singleton.
		/// </summary>
		class Nested
		{
			static Nested()
			{
			}

			internal static readonly ModelSynchronizationContext instance = new ModelSynchronizationContext();
		}

		#endregion

		readonly Thread threadOfCreation;

		/// <summary>
		/// Gets the model thread.
		/// </summary>
		/// <value>The thread.</value>
		internal Thread Thread
		{
			get
			{
				return threadOfCreation;
			}
		}

		/// <summary>
		/// Invokes the callback without blocking. Call will return immediately.
		/// </summary>
		/// <param name="callback">The callback to invoke.</param>
		/// <param name="state">The state to pass during the callback invocation.</param>
		public void InvokeWithoutBlocking(SendOrPostCallback callback, object state)
		{
			lock (callbacksLock)
			{
				callbacks.Enqueue(new CallbackReference { Callback = callback, State = state });
			}
			consumerThreadResetEvent.Set();
		}

		/// <summary>
		/// Invokes the specified action without blocking. Call will return immediately.
		/// </summary>
		/// <param name="action">The action to invoke.</param>
		public void InvokeWithoutBlocking(Action action)
		{
			lock (callbacksLock)
			{
				callbacks.Enqueue(new CallbackReference { Callback = o => action() });
			}
			consumerThreadResetEvent.Set();
		}

		/// <summary>
		/// Invokes the specified callback and blocks until completion.
		/// </summary>
		/// <param name="callback">The callback to invoke.</param>
		/// <param name="state">The state to pass during invocation.</param>
		/// <exception cref="InvalidOperationException">
		/// Occurs if call made from the UI thread.</exception>
		public void InvokeAndBlockUntilCompletion(SendOrPostCallback callback, object state)
		{
			RaiseExceptionIfUIThread();

			AutoResetEvent innerResetEvent = new AutoResetEvent(false);
			var callbackState = new CallbackReference { Callback = callback, State = state };
			lock (callbacksLock)
			{
				var processedHandler = new EventHandler<InvokeCompleteEventArgs>(
					delegate(object o, InvokeCompleteEventArgs args)
					{
						if (args.CallbackReference == callbackState)
						{
							innerResetEvent.Set();
						}
					});

				invokeComplete += processedHandler;
				callbacks.Enqueue(callbackState);
			}

			consumerThreadResetEvent.Set();
			innerResetEvent.WaitOne();
		}

		/// <summary>
		/// Invokes the specified callback and blocks until completion.
		/// </summary>
		/// <param name="action">The action to invoke.</param>
		/// <exception cref="InvalidOperationException">
		/// Occurs if call made from the UI thread.</exception>
		public void InvokeAndBlockUntilCompletion(Action action)
		{
			RaiseExceptionIfUIThread();

			var itemResetEvent = new AutoResetEvent(false);
			var callbackReference = new CallbackReference { Callback = o => action() };

			lock (callbacksLock)
			{
				var processedHandler = new EventHandler<InvokeCompleteEventArgs>(
					delegate(object o, InvokeCompleteEventArgs args)
					{
						if (args.CallbackReference == callbackReference)
						{
							itemResetEvent.Set();
						}
					});

				invokeComplete += processedHandler;
				callbacks.Enqueue(callbackReference);
			}

			consumerThreadResetEvent.Set();
			itemResetEvent.WaitOne();
		}

		void RaiseExceptionIfUIThread()
		{
			if (!UISynchronizationContext.Instance.InvokeRequired)
			{
				throw new InvalidOperationException(
					"Blocking the UI thread may cause the application to become unresponsive.");
			}
		}

		void ConsumeQueue()
		{
			while (callbacks.Count > 0)
			{
				var callback = callbacks.Dequeue();
				callback.Callback(callback.State);
				OnInvokeComplete(callback);
			}
		}

		event EventHandler<InvokeCompleteEventArgs> invokeComplete;

		/// <summary>
		/// Used to signal that an invocation has occurred.
		/// </summary>
		class InvokeCompleteEventArgs : EventArgs
		{
			public CallbackReference CallbackReference { get; private set; }

			public InvokeCompleteEventArgs(CallbackReference callbackReference)
			{
				CallbackReference = callbackReference;
			}
		}

		void OnInvokeComplete(CallbackReference callbackReference)
		{
			if (invokeComplete != null)
			{
				invokeComplete(this, new InvokeCompleteEventArgs(callbackReference));
			}
		}

		public void Initialize()
		{
			/* Intentionally left blank. Constructor performs initialization. */
		}

		void ISynchronizationContext.Initialize(Dispatcher dispatcher)
		{
			/* Intentionally left blank. Constructor performs initialization. */
		}

		/// <summary>
		/// Gets a value indicating whether the current thread 
		/// is the thread associated with the model thread.
		/// </summary>
		/// <value><c>true</c> if the current thread is the model thread; 
		/// otherwise, <c>false</c>.</value>
		public bool InvokeRequired
		{
			get
			{
				var result = threadOfCreation.ManagedThreadId != Thread.CurrentThread.ManagedThreadId;
				return result;
			}
		}
	}
}
