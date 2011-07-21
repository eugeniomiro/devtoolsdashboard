#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
	<License see="prj:///Documentation/License.txt"/>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2008-11-03 23:22:06Z</CreationDate>
	<LastSubmissionDate>$Date: $</LastSubmissionDate>
	<Version>$Revision: $</Version>
</File>
*/
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
