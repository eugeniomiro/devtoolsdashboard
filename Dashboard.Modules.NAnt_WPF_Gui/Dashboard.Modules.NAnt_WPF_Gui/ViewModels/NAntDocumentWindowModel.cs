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
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NAntGui;
using NAntGui.Core;
using NAntGui.Framework;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism;
using System.ComponentModel;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    class NAntDocumentWindowModel : DockingWindowViewModelBase<NAntDocumentWindow>, IActiveAware 
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;

        private CommandLineOptions _options;
        private ILogsMessage _logger;

        private BuildRunnerBase _buildRunner;
        private readonly BackgroundWorker _loader = new BackgroundWorker();

        /// <summary>
        /// Creates new untitled document
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <param name="container"></param>
        /// <param name="regionManager"></param>
        /// <param name="eventAggregator"></param>
        public NAntDocumentWindowModel(ILogsMessage logger, CommandLineOptions options, IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(container, regionManager, eventAggregator)
        {
            _container = container;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            this.IsActiveChanged += new EventHandler(Command_IsActiveChanged);
            _loader.DoWork += LoaderDoWork;
            _loader.RunWorkerCompleted += LoaderRunWorkerCompleted;

            Assert.NotNull(logger, "logger");
            Assert.NotNull(options, "options");

            _options = options;
            _logger = logger;
            Name = "Untitled*";
            Directory = ".\\";
            FullName = Directory + Name;
            Contents = "";
            FileType = FileType.New;

            BuildScript = new BlankBuildScript();

            this.View = CreateTextDocumentWindow();
            this.View.DataContext = this;
            regionManager.AddToRegion("DocumentRegion", this.View);

        }


        public NAntDocumentWindowModel(string fileName,ILogsMessage logger, CommandLineOptions options, IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(container, regionManager, eventAggregator)
        {
            _container = container;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            this.IsActiveChanged += new EventHandler(Command_IsActiveChanged);
            _loader.DoWork += LoaderDoWork;
            _loader.RunWorkerCompleted += LoaderRunWorkerCompleted;

            // Create a view
            _options = options;
            _logger = logger;
            FullName = fileName;

            FileInfo fileInfo = new FileInfo(FullName);
            Name = fileInfo.Name;
            Directory = fileInfo.DirectoryName;

            Load();

            BuildScript = ScriptParserFactory.Create(fileInfo);
            _buildRunner = BuildRunnerFactory.Create(fileInfo, logger, _options);
            _buildRunner.Properties = BuildScript.Properties;

            this.View = CreateTextDocumentWindow();
            this.View.DataContext = this;      
            regionManager.AddToRegion("DocumentRegion", this.View);

            // Hook up events
            //this.View.textEditor.TextChanged +=new EventHandler(textEditor_TextChanged);

            // Create Menu Bindings

            // Initialize commands
            //this.BuildCommand = new DelegateCommand<object>(BuildCommand_Executed, Command_CanExecute);
            //Techno_Fly.Tools.Dashboard.Commands.Build.RegisterCommand(this.BuildCommand);
            //this.BuildCommand.IsActive = true;

            //this.SaveCommand  = new DelegateCommand<object>(SaveCommand_Executed, Command_CanExecute);
            //Techno_Fly.Tools.Dashboard.Commands.Save.RegisterCommand(this.SaveCommand);
            //this.SaveCommand.IsActive = true;

            _loader.RunWorkerAsync();

        }

        public CommandLineOptions Options
        {
            get { return _options; }
        }

        /// <summary>
        /// Creates a new text <see cref="DocumentWindow"/>.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private NAntDocumentWindow CreateTextDocumentWindow()
        {
            // Create the document
            NAntDocumentWindow documentWindow = new NAntDocumentWindow(Name, new BitmapImage(new Uri("/Resources/Images/TextDocument16.png", UriKind.Relative)));
            documentWindow.Title = Name;
            documentWindow.textEditor.Text = Contents;
            return documentWindow;
        }


        internal void ParseBuildScript()
        {
            BuildScript.Parse();
        }

        internal void Reload()
        {
            if (FileType == FileType.Existing)
            {
                Load();
                ParseBuildFile();
            }
        }

        internal void SaveAs(string filename, string contents)
        {
            Assert.NotNull(filename, "filename");
            Assert.NotNull(contents, "contents");

            FullName = filename;
            Contents = contents;

            File.WriteAllText(FullName, Contents);

            FileInfo fileInfo = new FileInfo(filename);
            Name = fileInfo.Name;
            Directory = fileInfo.DirectoryName;
            LastModified = fileInfo.LastWriteTime;
            FileType = FileType.Existing;

            BuildScript = ScriptParserFactory.Create(fileInfo);
            _buildRunner = BuildRunnerFactory.Create(fileInfo, _logger, _options);

            ParseBuildFile();
        }

        internal void Save(string contents, bool update)
        {
            File.WriteAllText(FullName, contents);
            LastModified = File.GetLastWriteTime(FullName);
            Contents = contents;

            if (update)
                ParseBuildFile();
        }

        private void Load()
        {
            FileType = FileType.Existing;
            Contents = File.ReadAllText(FullName);
            LastModified = File.GetLastWriteTime(FullName);
        }

        internal void Stop()
        {
            if (_buildRunner != null)
                _buildRunner.Stop();
        }

        internal void Run()
        {
            if (_buildRunner != null)
                _buildRunner.Run();
        }

        internal void SetTargets(List<IBuildTarget> targets)
        {
            Assert.NotNull(targets, "targets");
            if (_buildRunner != null)
                _buildRunner.Targets = targets;
        }

        //internal void Close()
        //{
        //    if (_buildRunner != null)
        //        _buildRunner.Stop();
        //}

        private void ParseBuildFile()
        {
            // Might want a more specific exception type to be caught here.
            // For example, a NullReferenceException on _buildScript 
            // shouldn't be ignored.
            try
            {
                BuildScript.Parse();
            }
#if DEBUG
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
#else
			catch
			{
				/* ignore */
			}
#endif
        }

        internal event EventHandler<BuildFinishedEventArgs> BuildFinished
        {
            add { _buildRunner.BuildFinished += value; }
            remove { _buildRunner.BuildFinished -= value; }
        }

        #region Properties

        public string Contents { get; set; }

        public FileType FileType { get; set; }

        public string FullName { get; private set; }

        public string Name { get; private set; }

        public string Directory { get; private set; }

        internal IBuildScript BuildScript { get; private set; }

        public DateTime LastModified { get; private set; }

        public DateTime IgnoreModifiedDate { get; set; }

        public override void WriteTraceMessage(string message)
        {
            Debug.Print(message);
        }

        protected override string ViewName
        {
            get
            {
                return "NAntDocumentWindow";
            }
        }

        //private string _title;
        //public string Title
        //{
        //    get { return _title; }
        //    set { _title = value; }
        //}

        #endregion

        //#region Commands

        //public DelegateCommand<object> BuildCommand { get; private set; }

        ///// <summary>
        ///// Occurs when the <see cref="ICommand"/> is executed.
        ///// </summary>
        ///// <param name="parameter">The command parameter.</param>
        //private void BuildCommand_Executed(object parameter)
        //{
        //    //tab.Save(false);
        //    //tab.SetProperties(_propertyGrid.GetProperties());
        //    //tab.SetTargets(_targetsTree.GetTargets());		
        //    Run();
        //}

        //public DelegateCommand<object> SaveCommand { get; private set; }

        ///// <summary>
        ///// Occurs when the <see cref="ICommand"/> is executed.
        ///// </summary>
        ///// <param name="parameter">The command parameter.</param>
        //private void SaveCommand_Executed(object parameter)
        //{
        //    Save(View.textEditor.Text, true);
        //}

        ///// <summary>
        ///// Occurs when the <see cref="ICommand"/> needs to determine whether it can execute.
        ///// </summary>
        ///// <param name="parameter">The command parameter.</param>
        ///// <returns>
        ///// <c>true</c> if the command can execute; otherwise, <c>false</c>/.
        ///// </returns>
        //private bool Command_CanExecute(object parameter)
        //{
        //    return true;
        //    //return this.View.OnClearCommandCanExecute();
        //}

        //#endregion


        // credits http://www.softinsight.com/bnoyes/2009/12/08/DetectingTheActiveViewInAPrismApp.aspx
        private bool _IsActive;
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                //SaveCommand.IsActive = value;
                IsActiveChanged(this, EventArgs.Empty);
            }
        }


        public event EventHandler IsActiveChanged = delegate { };

        /// <summary>
        /// Handler for IsActiveChanged events of registered commands.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">EventArgs to pass to the event.</param>
        private void Command_IsActiveChanged(object sender,EventArgs e)
        {
            //this.OnCanExecuteChanged();
            AddTargets(BuildScript);
        }


        internal void AddProperties(List<IBuildProperty> properties)
        {
            PropertiesViewModel propertiesViewModel = _container.Resolve<PropertiesViewModel>();

            //_propertyWindow.AddProperties(properties);
        }

        internal void AddTargets(IBuildScript buildScript)
        {
            TargetsViewModel targetsViewModel = _container.Resolve<TargetsViewModel>();
            targetsViewModel.ProjectName = buildScript.Name;
            targetsViewModel.SetTargets(buildScript.Targets); 

            //_targetsWindow.ProjectName = buildScript.Name;
            //_targetsWindow.SetTargets(buildScript.Targets);
        }

        private void LoaderDoWork(object sender, DoWorkEventArgs e)
        {
            //ParseBuildFile();
            ParseBuildScript();
        }

        private void LoaderRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //UpdateDisplay();
            Command_IsActiveChanged(this, null);
        }

        //private void ParseBuildFile(NAntDocumentWindowModel document)
        //{
        //    try
        //    {
        //        document.ParseBuildScript();
        //    }
        //    catch (BuildFileLoadException error)
        //    {
        //        Errors.CouldNotLoadFile(document.Name, error.InnerException.Message);
        //        SetCursor(error.LineNumber, error.ColumnNumber);
        //    }
        //}
    }
}
