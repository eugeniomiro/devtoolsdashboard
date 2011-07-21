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
<CreationDate>2009-05-17 19:13:31Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Techno_Fly.Tools.Dashboard.Services
{
    /// <summary>
    /// Allows for the association of commands with content types.
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Adds a <seealso cref="CommandBinding"/> to the service, 
        /// which is associated with the active content.
        /// When the command binding's canExecute handler is called, 
        /// we get the active content <strong>β</strong> in the shell.
        /// If the active content is of the specified <code>TContent</code> 
        /// type then the specified <code>canExecuteHandler(β)</code> is called.
        /// When the command binding's execute handler is called, 
        /// we get the active content <strong>β</strong> in the shell.
        /// If the active content is of the specified <code>TContent</code> 
        /// type then the specified <code>executeHandler(β)</code> is called.
        /// </summary>
        /// <typeparam name="TContent">The type of the content that must be active 
        /// in the workspace.</typeparam>
        /// <param name="command">The command to register.</param>
        /// <param name="executeHandler">The execute handler. Must return <code>true</code> 
        /// if the command is to be marked as handled.</param>
        /// <param name="canExecuteHandler">The can execute handler. 
        /// If the handler returns <code>true</code>
        /// the command is executable (e.CanExecute is set to <code>true</code>), 
        /// otherwise the command will be not executable 
        /// (e.CanExecute is set to <code>false</code>.</param>
        void AddCommandBindingForContentType<TContent>(ICommand command,Func<TContent, bool> executeHandler,Func<TContent, bool> canExecuteHandler)where TContent : class;

        /// <summary>
        /// Adds a <seealso cref="CommandBinding"/> for the shell, 
        /// that is associated with the active content.
        /// When the command binding's canExecute handler is called, 
        /// we get the active content <strong>β</strong> in the shell.
        /// If the active content is of the specified <code>TContent</code> 
        /// type then the specified <code>canExecuteHandler(β)</code> is called.
        /// When the command binding's execute handler is called, 
        /// we get the active content <strong>β</strong> in the shell.
        /// If the active content is of the specified <code>TContent</code> 
        /// type then the specified <code>executeHandler(β)</code> is called.
        /// </summary>
        /// <typeparam name="TContent">The type of the content 
        /// that must be active in the workspace.</typeparam>
        /// <param name="command">The command to register.</param>
        /// <param name="executeHandler">The execute handler. 
        /// Must return <code>true</code> if the command 
        /// is to be marked as handled.</param>
        /// <param name="canExecuteHandler">The can execute handler. 
        /// If the handler returns <code>true</code> the command is executable 
        /// (e.CanExecute is set to <code>true</code>), otherwise the command 
        /// will be not executable (e.CanExecute is set to <code>false</code>.</param>
        void AddCommandBindingForContentType<TContent>(ICommand command,Func<TContent, object, bool> executeHandler,Func<TContent, object, bool> canExecuteHandler)where TContent : class;

        /// <summary>
        /// Adds a <seealso cref="CommandBinding"/> for the shell, 
        /// that is associated with the active content.
        /// When the command binding's canExecute handler is called, 
        /// we get the active content <strong>β</strong> in the shell.
        /// If the active content is of the specified <code>TContent</code> 
        /// type then the specified <code>canExecuteHandler(β)</code> is called.
        /// When the command binding's execute handler is called, 
        /// we get the active content <strong>β</strong> in the shell.
        /// If the active content is of the specified <code>TContent</code> 
        /// type then the specified <code>executeHandler(β)</code> is called.
        /// </summary>
        /// <typeparam name="TContent">The type of the content 
        /// that must be active in the workspace.</typeparam>
        /// <param name="command">The command to register.</param>
        /// <param name="executeHandler">The execute handler. 
        /// Must return <code>true</code> if the command 
        /// is to be marked as handled.</param>
        /// <param name="canExecuteHandler">The can execute handler. 
        /// If the handler returns <code>true</code> the command is executable 
        /// (e.CanExecute is set to <code>true</code>), otherwise the command 
        /// will be not executable (e.CanExecute is set to <code>false</code>.</param>
        /// <param name="keyGesture">The input gesture to trigger this command.</param>
        void AddCommandBindingForContentType<TContent>(ICommand command,Func<TContent, bool> executeHandler,Func<TContent, bool> canExecuteHandler,KeyGesture keyGesture)where TContent : class;

        void AddCommandBinding(ICommand command,Func<bool> executeHandler, Func<bool> canExecuteHandler);

        void AddCommandBinding(ICommand command,Func<bool> executeHandler, Func<bool> canExecuteHandler, KeyGesture keyGesture);

        void RegisterKeyGester(KeyGesture keyGesture, ICommand command);

        void RemoveCommandBinding(ICommand command);
    }
}


