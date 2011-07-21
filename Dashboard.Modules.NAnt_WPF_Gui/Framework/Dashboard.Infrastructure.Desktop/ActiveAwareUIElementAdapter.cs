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
<CreationDate>2010-02-05 11:48:23Z</CreationDate>
</File>
*/
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


