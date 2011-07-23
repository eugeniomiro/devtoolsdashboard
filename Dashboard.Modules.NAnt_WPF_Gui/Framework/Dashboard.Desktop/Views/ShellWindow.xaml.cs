using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Windows.Controls.Ribbon;
using Techno_Fly.Tools.Dashboard.Content;
using Techno_Fly.Tools.Dashboard.Gui;
using Techno_Fly.Tools.Dashboard.IO;
using Techno_Fly.Tools.Dashboard.Services;
using Techno_Fly.Tools.Dashboard.Shell.ViewModels;

namespace Techno_Fly.Tools.Dashboard.Shell.Views
{
    /// <summary>
    /// Interaction logic for ShellWindow.xaml
    /// </summary>
    public partial class ShellWindow : RibbonWindow, IShellWindow, IMainWindow, ICommandService
    {
        TabControl tabControl_Workspace;
        TabControl tabControl_Left;
        TabControl tabControl_Bottom;
        TabControl tabControl_Right;

        public ShellWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            AttachCommandBindings();

            Closing += OnWindowClosing;

            DataContext = new ShellWindowModel(this);

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //SetValue(RegionManager.RegionNameProperty, RegionNames.ShellWorkspaceRegion);

            // Insert code required on object creation below this point.
            // Initialize commands
            SaveCommand = new DelegateCommand<object>(SaveCommand_Executed, Command_CanExecute);
            Dashboard.Commands.Save.RegisterCommand(SaveCommand);
            SaveCommand.IsActive = true;

            this.AboutCommand = new DelegateCommand<object>(AboutCommand_Executed);
            Techno_Fly.Tools.Dashboard.Commands.About.RegisterCommand(this.AboutCommand);
            this.AboutCommand.IsActive = true;
        }

        bool _loaded;

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //UpdateAllRegionDimensions();
            if (_loaded)
            {
                return;
            }
            _loaded = true;

            AttachUIElementStateChangedEvent();

