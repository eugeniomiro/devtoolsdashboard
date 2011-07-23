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


