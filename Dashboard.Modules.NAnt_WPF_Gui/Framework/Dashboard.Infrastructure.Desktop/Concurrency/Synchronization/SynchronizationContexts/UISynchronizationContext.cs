#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2008-11-03 23:32:33Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
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
#if SILVERLIGHT
					dispatcher = System.Windows.Deployment.Current.Dispatcher;
#else
					dispatcher = Dispatcher.CurrentDispatcher;
#endif
					context = new DispatcherSynchronizationContext(dispatcher);
				}
				catch (InvalidOperationException)
				{
					/* TODO: Make localizable resource. */
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
