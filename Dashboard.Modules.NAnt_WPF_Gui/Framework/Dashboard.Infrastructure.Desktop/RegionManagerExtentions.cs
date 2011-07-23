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
using System.Linq;

using Microsoft.Practices.Prism.Regions;

namespace Techno_Fly.Tools.Dashboard 
{

    /// <summary>Metadata for class <see cref="DanielVaughan.Calcium.Modularity.RegionManagerExtensions"/></summary>
    public static class RegionManagerExtensionsMetadata
    {
        /// <summary>Refers to method <see cref="DanielVaughan.Calcium.Modularity.RegionManagerExtensions.GetRegion"/></summary>
        public const string GetRegion = "GetRegion";

        /// <summary>Refers to method <see cref="DanielVaughan.Calcium.Modularity.RegionManagerExtensions.GetViewRegion"/></summary>
        public const string GetViewRegion = "GetViewRegion";


    }

    /// <summary>
    /// Extension methods for the <see cref="IRegionManager"/> interface.
    /// </summary>
    public static class RegionManagerExtensions
    {
        /// <summary>
        /// Gets the region with the specified name.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="regionName">Name of the region to retrieve.</param>
        /// <returns>The region with the specified name.</returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs if regionManager or regionName is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Occurs if the number of regions is greater than 1.</exception>
        public static IRegion GetRegion(this IRegionManager regionManager, string regionName)
        {
            ArgumentValidator.AssertNotNull(regionManager, "regionManager");
            ArgumentValidator.AssertNotNullOrEmpty(regionName, "regionName");

            var regions = (from r in regionManager.Regions
                           where r.Name == regionName
                           select r).ToList();
            if (regions.Count > 1)
            {
                throw new InvalidOperationException("region " + regionName + " defined more than once.");
            }
            return regions.FirstOrDefault();
        }

        /// <summary>
        /// Gets the region where the specified view is located.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="view">The view.</param>
        /// <returns>The region for the specified view.</returns>
        /// <exception cref="ArgumentNullException">
        /// Occurs if regionManager or regionName is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Occurs if the number of regions is greater than 1.</exception>
        public static IRegion GetViewRegion(this IRegionManager regionManager, IView view)
        {
            ArgumentValidator.AssertNotNull(regionManager, "regionManager");
            ArgumentValidator.AssertNotNull(view, "view");

            var regions = (from r in regionManager.Regions
                           where r.Views != null && r.Views.Contains(view)
                           select r).ToList();
            if (regions.Count > 1)
            {
                /* Sanity check. */
                throw new InvalidOperationException("Multiple regions contain view " + view);
            }
            return regions.FirstOrDefault();
        }
    }
}
