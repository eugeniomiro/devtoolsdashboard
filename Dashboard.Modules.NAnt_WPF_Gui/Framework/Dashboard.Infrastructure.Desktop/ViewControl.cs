using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AvalonDock;
using Microsoft.Practices.Prism;

namespace Techno_Fly.Tools.Dashboard
{
    /// <summary>
    /// The base class for <see cref="IView"/>s.
    /// </summary>
    public class ViewControl : UserControl, IView //, IActiveAware /* (not abstract for Blendability) */
    {
        #region ViewModel Dependency Property

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(IViewModel), typeof(ViewControl), new PropertyMetadata(null, OnViewModelChanged));

        static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewControl = (ViewControl)d;
            viewControl.SetViewAwareAssociations((IViewModel)e.OldValue, (IViewModel)e.NewValue);
        }

        [Description("The view model for this view.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public IViewModel ViewModel
        {
            get
            {
                return (IViewModel)GetValue(ViewModelProperty);
            }
            set
            {
                SetValue(ViewModelProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewControl"/> class.
        /// </summary>
        public ViewControl()
        {
            Focusable = true;
            if (!EnvironmentValues.DesignTime)
            {
                Loaded += OnLoaded;
                //activeAwareUIElementAdapter = new ActiveAwareUIElementAdapter(this);
            }
        }

        bool alreadyLoaded;

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!alreadyLoaded)
            {
                alreadyLoaded = true;
                OnViewLoaded(e);
            }
        }

        #region ViewLoaded event

        event EventHandler<EventArgs> viewLoaded;

        /// <summary>
        /// Occurs when the view has been loaded.
        /// </summary>
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

        /// <summary>
        /// Closes the view.
        /// </summary>
        /// <param name="force">if set to <c>true</c> the control will be forced
        /// to close even if e.g., there is unsaved data and the user chooses 
        /// to cancel the closure.</param>
        /// <returns></returns>
        public virtual bool Close(bool force)
        {
            //var viewService = Dependency.Resolve<IViewService>("");
            //return viewService.CloseView(this, force);
            return true;
        }

        /// <summary>
        /// Raises the <see cref="E:ViewLoaded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnViewLoaded(EventArgs e)
        {
            if (viewLoaded != null)
            {
                viewLoaded(this, e);
            }
        }
        #endregion

        /// <summary>
        /// Gets a value indicating whether the control is in a designer.
        /// </summary>
        /// <value><c>true</c> if design time; otherwise, <c>false</c>.</value>
        protected bool DesignTime
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(this);
            }
        }

        #region IActiveAware and related

        void SetViewAwareAssociations(IViewModel oldViewModel, IViewModel newViewModel)
        {
            var oldViewAware = oldViewModel as IViewAware;
            var newViewAware = newViewModel as IViewAware;
            if (oldViewAware != null)
            {
                oldViewAware.DetachActiveAware();
            }

            if (newViewAware != null)
            {
                //newViewAware.Attach(activeAwareUIElementAdapter);
            }
        }

        //readonly ActiveAwareUIElementAdapter activeAwareUIElementAdapter;

        //bool IActiveAware.IsActive
        //{
        //    get
        //    {
        //        return activeAwareUIElementAdapter.IsActive;
        //    }
        //    set
        //    {
        //        activeAwareUIElementAdapter.IsActive = value;
        //    }
        //}

        //event EventHandler IActiveAware.IsActiveChanged
        //{
        //    add
        //    {
        //        activeAwareUIElementAdapter.IsActiveChanged += value;
        //    }
        //    remove
        //    {
        //        activeAwareUIElementAdapter.IsActiveChanged -= value;
        //    }
        //}

        #endregion
    }
}
