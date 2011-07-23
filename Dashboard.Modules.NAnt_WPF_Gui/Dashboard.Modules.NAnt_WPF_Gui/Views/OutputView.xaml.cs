using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using AvalonDock;
using Microsoft.Practices.Prism;
using NAntGui.Framework;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views
{

    /// <summary>
    /// Represents a window for the properties window.
    /// </summary>
    public partial class OutputView : DockableContentView, ILogsMessage, IEditCommands
    {

        //public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(PropertiesView), new FrameworkPropertyMetadata(null));

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        // OBJECT
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes an instance of the <c>PropertiesView</c> class.
        /// </summary>
        public OutputView()
        {
            InitializeComponent();

            DataContext = ViewModel = new OutputViewModel();
        }

        #region Implementation of ILogsMessage

        public void LogMessage(string message)
        {
            //eventsListBox.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //{
            //    ListBoxItem item = new ListBoxItem();
            //    item.Content = message;
            //    eventsListBox.Items.Add(item);
            //    eventsListBox.SelectedItem = item;
            //    eventsListBox.ScrollIntoView(item);
            //}));

            TextBoxLog.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                OutputViewModel model = ((OutputViewModel)ViewModel);
                model.LogMessage(message);
                TextBoxLog.ScrollToEnd();
            }));



        }

        #endregion

        #region Implementation of IEditCommands

        public void Cut()
        {
            throw new NotImplementedException();
        }

        public void Copy()
        {
            throw new NotImplementedException();
        }

        public void Paste()
        {
            throw new NotImplementedException();
        }

        public void SelectAll()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
