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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Techno_Fly.Tools.Dashboard.Content;
using Techno_Fly.Tools.Dashboard.Gui;
using Techno_Fly.Tools.Dashboard.Gui.Adaptation;
using Techno_Fly.Tools.Dashboard.IO;

namespace Techno_Fly.Tools.Dashboard.Services
{
	/// <summary>
	/// Default implementation of <see cref="IViewService"/>.
	/// Uses the <see cref="SelectedWorkspaceViewChangedEvent"/> for active view tracking.
	/// </summary>
	public partial class ViewService : IViewService
	{
		string substituteRegionName = "Left"; 

		class UIElementVisibility
		{
			public IUIElement UIElement { get; set; }
			public Visibility HiddenVisibility { get; set; }

			public override int GetHashCode() /* We use the UIElement as the collection key. */
			{
				return UIElement != null ? UIElement.GetHashCode() : base.GetHashCode();
			}
		}

		readonly Dictionary<Type, List<UIElementVisibility>> uiElementDictionary = new Dictionary<Type, List<UIElementVisibility>>();
		readonly ReaderWriterLockSlim uiElementDictionaryLock = new ReaderWriterLockSlim();

		bool initialized;

		[MethodImpl(MethodImplOptions.Synchronized)]
		void EnsureInitialized()
		{
			if (initialized)
			{
				return;
			}

			var eventAggregator = Dependency.Resolve<IEventAggregator>("");
			var viewChangedEvent = eventAggregator.GetEvent<SelectedWorkspaceViewChangedEvent>();
			viewChangedEvent.Subscribe(AssignVisibilitiesToUIElements, ThreadOption.UIThread, false);
			initialized = true;
		}

		/// <summary>
		/// Updates the ui elements by hiding those that do not have an associated <code>Type</code>
		/// that is implemented by the specified view.
		/// </summary>
		/// <param name="view">The view.</param>
		void AssignVisibilitiesToUIElements(IView view)
		{
			IEnumerable<UIElementVisibility> elements;
			uiElementDictionaryLock.EnterReadLock();
			try
			{
				elements = (from list in uiElementDictionary.Values from element in list select element).ToList();
			}
			finally
			{
				uiElementDictionaryLock.ExitReadLock();
			}

			if (view == null)
			{
				foreach (var element in elements)
				{
					element.UIElement.Visibility = element.HiddenVisibility;
				}
				return;
			}

			IEnumerable<UIElementVisibility> elementsToMakeVisible;

			uiElementDictionaryLock.EnterReadLock();
			try
			{
				elementsToMakeVisible = (from pair in uiElementDictionary where pair.Key.IsAssignableFrom(view.GetType())from element in pair.Value select element).ToList();
			}
			finally
			{
				uiElementDictionaryLock.ExitReadLock();
			}

			/* elements to hide = all elements / elementsToMakeVisible. */
			var elementsToHide = elements.Except(elementsToMakeVisible);

			foreach (var element in elementsToHide)
			{
				element.UIElement.Visibility = element.HiddenVisibility;
			}

			foreach (var element in elementsToMakeVisible)
			{
				element.UIElement.Visibility = Visibility.Visible;
			}
		}

		public void AssociateVisibility(Type workspaceContentType, IUIElement uiElement,
			Visibility hiddenVisibility)
		{
			ArgumentValidator.AssertNotNull(workspaceContentType, "workspaceContentType");
			ArgumentValidator.AssertNotNull(uiElement, "uiElement");

			EnsureInitialized(); /* This call is synchronized to ensure that it is only performed once. */

			var uiElementVisibility = new UIElementVisibility { UIElement = uiElement, HiddenVisibility = hiddenVisibility };

			uiElementDictionaryLock.EnterWriteLock(); /* We avoid the complexity of upgrading a read lock here. */
			try
			{
				List<UIElementVisibility> uiElements;
				if (!uiElementDictionary.TryGetValue(workspaceContentType, out uiElements))
				{
					uiElements = new List<UIElementVisibility>();
					uiElementDictionary.Add(workspaceContentType, uiElements);
				}
				if (uiElements.Contains(uiElementVisibility))
				{
					return;
				}
				uiElements.Add(uiElementVisibility);
			}
			finally
			{
				uiElementDictionaryLock.ExitWriteLock();
			}
		}

		public bool CloseView(IView view, bool force)
		{
			if (!force)
			{
				ISavableContent savableContent = null;

				var provider = GetContentFromView<IContentProvider<ISavableContent>>(view);
				if (provider != null)
				{
					savableContent = provider.Content;
				}
				if (savableContent == null)
				{
					savableContent = GetContentFromView<ISavableContent>(view);
				}

				if (savableContent != null && savableContent.CanSave && savableContent.Dirty)
				{
					var messageService = Dependency.Resolve<IMessageService>("");
					var userResponse = messageService.AskYesNoCancelQuestion(string.Format("Save changes to '{0}'?", savableContent.FileName),"",null);
					if (userResponse == YesNoCancelQuestionResult.Yes)
					{
						if (savableContent.NewFile)
						{
							if (savableContent.Save(FileErrorAction.UseAlternative) != FileOperationResult.Successful)
							{
								return false;
							}
						}
						else
						{
							if (savableContent.SaveAs(FileErrorAction.UseAlternative) != FileOperationResult.Successful)
							{
								return false;
							}
						}
					}

					if (userResponse == YesNoCancelQuestionResult.Cancel)
					{
						return false;
					}
				}
			}

			var region = GetContentRegion(view);
			region.Remove(view);

			return true;
		}

