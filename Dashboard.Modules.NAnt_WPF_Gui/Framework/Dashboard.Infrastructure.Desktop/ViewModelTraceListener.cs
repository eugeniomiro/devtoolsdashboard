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

using System.Diagnostics;

namespace Techno_Fly.Tools.Dashboard
{

    [Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ConfigurationElementType(typeof(Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.CustomTraceListenerData))]
    public class ViewModelTraceListener : Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners.CustomTraceListener
    {

        public ViewModelTraceListener()
        {
        }

        public ViewModelTraceListener(IDockableContentViewModel model)
        {
            _myViewModel = model;
        }

        public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, object data)
        {
            string message = null;
            Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry le = null;
            string prefix = "";
            string skipRegex = "";

            switch (eventType)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    prefix = "!!! ";
                    break;
                case TraceEventType.Warning:
                    prefix = "! ";
                    break;
            }

            if ((data) is Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry)
            {
                le = (Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry)data;
                message = le.Message;
            }
            else
            {
                message = data.ToString();
            }

            if (this.Attributes.ContainsKey("SkipRegex"))
            {
                skipRegex = this.Attributes["SkipRegex"];
            }

            if (skipRegex.Length > 0 && System.Text.RegularExpressions.Regex.IsMatch(message, skipRegex))
            {
                return;
            }

            if ((this.Formatter != null) && (le != null))
            {
                this.WriteLine(this.Formatter.Format(le));

            }
            else
            {
                this.WriteLine(string.Concat(prefix, message));
            }
        }


        public override void TraceData(System.Diagnostics.TraceEventCache eventCache, string source, System.Diagnostics.TraceEventType eventType, int id, params object[] data)
        {
        }

        public override void Write(string message)
        {
            if (_myViewModel != null)
            {
                _myViewModel.WriteTraceMessage(message);
            }
        }

        public override void WriteLine(string message)
        {
            if (_myViewModel != null)
            {
                _myViewModel.WriteTraceMessage(message);
            }
        }

        private static IDockableContentViewModel _myViewModel;
        public static IDockableContentViewModel MyViewModel
        {
            get { return _myViewModel; }
            set { _myViewModel = value; }
        }
    }

}
