#region File and License Information
/*
<File>
	<Copyright>Copyright © 2007, Daniel Vaughan. All rights reserved.</Copyright>
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
	<CreationDate>2010-01-03 13:05:47Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Techno_Fly.Tools.Dashboard
{
    /// <summary>
    /// Carries the payload for a composite event, and also allows an action to be cancelled.
    /// </summary>
    /// <typeparam name="TPayload">The type of the payload.</typeparam>
    public class CancelableEventArgs<TPayload>
    {
        bool _cancel;
        /// <summary>
        /// Gets or sets a value indicating whether the action should be cancelled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get
            {
                return _cancel;
            }
            set
            {
                if (!value) /* We do not allow the value to be set to false 
							 * because it may hide another handlers intent. [DV] */
                {
                    return;
                }
                _cancel = true;
            }
        }

        /// <summary>
        /// Gets or sets the payload used by the action.
        /// </summary>
        /// <value>The payload.</value>
        public TPayload Payload { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancelableEventArgs{TPayload}"/> class,
        /// using the specified payload. Can be <code>null</code>.
        /// </summary>
        /// <param name="payload">The payload. Can be null.</param>
        public CancelableEventArgs(TPayload payload)
        {
            Payload = payload;
        }

    }
}