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
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Regions;
using Techno_Fly.Tools.Dashboard.Gui;
using Techno_Fly.Tools.Dashboard.Gui.Adaptation;

namespace Techno_Fly.Tools.Dashboard.Services
{
    /// <summary>
    /// This service allows the association and interaction of UIElements 
    /// with workspace content, and provides the capability to have the visibility 
    /// of a UIElement change according to the visibility/activiteness 
    /// of a view <code>Type</code>.
    /// </summary>
    public interface IViewService : IViewTracking
    {
        /// <summary>
        /// Associates the visibility of the specified <see cref="IUIElement"/>
        /// with the <code>Type</code> of the active view. 
        /// When θ, the active view, changes, if θ is of the <code>Type</code>
        /// as specified by <code>workspaceContentType</code>, 
        /// then the specified <code>IUIElement</code> uiElement 
        /// will have its <see cref="IUIElement.Visibility"/> 
        /// property set to <see cref="Visibility.Visible"/>. 
        /// Otherwise, if θ is not of the <code>Type</code> specified 
        /// by <code>workspaceContentType</code>, then the specified <see cref="IUIElement"/>
        /// <code>uiElement</code> will have its <see cref="IUIElement.Visibility"/> property
        /// set to the specified <see cref="Visibility"/> <code>hiddenVisibility</code>.
        /// </summary>
        /// <param name="workspaceContentType">Type of the active workspace view 
        /// that will cause the specified <see cref="IUIElement"/> 
        /// to be made <see cref="Visibility.Visible"/>.</param>
        /// <param name="uiElement">The UI element to be made <see cref="Visibility.Visible"/> 
        /// or hidden according to the specified <code>hiddenVisibility</code>.</param>
        /// <param name="hiddenVisibility">The visibility value of a hidden UI Element. 
        /// Should be either <see cref="Visibility.Hidden"/> 
        /// or <see cref="Visibility.Collapsed"/>.</param>
        /// <example>
        /// ToolBar toolBar = new ToolBar(); 
        /// toolBar.Items.Add(new Button {Content = "Test"}); 
        /// regionManager.Regions[RegionNames.ToolBarTray].Add(toolBar); 
        /// var viewService = ServiceLocatorSingleton.UnityContainer.Resolve<IViewService>();
        /// viewService.AssociateVisibility(typeof(YourView), 
        /// new UIElementAdapter(toolBar), System.Windows.Visibility.Collapsed);
        /// </example>
        void AssociateVisibility(Type workspaceContentType,
            IUIElement uiElement, Visibility hiddenVisibility);

        /// <summary>
        /// Closes the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="force">if set to <c>true</c> the view will be closed
        /// even if it indicates that it would like to remain open,
        /// such as when it contains unsaved dirty content.</param>
        /// <returns><c>true</c> if the view was closed, <c>false</c> otherwise.</returns>
        bool CloseView(IView view, bool force);

        /// <summary>
        /// Gets the content region for the specified view content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The region in which the content resides, or <c>null</c>.</returns>
        IRegion GetContentRegion(object content);

        /// <summary>
        /// Registers the view so that it can be placed in the application's view menu.
        /// </summary>
        /// <param name="displayName">The text of the menu item. Can not be <c>null</c></param>
        /// <param name="showView">The method to handle displaying the view. 
        /// Can not be <c>null</c></param>
        /// <param name="showViewParam">An arbitrary object that may be used
        /// to identify the view to display. This object is passed to the showView handler.
        /// Can be <c>null</c></param>
        /// <param name="showGesture">The gesture that triggers showing of the view.
        /// Can be <c>null</c></param>
        /// <param name="icon">The icon to display in the menu. Can be <c>null</c></param>
        void RegisterView(string displayName, Action<object> showView,
            object showViewParam, KeyGesture showGesture, object icon);

        /// <summary>
        /// Registers the view so that it can be placed in the applications view menu.
        /// </summary>
        /// <param name="getDisplayName">A method to retrieve the display name, 
        /// which is displayed in the application's view menu. Can not be <c>null</c></param>
        /// <param name="showView">The method to handle displaying the view.
        /// Can not be <c>null</c></param>
        /// <param name="showViewParam">An arbitrary object that may be used
        /// to identify the view to display. This object is passed to the showView handler. 
        /// Can be <c>null</c></param>
        /// <param name="showGesture">The gesture that triggers showing of the view. 
        /// Can be <c>null</c></param>
        /// <param name="getIcon">A method that is used to retrieve 
        /// the icon to display in the menu. Can be <c>null</c></param>
        void RegisterView(Func<object, object> getDisplayName, Action<object> showView,
            object showViewParam, KeyGesture showGesture, Func<object, object> getIcon);

        /// <summary>
        /// Adds a view to the specified default region. The view may or may not be added
        /// to the specified region depending on whether the user has customized the location
        /// of the view previously. 
        /// </summary>
        /// <param name="defaultRegionName">Name of the default region to add the view.</param>
        /// <param name="view">The view.</param>
        /// <param name="activate">If <c>true</c> the view will be made visible.</param>
        /// <param name="regionName">The actual region where the view is located.</param>
        /// <exception cref="ArgumentNullException">If the specified view is null.</exception>
        /// <exception cref="ArgumentNullException">If defaultRegionName is null or empty.</exception>
        void ShowView(object view, string defaultRegionName, bool activate, out string regionName);

        /// <summary>
        /// Adds a view to the specified default region. The view may or may not be added
        /// to the specified region depending on whether the user has customized the location
        /// of the view previously. 
        /// </summary>
        /// <param name="newRegionName">Name of the region to move the view to.</param>
        /// <param name="view">The view.</param>
        /// <param name="activate">If <c>true</c> the view will be made visible.</param>
        /// <exception cref="ArgumentNullException">If the specified view is null.</exception>
        /// <exception cref="ArgumentNullException">If newRegionName is null or empty.</exception>
        void MoveView(object view, string newRegionName, bool activate);
    }
}

