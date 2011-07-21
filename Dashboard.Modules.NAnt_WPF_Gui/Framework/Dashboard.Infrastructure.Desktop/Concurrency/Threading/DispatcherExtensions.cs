using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Techno_Fly.Tools.Dashboard
{
	/* TODO: [DV] Comment. */
	public static class DispatcherExtensions
	{

#if !SILVERLIGHT
		/// <summary>
		/// A simple threading extension method, to invoke a delegate
		/// on the correct thread if it is not currently on the correct thread
		/// which can be used with DispatcherObject types.
		/// </summary>
		/// <param name="dispatcher">The Dispatcher object on which to 
		/// perform the Invoke</param>
		/// <param name="action">The delegate to run</param>
		/// <param name="priority">The DispatcherPriority for the invoke.</param>
		public static void InvokeIfRequired(this Dispatcher dispatcher,
			Action action, DispatcherPriority priority)
		{
			if (!dispatcher.CheckAccess())
			{
				dispatcher.Invoke(priority, action);
			}
			else
			{
				action();
			}
		}
#endif

		/// <summary>
		/// A simple threading extension method, to invoke a delegate
		/// on the correct thread if it is not currently on the correct thread
		/// which can be used with DispatcherObject types.
		/// </summary>
		/// <param name="dispatcher">The Dispatcher object on which to 
		/// perform the Invoke</param>
		/// <param name="action">The delegate to run</param>
		public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
		{
			if (!dispatcher.CheckAccess())
			{
#if SILVERLIGHT
				dispatcher.BeginInvoke(action);
#else
				dispatcher.Invoke(DispatcherPriority.Normal, action);
#endif
			}
			else
			{
				action();
			}
		}

		/// <summary>
		/// A simple threading extension method, to invoke a delegate
		/// on the correct thread if it is not currently on the correct thread
		/// which can be used with DispatcherObject types.
		/// </summary>
		/// <param name="dispatcher">The Dispatcher object on which to 
		/// perform the Invoke</param>
		/// <param name="action">The delegate to run</param>
//		public static void InvokeInBackgroundIfRequired(this Dispatcher dispatcher, Action action)
//		{
//			if (!dispatcher.CheckAccess())
//			{
//#if SILVERLIGHT
//				dispatcher.BeginInvoke(action);
//#else
//				dispatcher.Invoke(DispatcherPriority.Background, action);
//#endif
//			}
//			else
//			{
//				action();
//			}
//		}

		/// <summary>
		/// A simple threading extension method, to invoke a delegate
		/// on the correct thread asynchronously if it is not currently on the correct thread
		/// which can be used with DispatcherObject types.
		/// </summary>
		/// <param name="dispatcher">The Dispatcher object on which to 
		/// perform the Invoke</param>
		/// <param name="action">The delegate to run</param>
		public static void InvokeWithoutBlocking(this Dispatcher dispatcher, Action action)
		{
#if SILVERLIGHT
			dispatcher.BeginInvoke(action);
#else
			dispatcher.BeginInvoke(DispatcherPriority.Background, action);
#endif
		}

	}
}
