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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Techno_Fly.Tools.Dashboard.Collections;

//using Techno_Fly.Tools.Dashboard.Collections;

namespace Techno_Fly.Tools.Dashboard.Concurrency
{
	/// <summary>
	/// Indicates the manner in which delegates are invoked.
	/// </summary>
	public enum DelegateInvocationMode
	{
		/// <summary>
		/// Delegates are sent to the thread.
		/// </summary>
		Blocking,
		/// <summary>
		/// Delegates are posted to the thread.
		/// </summary>
		NonBlocking
	}

	/// <summary>
	/// Managers a collection of <see cref="Delegate"/> instances.
	/// Allows Delegates to be referenced directly 
	/// or using a <see cref="WeakReference"/>. 
	/// Allows Delegates to be handled on the thread of subscription.
	/// </summary>
	public class DelegateManager
	{
		readonly bool preserveThreadAffinity;
		readonly DelegateInvocationMode invocationMode;
		readonly IProvider<ISynchronizationContext> contextProvider;
		readonly bool useWeakReferences = true;
		readonly List<DelegateReference> delegateReferences = new List<DelegateReference>();
		readonly object membersLock = new object();
		readonly Dictionary<DelegateReference, ISynchronizationContext> synchronizationContexts
			= new Dictionary<DelegateReference, ISynchronizationContext>();

		/// <summary>Initializes a new instance 
		/// of the <see cref="DelegateManager"/> class.</summary>
		/// <param name="preserveThreadAffinity">If set to <c>true</c>, 
		/// delegate invocation  will occur using the <see cref="ISynchronizationContext"/> 
		/// provided by the specified context provider.</param>
		/// <param name="useWeakReferences">If <c>true</c> weak references will be used.</param>
		/// <param name="invocationMode">The invocation mode. 
		/// If <c>Blocking</c> delegates will be invoked 
		/// in serial, other in parallel.</param>
		/// <param name="contextProvider">The context provider, 
		/// which is used to supply a context when a delegate is added.
		/// If preservedThreadAffinity is <c>false</c>, this value will be ignored.</param>
		public DelegateManager(bool preserveThreadAffinity,
			bool useWeakReferences, 
			DelegateInvocationMode invocationMode, 
			IProvider<ISynchronizationContext> contextProvider)
		{
			this.preserveThreadAffinity = preserveThreadAffinity;
			this.invocationMode = invocationMode;
			this.useWeakReferences = useWeakReferences;

			this.contextProvider = contextProvider ?? new DualContextProvider();
		}

		public DelegateManager() 
			: this(true, false, DelegateInvocationMode.Blocking, new DualContextProvider())
		{
		}

		/// <summary>
		/// Adds the specified target delegate to the list of delegates 
		/// that are invoked when <see cref="InvokeDelegates"/> is called.
		/// </summary>
		/// <param name="targetDelegate">The target delegate.</param>
		public void Add(Delegate targetDelegate)
		{
			ArgumentValidator.AssertNotNull(targetDelegate, "targetDelegate");

			var reference = new DelegateReference(targetDelegate, useWeakReferences);

			lock (membersLock)
			{
				delegateReferences.Add(reference);
				if (preserveThreadAffinity)
				{
					synchronizationContexts[reference] = contextProvider.ProvidedItem;
				}
			}
		}

		/// <summary>
		/// Removes the specified target delegate from the list of delegates.
		/// </summary>
		/// <param name="targetDelegate">The target delegate.</param>
		public void Remove(Delegate targetDelegate)
		{
			lock (membersLock)
			{
				var removedItems = delegateReferences.RemoveAllAndReturnItems(
					reference =>
					{
						Delegate target = reference.Delegate;
						return target == null || targetDelegate.Equals(target);
					});

				if (preserveThreadAffinity)
				{
					foreach (var delegateReference in removedItems)
					{
						synchronizationContexts.Remove(delegateReference);
					}
				}
			}
		}

		/// <summary>
		/// Invokes each delegate.
		/// </summary>
		/// <param name="args">The args included during delegate invocation.</param>
		/// <exception cref="Exception">
		/// Rethrown exception if a delegate invocation raises an exception.
		/// </exception>
		public void InvokeDelegates(params object[] args)
		{
			IEnumerable<DelegateReference> delegates;
			/* Retrieve the valid delegates by first trim 
			 * the collection of null delegates. */
			lock (membersLock)
			{
				var removedItems = delegateReferences.RemoveAllAndReturnItems(
					listener => listener.Delegate == null);

				if (preserveThreadAffinity)
				{
					/* Clean the synchronizationContexts of those removed 
					 * in the preceding step. */
					foreach (var delegateReference in removedItems)
					{
						synchronizationContexts.Remove(delegateReference);
					}
				}
				/* The lock prevents changes to the collection, 
				 * therefore we can safely compile our list. */
				delegates = (from reference in delegateReferences
						   select reference).ToList();
			}

			/* At this point any changes to the delegateReferences collection 
			 * won't be noticed. */

			foreach (var reference in delegates)
			{
				if (!preserveThreadAffinity)
				{
					reference.Delegate.DynamicInvoke(args);
					continue;
				}

				var context = synchronizationContexts[reference];
				DelegateReference referenceInsideCloser = reference;
				Exception exception = null;

				var callback = new SendOrPostCallback(
					delegate
				        {
				            try
				            {
				                referenceInsideCloser.Delegate.DynamicInvoke(args);
				            }
				            catch (Exception ex)
				            {
				                exception = ex;
				            }
				        });

				switch (invocationMode)
				{
					case DelegateInvocationMode.Blocking:
						context.InvokeAndBlockUntilCompletion(callback, null);
						break;
					case DelegateInvocationMode.NonBlocking:
						context.InvokeWithoutBlocking(callback, null);
						break;
					default:
						throw new ArgumentOutOfRangeException("Unknown DispatchMode: " 
							+ invocationMode.ToString("G"));
				}

				/* Rethrowing the exception may be missed 
				 * in a DispatchMode.Post scenario. */
				if (exception != null) 
				{
					throw exception;
				}
			}
		}
	}
}
