using System.Windows;

using Microsoft.Practices.Prism.Events;

namespace Techno_Fly.Tools.Dashboard
{
    public class UIElementStateChangedEvent : CompositePresentationEvent<ViewStateChangedEventArgs>
    {
        /* Intentionally left blank. */
    }

    public enum UIElementState
    {
        Loaded, Unloaded, GotFocus, LostFocus
    }

    public class ViewStateChangedEventArgs
    {
        public UIElement UIElement { get; set; }
        public UIElementState UIElementState { get; set; }

        public ViewStateChangedEventArgs(UIElement uiElement, UIElementState uiElementState)
        {
            UIElement = uiElement;
            UIElementState = uiElementState;
        }

        public override string ToString()
        {
            return UIElement + " " + UIElementState;
        }
    }
}