		public void ShowView(object view, string defaultRegionName, bool activate, out string regionName)
		{
			regionName = null;
			var region = GetContentRegion(view);
			if (region == null)
			{
				region = GetRegion(defaultRegionName);
				if (region == null)
				{
					region = GetRegion(substituteRegionName);
					if (region == null)
					{
						throw new InvalidOperationException(String.Format("Default region \"{0}\" does not exist.", substituteRegionName));
					}
					regionName = substituteRegionName;
				}
				region.Add(view);
			}

			if (regionName == null)
			{
				regionName = defaultRegionName;
			}

			if (activate)
			{
				region.Activate(view);
			}
		}

		public void MoveView(object view, string newRegionName, bool activate)
		{
			var region = GetRegion(newRegionName);
			if (region == null)
			{
				throw new ArgumentOutOfRangeException(String.Format("Region \"{0}\" does not exist.", newRegionName));
			}
			ForceViewToRegion(view, region, activate);
		}

		void ForceViewToRegion(object view, IRegion newRegion, bool activate)
		{
			if (newRegion.Views.Contains(view))
			{
				if (activate)
				{
					newRegion.Activate(view);
				}
				return;
			}

			var region = GetContentRegion(view);
			if (region != null)
			{
				region.Remove(view);
			}

			newRegion.Add(view);

			if (activate)
			{
				newRegion.Activate(view);
			}
		}

		/// <summary>
		/// Gets the region with the specified name.
		/// </summary>
		/// <param name="regionName">Name of the region.</param>
		/// <returns></returns>
		/// <exception cref="UserMessageException">If a region with the specified name was not found.</exception>
		static protected IRegion GetRegion(string regionName)
		{
			ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");

			var regionManager = Dependency.Resolve<IRegionManager>("");
			try
			{
				return regionManager.Regions[regionName];
			}
			catch (KeyNotFoundException ex)
			{
				throw new Exception(regionName + " region was not found.", ex); 
			}
		}

		public IRegion GetContentRegion(object content)
		{
			var regionManager = Dependency.Resolve<IRegionManager>("");

			var result = (from region in regionManager.Regions
						  where region.Views.Contains(content)
						  select region).FirstOrDefault();
			return result;
		}

		static TContent GetContentFromView<TContent>(IView view) where TContent : class
		{
			var content = view as TContent;
			if (content != null)
			{
				return content;
			}
			content = view.ViewModel as TContent;
			return content;
		}

		#region ShowViewCommand
		DelegateCommand<object> showViewCommand;
		public DelegateCommand<object> ShowViewCommand
		{
			get
			{
				if (showViewCommand == null)
				{
					showViewCommand = new DelegateCommand<object>(OnShowView);
				}
				return showViewCommand;
			}
		}

		void OnShowView(object obj)
		{
			var param = obj as OpenViewCommandParameter;
			if (param == null)
			{
				return;
			}
			param.EventAction(param.Param);
		}
		#endregion

		class OpenViewCommandParameter
		{
			public Action<object> EventAction { get; private set; }
			public object Param { get; private set; }

			public OpenViewCommandParameter(Action<object> eventAction, object param)
			{
				EventAction = eventAction;
				Param = param;
			}
		}

		public void RegisterView(string displayName, Action<object> showView, object showViewParam, KeyGesture showGesture, object icon)
		{
			var regionManager = Dependency.Resolve<IRegionManager>("");
			IRegion viewRegion;
			try
			{
				viewRegion = regionManager.Regions[MenuNames.View];
			}
			catch (KeyNotFoundException ex)
			{
				throw new Exception(MenuNames.View + " region was not found.", ex); 
			}

			var commandParameter = new OpenViewCommandParameter(showView, showViewParam);

			var menuItem = new MenuItem {
				Command = ShowViewCommand, 
				Header = displayName, 
				CommandParameter = commandParameter};

			viewRegion.Add(menuItem);

		}

		public void RegisterView(Func<object, object> getDisplayName, Action<object> showView, 
			object showViewParam, KeyGesture showGesture, Func<object, object> getIcon)
		{
			var regionManager = Dependency.Resolve<IRegionManager>("");
			IRegion viewRegion;
			try
			{
				viewRegion = regionManager.Regions[MenuNames.View];
			}
			catch (KeyNotFoundException ex)
			{
				throw new Exception(MenuNames.View + " region was not found.", ex); 
			}

			var commandParameter = new OpenViewCommandParameter(showView, showViewParam);

			var menuItem = new MenuItem
			{
				Command = ShowViewCommand,
				Header = getDisplayName(showViewParam),
				CommandParameter = commandParameter
			};
			viewRegion.Add(menuItem);

		}
	}
}
