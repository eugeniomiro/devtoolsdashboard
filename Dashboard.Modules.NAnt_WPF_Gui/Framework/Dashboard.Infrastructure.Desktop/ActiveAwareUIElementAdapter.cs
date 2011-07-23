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
using System.Diagnostics;
using System.Windows;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;

namespace Techno_Fly.Tools.Dashboard
{
    /// <summary>
    /// Wraps a <see cref="UIElement"/> to provide an <see cref="IActiveAware"/>
    /// implementation based on its focus state.
    /// </summary>
    public class ActiveAwareUIElementAdapter : IActiveAware
    {
        bool _active;

        public ActiveAwareUIElementAdapter(UIElement uiElement)
        {
            ArgumentValidator.AssertNotNull(uiElement, "uiElement");
            uiElement.GotKeyboardFocus += uiElement_GotFocus; //(o, args) => IsActive = true;
            uiElement.LostKeyboardFocus += uiElement_LostFocus; //(o, args) => IsActive = false;

            var frameworkElement = uiElement as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.Loaded += frameworkElement_Loaded; //(o, args) => IsActive = true;
                frameworkElement.Unloaded += frameworkElement_Unloaded; //(o, args) => IsActive = false;
            }
        }

        void uiElement_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.Print("uiElement_LostFocus: " + sender.ToString());
            PublishEvent((UIElement)sender, UIElementState.LostFocus);
            IsActive = false;
        }

        void uiElement_GotFocus(object sender, RoutedEventArgs e)
        {
            Debug.Print("uiElement_GotFocus: " + sender.ToString());
            PublishEvent((UIElement)sender, UIElementState.GotFocus);
            IsActive = true;
        }

        void frameworkElement_Unloaded(object sender, RoutedEventArgs e)
        {
            Debug.Print("frameworkElement_Unloaded: " + sender.ToString());
            PublishEvent((UIElement)sender, UIElementState.Unloaded);
            IsActive = false;
        }

        void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.Print("frameworkElement_Loaded: " + sender.ToString());
            PublishEvent((UIElement)sender, UIElementState.Loaded);
            IsActive = true;
        }

        void PublishEvent(UIElement uiElement, UIElementState uiElementState)
        {
            var eventAggregator = Dependency.Resolve<IEventAggregator>("");
            var stateChangedEvent = eventAggregator.GetEvent<UIElementStateChangedEvent>();
            stateChangedEvent.Publish(new ViewStateChangedEventArgs(uiElement, UIElementState.LostFocus));
        }

        public bool IsActive
        {
            get
            {
                return _active;
            }
            set
            {
                if (_active != value)
                {
                    _active = value;
                    OnIsActiveChanged(EventArgs.Empty);
                }
            }
        }

        #region event IsActiveChanged

        event EventHandler MyActiveChanged;

        public event EventHandler IsActiveChanged
        {
            add
            {
                MyActiveChanged += value;
            }
            remove
            {
                MyActiveChanged -= value;
            }
        }

        protected void OnIsActiveChanged(EventArgs e)
        {
            if (MyActiveChanged != null)
            {
                MyActiveChanged(this, e);
            }
        }

        #endregion
    }
}


