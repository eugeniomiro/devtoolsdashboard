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

using Microsoft.Practices.Prism.Commands;

namespace Techno_Fly.Tools.Dashboard
{

    /// <summary>
    /// Contains the application-defined commands.
    /// </summary>
    public static class Commands
    {

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // OBJECT
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes the <c>Commands</c> class.
        /// </summary>
        static Commands()
        {
            New = new CompositeCommand(true);
            Open = new CompositeCommand(true);
            Save = new CompositeCommand(true);
            SaveAs = new CompositeCommand(true);
            Print = new CompositeCommand(true);
            Add = new CompositeCommand(true);
            Clear = new CompositeCommand(true);
            Remove = new CompositeCommand(true);
            LoadLayout = new CompositeCommand(true);
            SaveLayout = new CompositeCommand(true);
            SetApplicationTheme = new CompositeCommand(true);
            Build = new CompositeCommand(true);
            About = new CompositeCommand(true);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // PUBLIC PROCEDURES
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand New { get; private set; }

        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand Open { get; private set; }

        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand Save { get; private set; }

        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand SaveAs { get; private set; }

        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand Print { get; private set; }



        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand Add { get; private set; }

        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand Clear { get; private set; }

        /// <summary>
        /// Gets the <see cref="CompositeCommand"/>.
        /// </summary>
        /// <value>The <see cref="CompositeCommand"/>.</value>
        public static CompositeCommand Remove { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static CompositeCommand LoadLayout { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static CompositeCommand SaveLayout { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static CompositeCommand SetApplicationTheme { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static CompositeCommand Build { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static CompositeCommand About { get; private set; }
    }
}

