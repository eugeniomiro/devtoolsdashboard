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
	<CreationDate>2010-02-03 18:42:20Z</CreationDate>
</File>
*/
#endregion

namespace Techno_Fly.Tools.Dashboard.Gui
{
    /// <summary>
    /// Allows for retrieval of the active and main views.
    /// </summary>
    public interface IViewTracking
    {
        /// <summary>
        /// Gets the view that is active within the interface.
        /// </summary>
        /// <value>The active view.</value>
        IView ActiveView
        {
            get;
        }

        /// <summary>
        /// Gets the active workspace view. This is the primary content in the interface, such as a document. 
        /// In the shell this is the selected tab content in the workspace region. 
        /// It may or may not be the equal to the <see cref="ActiveView"/>. 
        /// </summary>
        /// <value>The active workspace view.</value>
        IView MainView
        {
            get;
        }
    }
}

