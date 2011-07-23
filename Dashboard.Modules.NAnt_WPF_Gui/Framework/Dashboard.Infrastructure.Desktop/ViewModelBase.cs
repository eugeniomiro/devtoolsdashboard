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
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Techno_Fly.Tools.Dashboard.ComponentModel;
using Microsoft.Practices.Prism;

namespace Techno_Fly.Tools.Dashboard
{

    public abstract class ViewModelBase : NotifyPropertyChangeBase, IViewModel, IViewAware
    {
        IActiveAware _activeAwareInstance;

        protected ViewModelBase()
        {
            Initialize();
        }

        private IView _view;

        protected ViewModelBase(IView view)
        {
            ArgumentValidator.AssertNotNull(_view, "view");
            //notifier = new PropertyChangedNotifier(this);
            _view = view;
        }

        public IView View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
            }
        }



        [OnDeserializing]
        internal void OnDeserializing(StreamingContext context)
        {
            Initialize();
        }

        /// <summary>
        /// When deserialization occurs fields are not instantiated,
        /// therefore we must instanciate the notifier.
        /// </summary>
        void Initialize()
        {
            Container = Dependency.Resolve<IUnityContainer>("");
            RegionManager = Dependency.Resolve<IRegionManager>("");
            EventAggregator = Dependency.Resolve<IEventAggregator>("");
        }

        readonly Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        #region Title Property

        object _title;

        [Description("The text to display on a tab.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public object Title
        {
            get
            {
                return _title;
            }
            set
            {
                Assign("Title", ref _title, value);
            }
        }

        #endregion

        #region Active Aware

        void IViewAware.Attach(IActiveAware activeAware)
        {
            ReplaceActiveAware(activeAware);
        }

        void IViewAware.DetachActiveAware()
        {
            ReplaceActiveAware(null);
        }

        void ReplaceActiveAware(IActiveAware activeAwareInstance)
        {
            if (_activeAwareInstance != null)
            {
                _activeAwareInstance.IsActiveChanged -= OnIsActiveChanged;
            }
            _activeAwareInstance = activeAwareInstance;
            if (activeAwareInstance != null)
            {
                activeAwareInstance.IsActiveChanged += OnIsActiveChanged;
            }
        }

        bool _lastActiveState;

        void OnIsActiveChanged(object sender, EventArgs e)
        {
            Notifier.NotifyChanged("Active", _lastActiveState, Active);
            _lastActiveState = Active;
            OnActiveChanged(e);
        }

        #region event ActiveChanged

        private event EventHandler MyActiveChanged;

        protected event EventHandler ActiveChanged
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

        protected void OnActiveChanged(EventArgs e)
        {
            if (MyActiveChanged != null)
            {
                MyActiveChanged(this, e);
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance is being notified 
        /// of when it becomes active or inactive, 
        /// this may occur for example when its view gains focus or loses focus.
        /// </summary>
        /// <value><c>true</c> if monitoring the active state 
        /// of its view; otherwise, <c>false</c>.</value>
        public bool ActiveAware
        {
            get
            {
                return _activeAwareInstance != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ViewModelBase"/> 
        /// is active within the user interface.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get
            {
                return _activeAwareInstance != null ? _activeAwareInstance.IsActive : false;
            }
        }

        #endregion

        #region Dimensions
        double _width = 300;

        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                Assign("Width", ref _width, value);
            }
        }

        double _height = 300;

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                Assign("Height", ref _height, value);
            }
        }
        #endregion

        internal static T FindType<T>(object view) where T : class
        {
            var result = view as T;
            if (result != null)
            {
                return result;
            }
            var temp = view as IView;
            if (temp != null)
            {
                return temp.ViewModel as T;
            }
            return null;
        }

        public override string ToString()
        {
            return _title != null ? _title.ToString() : base.ToString();
        }

        public IUnityContainer Container { get; private set; }

        public IRegionManager RegionManager { get; private set; }

        public IEventAggregator EventAggregator { get; private set; }

    }
}
