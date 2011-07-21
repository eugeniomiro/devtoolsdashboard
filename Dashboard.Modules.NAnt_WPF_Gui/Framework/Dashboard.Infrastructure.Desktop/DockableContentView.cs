using System;
using System.ComponentModel;
using System.Windows;
using AvalonDock;
using Microsoft.Practices.Prism;

namespace Techno_Fly.Tools.Dashboard
{
	/// <summary>
	/// The base class for <see cref="IView"/>s.
	/// </summary>
	public class DockableContentView : DockableContent, IView , IActiveAware /* (not abstract for Blendability) */
	{
        //public string DirtyTitle
        //{
        //    get
        //    {
        //        if(Dirty)
        //        {
        //            return Title + "*";
        //        }
        //        else return Title;
        //    } 
        //}

		#region ViewModel Dependency Property

		public static DependencyProperty ViewModelProperty = DependencyProperty.Register(
			"ViewModel", typeof(IViewModel), typeof(DockableContentView), new PropertyMetadata(null, OnViewModelChanged));

		static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var viewControl = (DockableContentView)d;
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
        /// Initializes a new instance of the <see cref="DockableContentView"/> class.
		/// </summary>
		public DockableContentView()
		{
			Focusable = true;

			if (!EnvironmentValues.DesignTime)
			{
				Loaded += OnLoaded;
				_activeAwareUIElementAdapter = new ActiveAwareUIElementAdapter(this);
			}
		}

		bool _alreadyLoaded;

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (!_alreadyLoaded)
			{
				_alreadyLoaded = true;
				OnViewLoaded(e);
			}
		}

		#region ViewLoaded event

		event EventHandler<EventArgs> MyViewLoaded;

		/// <summary>
		/// Occurs when the view has been loaded.
		/// </summary>
		public event EventHandler<EventArgs> ViewLoaded
		{
			add
			{
				MyViewLoaded += value;
			}
			remove
			{
				MyViewLoaded -= value;
			}
		}

        ///// <summary>
        ///// Closes the view.
        ///// </summary>
        ///// <param name="force">if set to <c>true</c> the control will be forced
        ///// to close even if e.g., there is unsaved data and the user chooses 
        ///// to cancel the closure.</param>
        ///// <returns></returns>
        //public virtual bool Close(bool force)
        //{
        //    //var viewService = Dependency.Resolve<IViewService>("");
        //    //return viewService.CloseView(this, force);
        //    //return true;
        //}

		/// <summary>
		/// Raises the <see cref="OnViewLoaded"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void OnViewLoaded(EventArgs e)
		{
			if (MyViewLoaded != null)
			{
				MyViewLoaded(this, e);
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
				newViewAware.Attach(_activeAwareUIElementAdapter);
			}
		}

		readonly ActiveAwareUIElementAdapter _activeAwareUIElementAdapter;

        bool IActiveAware.IsActive
        {
            get
            {
                return _activeAwareUIElementAdapter.IsActive;
            }
            set
            {
                _activeAwareUIElementAdapter.IsActive = value;
            }
        }

        event EventHandler IActiveAware.IsActiveChanged
        {
            add
            {
                _activeAwareUIElementAdapter.IsActiveChanged += value;
            }
            remove
            {
                _activeAwareUIElementAdapter.IsActiveChanged -= value;
            }
        }

		#endregion
	}
}