            //InitializeDimensionDefaults();
            OnViewLoaded(e);
        }

        void AttachUIElementStateChangedEvent()
        {
            var eventAggregator = Dependency.Resolve<IEventAggregator>("");
            var stateChangedEvent = eventAggregator.GetEvent<UIElementStateChangedEvent>();
            stateChangedEvent.Subscribe(OnUIElementStateChanged);
        }


        public IEnumerable<IView> WorkspaceViews
        {
            get
            {
                return GetWorkspaceViews();
            }
        }

        IRegionManager RegionManagerPrivate
        {
            get
            {
                var result = (IRegionManager)GetValue(RegionManager.RegionManagerProperty);
                if (result == null)
                {
                    result = Dependency.Resolve<IRegionManager>("");
                }
                return result;
            }
        }

        IEnumerable<IView> GetWorkspaceViews()
        {
            if (!RegionManagerPrivate.Regions.ContainsRegionWithName(RegionNames.ShellWorkspaceRegion))
            {
                /* This can occur if a command evaluation occurs 
                 * before the ShellView has finished applying templates. */
                return new List<IView>();
            }

            var workspaceRegion = RegionManagerPrivate.Regions[RegionNames.ShellWorkspaceRegion];
            if (workspaceRegion == null)
            {
                Debug.Fail("Workspace region should not be null.");
                return new List<IView>();
            }
            var views = from workspaceView in workspaceRegion.Views
                        select workspaceView;
            return views.OfType<IView>().ToList();

            return null;
        }

        IEnumerable<ISavableContent> GetAllSavableContent()
        {
            return GetAllContentOfType<ISavableContent>();
        }

        IEnumerable<TContent> GetAllContentOfType<TContent>() where TContent : class
        {
            var activeViews = (from region in RegionManagerPrivate.Regions
                               from view in region.ActiveViews
                               select view).OfType<IView>();

            var result = new List<TContent>();
            foreach (var view in activeViews)
            {
                var content = GetContentFromView<TContent>(view);
                if (content != null)
                {
                    result.Add(content);
                }
            }

            var workspaceViews = GetWorkspaceViews();
            foreach (var view in workspaceViews)
            {
                var content = GetContentFromView<TContent>(view);
                if (content != null && !result.Contains(content))
                {
                    result.Add(content);
                }
            }
            return result;
        }

        IView lastSelectedTabView;

        void workspaceTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != sender)
            {
                return;
            }

            lastSelectedTabView = null;

            var selectedTabView = tabControl_Workspace.SelectedItem as IView;
            lastSelectedTabView = selectedTabView;
            if (selectedTabView == null)
            {
                return;
            }

            /* When the workspace selected item changes
             * we raise the SelectedWorkspaceViewChangedEvent. */
            PublishActiveWorkspaceViewChanged(selectedTabView);
            //lastActiveView = selectedTabView;

            var content = selectedTabView as UIElement;
            if (content != null)
            {
                content.Focus();
            }
        }

        void PublishActiveWorkspaceViewChanged(IView workspaceView)
        {
            var eventAggregator = Dependency.Resolve<IEventAggregator>("");
            eventAggregator.GetEvent<SelectedWorkspaceViewChangedEvent>().Publish(workspaceView);
            eventAggregator.GetEvent<ActiveViewChangedInShellEvent>().Publish(workspaceView);
        }

        void PublishActiveNonWorkspaceViewChanged(IView view)
        {
            var eventAggregator = Dependency.Resolve<IEventAggregator>("");
            eventAggregator.GetEvent<SelectedWorkspaceViewChangedEvent>().Publish(null);
            eventAggregator.GetEvent<ActiveViewChangedInShellEvent>().Publish(view);
        }

        /// <summary>
        /// Gets the selected content in the shell. First we check the active region
        /// to see if it has content that is of the specified type, then we fall
        /// back on the current active view in the workspace.
        /// </summary>
        /// <returns>The selected content of the type specified, or <code>null</code>.</returns>
        TContent GetSelectedContent<TContent>() where TContent : class
        {
            var content = GetActiveRegionContent<TContent>();
            if (content != null)
            {
                return content;
            }
            var view = MainView;
            if (view == null)
            {
                return default(TContent);
            }
            content = view as TContent ?? view.ViewModel as TContent;
            return content;
        }

        TContent GetActiveRegionContent<TContent>() where TContent : class
        {
            var activeViews = (from region in RegionManagerPrivate.Regions
                               from view in region.ActiveViews
                               select view).OfType<IView>();
            foreach (var view in activeViews)
            {
                var content = GetContentFromView<TContent>(view);
                if (content != null)
                {
                    return content;
                }
            }
            return default(TContent);
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

        object GetActiveRegionContent(Type contentType)
        {
            var activeViews = (from region in RegionManagerPrivate.Regions
                               from view in region.ActiveViews
                               select view).OfType<IView>();
            foreach (var view in activeViews)
            {
                if (contentType.IsAssignableFrom(view.GetType()))
                {
                    return view;
                }

                if (view.ViewModel != null && contentType.IsAssignableFrom(view.ViewModel.GetType()))
                {
                    return view.ViewModel;
                }
            }
            return null;
        }

        public IView ActiveView
        {
            get
            {
                return GetActiveView();
            }
        }

        IRegion GetRegion(string regionName)
        {
            return RegionManagerPrivate.GetRegion(regionName);
        }

        IRegion GetViewRegion(IView view)
        {
            return RegionManagerPrivate.GetViewRegion(view);
        }

        #region CustomActiveView Dependency Property

        //public static DependencyProperty CustomActiveViewProperty = DependencyProperty.RegisterAttached(
        //    "CustomActiveView", typeof(IView), typeof(ShellView), new FrameworkPropertyMetadata(OnCustomActiveViewChanged));

        //public static void SetCustomActiveView(DependencyObject element, IView value)
        //{
        //    element.SetValue(CustomActiveViewProperty, value);
        //}

        //public static IView GetCustomActiveViewProperty(DependencyObject element)
        //{
        //    return (IView)element.GetValue(CustomActiveViewProperty);
        //}

        //IView customActiveView;

        //public static void OnCustomActiveViewChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        //{
        //    var shellView = obj as ShellView;
        //    if (shellView == null)
        //    {
        //        return;
        //    }
        //    var newValue = (IView)args.NewValue;
        //    shellView.customActiveView = newValue;
        //}

        #endregion

        IView GetActiveView()
        {
            //var view = customActiveView;
            //if (view != null)
            //{
            //    return view;
            //}

            /* First we look for the view of the focussed element. */
            var focusedElement = FocusManager.GetFocusedElement(this) as UIElement;
            if (focusedElement != null)
            {
                IView activeView = focusedElement.FindAncestorOrSelf<IView>();
                if (activeView != this) /* The shell view is an IView, and we don't want that. */
                {
                    /* At this point the focused item may not be the active view. */
                    if (tabControl_Workspace != null
                        && !tabControl_Workspace.Items.Contains(activeView))
                    {
                        return activeView;
                    }
                }
            }

            /* Otherwise we grab the first active view we find, starting with the workspace. */
            var workspaceContent = tabControl_Workspace != null
                ? tabControl_Workspace.SelectedContent as IView : null;
            if (workspaceContent != null)
            {
                return workspaceContent;
            }

            var leftContent = GetViewContent(tabControl_Left);
            if (leftContent != null)
            {
                return leftContent;
            }
            var bottomContent = GetViewContent(tabControl_Bottom);
            if (bottomContent != null)
            {
                return bottomContent;
            }

            var rightContent = GetViewContent(tabControl_Right);
            if (rightContent != null)
            {
                return rightContent;
            }

            return null;
        }

        #region MainView Attached Property

        public static DependencyProperty MainViewProperty = DependencyProperty.RegisterAttached(
            "MainView", typeof(IView), typeof(ShellWindow),
            new FrameworkPropertyMetadata(OnMainViewChanged));

        public static void SetMainView(DependencyObject element, IView value)
        {
            element.SetValue(MainViewProperty, value);
        }

        public static IView GetMainView(DependencyObject element)
        {
            return (IView)element.GetValue(MainViewProperty);
        }

        IView mainView;

        public static void OnMainViewChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = obj as UIElement;
            if (element == null)
            {
                return;
            }
            element.Focus();
            var shellView = element.FindAncestorOrSelf<ShellWindow>();
            if (shellView == null)
            {
                return;
            }
            IView newValue = (IView)args.NewValue;
            shellView.mainView = newValue;
            shellView.PublishActiveWorkspaceViewChanged(newValue);
        }

        #endregion

        public IView MainView
        {
            get
            {
                var view = mainView;
                if (view != null)
                {
                    return view;
                }
                return tabControl_Workspace != null
                    ? tabControl_Workspace.SelectedContent as IView : null;
            }
        }

        IView GetViewContent(TabControl tabControl)
        {
            if (tabControl != null)
            {
                var content = tabControl.SelectedContent as IView;
                if (content != null)
                {
                    var view = content as IActiveAware;
                    if (view != null && view.IsActive)
                    {
                        return (IView)view;
                    }
                }
            }
            return null;
        }

        void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var allViewsClosed = Close(false);
            if (!allViewsClosed)
            {
                e.Cancel = true;
            }
        }

        ISavableContent GetSavableContent()
        {
            var savableContent = GetSelectedContent<ISavableContent>();
            if (savableContent == null)
            {
                var savableContentProvider = GetSelectedContent<IContentProvider<ISavableContent>>();
                if (savableContentProvider != null)
                {
                    savableContent = savableContentProvider.Content;
                }
            }
            return savableContent;
        }

        #region External Command Binding support
        /// <summary>
        /// <see cref="ICommandService.AddCommandBindingForContentType{TContent}"/>
        /// </summary>
        public void AddCommandBindingForContentType<TContent>(ICommand command,
            Func<TContent, bool> executeHandler, Func<TContent, bool> canExecuteHandler)
            where TContent : class
        {
            /* When the workspace view changes, 
             * if the view is viewType then the specified command will be enabled depending 
             * on the result of the command.CanExecute. 
             * When the command is executed the current view's specified member will be called. */
            CommandBindings.Add(new CommandBinding(command,
                (sender, e) =>
                {
                    var content = GetSelectedContent<TContent>();
                    if (content == null)
                    {
                        /* Shouldn't get here because the CanExecute handler should prevent it. */
                        return;
                    }
                    e.Handled = executeHandler(content);
                },
                (sender, e) =>
                {
                    var content = GetSelectedContent<TContent>();
                    if (content == null)
                    {
                        e.CanExecute = false;
                        return;
                    }
                    e.CanExecute = canExecuteHandler(content);
                }));
        }

        /// <summary>
        /// <see cref="ICommandService.AddCommandBindingForContentType{TContent}"/>
        /// </summary>
        public void AddCommandBindingForContentType<TContent>(
            ICommand command,
            Func<TContent, object, bool> executeHandler,
            Func<TContent, object, bool> canExecuteHandler)
            where TContent : class
        {
            /* When the workspace view changes, 
             * if the view is viewType then the specified command 
             * will be enabled depending on the result of the command.CanExecute. 
             * When the command is executed the current view's specified member 
             * will be called. */
            CommandBindings.Add(new CommandBinding(command,
                (sender, e) =>
                {
                    var content = GetSelectedContent<TContent>();
                    if (content == null)
                    {
                        /* Shouldn't get here because the CanExecute handler 
                         * should prevent it. */
                        return;
                    }
                    e.Handled = executeHandler(content, e.Parameter);
                },
                (sender, e) =>
                {
                    var content = GetSelectedContent<TContent>();
                    if (content == null)
                    {
                        e.CanExecute = false;
                        return;
                    }
                    e.CanExecute = canExecuteHandler(content, e.Parameter);
                }));
        }

        /// <summary>
        /// <see cref="ICommandService.AddCommandBindingForContentType{TContent}"/>
        /// </summary>
        public void AddCommandBindingForContentType<TContent>(ICommand command,
            Func<TContent, bool> executeHandler,
            Func<TContent, bool> canExecuteHandler, KeyGesture keyGesture) where TContent : class
        {
            AddCommandBindingForContentType(command, executeHandler, canExecuteHandler);
            var keybinding = new KeyBinding(command, keyGesture);
            InputBindings.Add(keybinding);
        }

        public void AddCommandBinding(ICommand command,
            Func<bool> executeHandler, Func<bool> canExecuteHandler)
        {
            CommandBindings.Add(new CommandBinding(command,
                (sender, e) =>
                {
                    e.Handled = executeHandler();
                },
                (sender, e) =>
                {
                    e.CanExecute = canExecuteHandler();
                }));
        }

        public void AddCommandBinding(ICommand command,
            Func<bool> executeHandler, Func<bool> canExecuteHandler, KeyGesture keyGesture)
        {
            AddCommandBinding(command, executeHandler, canExecuteHandler);

            var keybinding = new KeyBinding(command, keyGesture);
            InputBindings.Add(keybinding);
        }

        public void RemoveCommandBinding(ICommand command)
        {
            ArgumentValidator.AssertNotNull(command, "command");

            var commandBinding = (from binding in CommandBindings.OfType<CommandBinding>()
                                  where binding.Command == command
                                  select binding).FirstOrDefault();

            if (commandBinding != null)
            {
                CommandBindings.Remove(commandBinding);
            }
        }

        public void RegisterKeyGester(KeyGesture keyGesture, ICommand command)
        {
            var keybinding = new KeyBinding(command, keyGesture);
            InputBindings.Add(keybinding);
        }
        #endregion

        #region ViewLoaded event

        event EventHandler<EventArgs> viewLoaded;

        public event EventHandler<EventArgs> ViewLoaded
        {
            add
            {
                viewLoaded += value;
            }
            remove
            {
                viewLoaded -= value;
            }
        }

        protected void OnViewLoaded(EventArgs e)
        {
            if (viewLoaded != null)
            {
                viewLoaded(this, e);
            }
        }
        #endregion

        public bool Close(bool force)
        {
            var viewService = Dependency.Resolve<IViewService>("");

            foreach (var view in WorkspaceViews)
            {
                if (!viewService.CloseView(view, false))
                {
                    return false;
                }
            }
            return true;
        }

        IView lastActiveView;

        void Grid_Root_GotFocus(object sender, RoutedEventArgs e)
        {
            SetActiveView();
        }

        void OnUIElementStateChanged(ViewStateChangedEventArgs e)
        {
            if (e.UIElement == null)
            {
                throw new ArgumentException("UIElement property should not be null");
            }
            var eventAggregator = Dependency.Resolve<IEventAggregator>("");
            var activeView = e.UIElement.FindAncestorOrSelf<IView>();
            if (activeView == null)
            {
                return;
            }

            var temp = lastActiveView;
            lastActiveView = activeView;

            if (activeView == temp)
            {
                switch (e.UIElementState)
                {
                    case UIElementState.Unloaded:
                    case UIElementState.LostFocus:
                        eventAggregator.GetEvent<ActiveViewChangedInShellEvent>().Publish(null);
                        break;
                    case UIElementState.Loaded:
                    case UIElementState.GotFocus:
                        eventAggregator.GetEvent<ActiveViewChangedInShellEvent>().Publish(activeView);
                        break;
                }
            }
            else
            {
                switch (e.UIElementState)
                {
                    case UIElementState.Loaded:
                    case UIElementState.GotFocus:
                        eventAggregator.GetEvent<ActiveViewChangedInShellEvent>().Publish(activeView);
                        break;
                }
            }
            lastActiveView = activeView;
        }

        /// <summary>
        /// Sets the active view to the view containing the focused element.
        /// If no view is found to be a parent of the focused element, 
        /// the the active workspace view is used and focused.
        /// </summary>
        void SetActiveView()
        {
            var focusedElement = FocusManager.GetFocusedElement(this);
            var control = focusedElement as UIElement;

            var contentControl = control as ContentControl;
            if (contentControl != null)
            {
                var temp = contentControl.Content as UIElement;
                if (temp != null)
                {
                    control = temp;
                }
            }
            IView activeView = null;
            if (control != null && !(control is IView))
            {
                activeView = control.FindAncestor<IView>();
                if (activeView == this)
                {
                    activeView = null;
                }
            }

            var main = MainView;
            if (activeView == null && main != null)
            {
                activeView = main;
                //var focusable = activeView as UIElement;
                //if (focusable != null)
                //{
                //    focusable.Focus();
                //}
            }

            if (activeView == lastActiveView)
            {
                return; /* Already signalled. */
            }
            lastActiveView = activeView;

            var eventAggregator = Dependency.Resolve<IEventAggregator>("");
            eventAggregator.GetEvent<ActiveViewChangedInShellEvent>().Publish(activeView);

            CommandManager.InvalidateRequerySuggested();
        }

        public DelegateCommand<object> SaveCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void SaveCommand_Executed(object parameter)
        {
            //Save(TextContent.Text, true);
            var savableContent = GetSavableContent();

            if (savableContent != null)
            {
                if (savableContent.NewFile)
                {
                    savableContent.SaveAs(FileErrorAction.UseAlternative);
                }
                else
                {
                    savableContent.Save(FileErrorAction.UseAlternative);
                }
                //e.Handled = true;
            }

        }

        public DelegateCommand<object> AboutCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void AboutCommand_Executed(object parameter)
        {
            WPFAboutBox1 about = new WPFAboutBox1(this);
            about.ShowDialog();
        }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> needs to determine whether it can execute.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>
        /// <c>true</c> if the command can execute; otherwise, <c>false</c>/.
        /// </returns>
        private bool Command_CanExecute(object parameter)
        {
            return true;
            //return this.View.OnClearCommandCanExecute();
        }

        public IViewModel ViewModel
        {
            get
            {
                return (IViewModel)DataContext;
            }
        }
    }
}
