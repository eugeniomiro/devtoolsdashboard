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
	<CreationDate>2010-02-05 13:31:57Z</CreationDate>
</File>
*/
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