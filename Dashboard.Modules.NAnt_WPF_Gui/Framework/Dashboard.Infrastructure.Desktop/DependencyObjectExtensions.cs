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
using System.Windows;
using System.Windows.Media;
using Techno_Fly.Tools.Dashboard;

namespace Techno_Fly.Tools.Dashboard
{

    public static class DependencyObjectExtensions
    {
        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject dependencyObject) where T : class
        {
            ArgumentValidator.AssertNotNull(dependencyObject, "dependencyObject");
            var accumulator = new List<T>();
            AccumulateChildrenOfType(dependencyObject, accumulator, true);
            return accumulator;
        }

        public static IEnumerable<T> GetChildrenOfTypeBreadthFirst<T>(this DependencyObject dependencyObject) where T : class
        {
            ArgumentValidator.AssertNotNull(dependencyObject, "dependencyObject");
            var accumulator = new List<T>();
            AccumulateChildrenOfType(dependencyObject, accumulator, false);
            return accumulator;
        }

        public static TAncestor FindAncestor<TAncestor>(this DependencyObject dependencyObject) where TAncestor : class
        {
            ArgumentValidator.AssertNotNull(dependencyObject, "dependencyObject");
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            while (parent != null)
            {
                var casted = parent as TAncestor;
                if (casted != null)
                {
                    return casted;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static TAncestor FindAncestorOrSelf<TAncestor>(this DependencyObject dependencyObject) where TAncestor : class
        {
            ArgumentValidator.AssertNotNull(dependencyObject, "dependencyObject");
            var result = dependencyObject as TAncestor;
            if (result != null)
            {
                return result;
            }
            result = dependencyObject.FindAncestor<TAncestor>();
            return result;
        }

        public static bool IsAncestorOf(this DependencyObject dependencyObject, DependencyObject child)
        {
            ArgumentValidator.AssertNotNull(dependencyObject, "dependencyObject");
            ArgumentValidator.AssertNotNull(child, "child");
            var accumulator = new List<DependencyObject>();
            /* We can improve efficiency here. There is no need to gather all. */
            AccumulateChildrenOfType(dependencyObject, accumulator, false);
            var result = accumulator.Contains(child);
            return result;
        }

        static void AccumulateChildrenOfType<T>(DependencyObject dependencyObject, ICollection<T> accumulator, bool depthFirst)
            where T : class
        {
            var children = LogicalTreeHelper.GetChildren(dependencyObject);
            foreach (var child in children)
            {
                //				if (child is System.Windows.Controls.Expander)
                //				{
                //					Console.WriteLine("");
                //				}

                var childOfType = child as T;
                if (childOfType != null && !accumulator.Contains(childOfType))
                {
                    accumulator.Add(childOfType);
                }
                if (depthFirst)
                {
                    var childDependencyObject = child as DependencyObject;
                    if (childDependencyObject != null)
                    {
                        AccumulateChildrenOfType(childDependencyObject, accumulator, true);
                    }
                }
            }

            if (depthFirst)
            {
                return;
            }

            /* Breadth first. */
            foreach (var child in children)
            {
                var childDependencyObject = child as DependencyObject;
                if (childDependencyObject != null)
                {
                    AccumulateChildrenOfType(childDependencyObject, accumulator, false);
                }
            }
        }
    }
}
