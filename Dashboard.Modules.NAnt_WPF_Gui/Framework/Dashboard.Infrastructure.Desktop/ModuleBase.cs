using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Techno_Fly.Tools.Dashboard.Services;

namespace Techno_Fly.Tools.Dashboard
{
    /// <summary>
    /// Base class for <see cref="IModule"/>s.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        public virtual void Initialize()
        {
            Container = Dependency.Resolve<IUnityContainer>("");
            RegionManager = Dependency.Resolve<IRegionManager>("");
            EventAggregator = Dependency.Resolve<IEventAggregator>("");
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
                //throw new UserMessageException(regionName + " region was not found.", ex);
                throw new Exception(regionName + " region was not found.", ex); 
            }
        }

        /// <summary>
        /// Adds a view to the specified region.
        /// </summary>
        /// <param name="regionName">Name of the region to add the view.</param>
        /// <param name="view">The view.</param>
        /// <exception cref="ArgumentNullException">If the specified view is null.</exception>
        /// <exception cref="ArgumentException">If regionName is null or empty.</exception>
        protected void ShowView(string regionName, object view)
        {
            ShowView(regionName, view, false);
        }

        /// <summary>
        /// Adds a view to the specified region. If the view already exists in the UI
        /// it will not be added. If activate is <c>true</c> the view will be brought 
        /// forward to the user.
        /// </summary>
        /// <param name="regionName">Name of the region to add the view.</param>
        /// <param name="view">The view.</param>
        /// <param name="activate">If <c>true</c> the view will be immediatly brought to visibility. 
        /// If <c>false</c> it may or may not be made visible.
        /// For example, if the view is located in a TabControl, 
        /// if <c>true</c> the view will be made the <c>SelectedItem</c>.</param>
        /// <exception cref="ArgumentNullException">If the specified view is null.</exception>
        /// <exception cref="ArgumentException">If regionName is null or empty.</exception>
        protected void ShowView(string regionName, object view, bool activate)
        {
            ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");
            ArgumentValidator.AssertNotNull(view, "view");

            var viewService = Dependency.Resolve<IViewService>("");
            string actualRegionName;
            viewService.ShowView(view, regionName, activate, out actualRegionName);
        }

        /// <summary>
        /// Adds a list of views to a region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="views">The views.</param>
        /// <exception cref="ArgumentNullException">If the specified view list is null.</exception>
        /// <exception cref="ArgumentException">If regionName is null or empty.</exception>
        protected void ShowView(string regionName, IEnumerable<object> views)
        {
            ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");
            ArgumentValidator.AssertNotNull(views, "views");

            var viewService = Dependency.Resolve<IViewService>("");
            string actualRegionName;

            foreach (var view in views)
            {
                viewService.ShowView(view, regionName, false, out actualRegionName);
            }
        }

        public IUnityContainer Container { get; private set; }

        public IRegionManager RegionManager { get; private set; }

        public IEventAggregator EventAggregator { get; private set; }
    }
}
