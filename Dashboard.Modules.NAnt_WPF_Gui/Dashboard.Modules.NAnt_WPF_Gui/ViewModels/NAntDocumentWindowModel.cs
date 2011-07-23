using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using NAntGui.Core;
using NAntGui.Framework;
using Techno_Fly.Tools.Dashboard.Content;
using Techno_Fly.Tools.Dashboard.IO;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views;
using ICSharpCode.AvalonEdit.Document;
using Techno_Fly.Tools.Dashboard.Services;
using Microsoft.Win32;
using Microsoft.Practices.Unity;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    [CLSCompliant(false)]
    public class NAntDocumentWindowModel : DockableContentViewModel<NAntDocumentWindow>, IContentProvider<ISavableContent>, ISavableContent
    {
        private readonly CommandLineOptions _options;
        private readonly ILogsMessage _logger;
        private readonly BackgroundWorker _loader = new BackgroundWorker();
        private BuildRunnerBase _buildRunner;
        private PropertiesView _propertiesView;
        private TargetsView _targetsView;

        #region TextContent Dependency Property

        TextDocument _textContent;
        [Description("Text content to display.")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TextDocument TextContent
        {
            get
            {
                return _textContent;
            }
            set
            {
                Notifier.Assign("TextContent", ref _textContent, value);
                Dirty = true;
            }
        }

        #endregion

        /// <summary>
        /// Creates new untitled document
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public NAntDocumentWindowModel(ILogsMessage logger, CommandLineOptions options)
        {
            IsActiveChanged += Command_IsActiveChanged;
            _loader.DoWork += LoaderDoWork;
            _loader.RunWorkerCompleted += LoaderRunWorkerCompleted;
            _options = options;
            _logger = logger;

            // Create Menu Bindings

            // Initialize commands
            BuildCommand = new DelegateCommand<object>(BuildCommand_Executed, Command_CanExecute);
            Dashboard.Commands.Build.RegisterCommand(BuildCommand);
            BuildCommand.IsActive = true;

            SaveCommand = new DelegateCommand<object>(SaveCommand_Executed, Command_CanExecute);
            Dashboard.Commands.Save.RegisterCommand(SaveCommand);
            SaveCommand.IsActive = true;
        }

        public NAntDocumentWindowModel(string fileName, ILogsMessage logger, CommandLineOptions options)
        {
            IsActiveChanged += Command_IsActiveChanged;
            _loader.DoWork += LoaderDoWork;
            _loader.RunWorkerCompleted += LoaderRunWorkerCompleted;
            _options = options;
            _logger = logger;

            // Create Menu Bindings

            // Initialize commands
            BuildCommand = new DelegateCommand<object>(BuildCommand_Executed, Command_CanExecute);
            Dashboard.Commands.Build.RegisterCommand(BuildCommand);
            BuildCommand.IsActive = true;

            SaveCommand = new DelegateCommand<object>(SaveCommand_Executed, Command_CanExecute);
            Dashboard.Commands.Save.RegisterCommand(SaveCommand);
            SaveCommand.IsActive = true;

        }

        public CommandLineOptions Options
        {
            get { return _options; }
        }

        internal void ParseBuildScript()
        {
            BuildScript.Parse();
        }

        internal void Reload()
        {
            if (FileType == FileType.Existing)
            {
                //Load();
                ParseBuildFile();
            }
        }

        internal void SaveAs(string filename, string content)
        {
            Assert.NotNull(filename, "filename");
            Assert.NotNull(content, "content");

            FullName = filename;
            TextContent = new TextDocument("content");

            File.WriteAllText(FullName, TextContent.Text);

            FileInfo fileInfo = new FileInfo(filename);
            Name = fileInfo.Name;
            Directory = fileInfo.DirectoryName;
            LastModified = fileInfo.LastWriteTime;
            FileType = FileType.Existing;

            BuildScript = ScriptParserFactory.Create(fileInfo);
            _buildRunner = BuildRunnerFactory.Create(fileInfo, _logger, _options);

            ParseBuildFile();
        }

        internal void Save(string content, bool update)
        {
            File.WriteAllText(FullName, content);
            LastModified = File.GetLastWriteTime(FullName);
            TextContent = new TextDocument(content);

            if (update) ParseBuildFile();
        }

        public void CreateNew(string name)
        {
            _fileName = name;
            Title = CreateName(_fileName);
        }

        public void CreateOpened(string name, string content)
        {
            CreateNew(name);
            TextContent = new TextDocument(content);
            Dirty = false;
            NewFile = false;
            FullName = name;

            FileType = FileType.Existing;
            LastModified = File.GetLastWriteTime(FullName);

            FileInfo fileInfo = new FileInfo(FullName);
            Name = fileInfo.Name;
            Directory = fileInfo.DirectoryName;

            BuildScript = ScriptParserFactory.Create(fileInfo);
            _buildRunner = BuildRunnerFactory.Create(fileInfo, _logger, _options);
            _buildRunner.Properties = BuildScript.Properties;

            _loader.RunWorkerAsync();
        }


        internal void Stop()
        {
            if (_buildRunner != null)
                _buildRunner.Stop();
        }

        internal void Run()
        {
            if (ActiveWindow != null && ActiveWindow.ViewModel == this)
            {
                if (_buildRunner != null)
                {
                    _buildRunner.Run();
                }
            }
        }

        private NAntDocumentWindow ActiveWindow
        {
            get
            {
                IViewsCollection vc = RegionManager.Regions["DocumentRegion"].ActiveViews;
                if (vc != null)
                {
                    foreach (var item in vc)
                    {
                        if (item is NAntDocumentWindow)
                        {
                            if (((IActiveAware)item).IsActive)
                            {
                                return (NAntDocumentWindow)item;
                            }
                        }
                    }
                }
                return null;
            }
        }

        internal void SetTargets(List<IBuildTarget> targets)
        {
            Assert.NotNull(targets, "targets");
            if (_buildRunner != null) _buildRunner.Targets = targets;
        }

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

        #endregion

        #region Commands

        public DelegateCommand<object> BuildCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void BuildCommand_Executed(object parameter)
        {
                _targetsView = Container.Resolve<TargetsView>("TargetsView");
                var view = new Uri("TargetsView", UriKind.Relative);
                RegionManager.RequestNavigate("LeftRegion", view);

                var model = (TargetsViewModel)_targetsView.ViewModel;

                Save(TextContent.Text, false);
                //SetProperties(_propertyGrid.GetProperties());
                //SetTargets(model.GetTargets());

                Run();
        }

        //private bool DisplayLoginScreen()
        //{
        //    Login frm = new Login();
        //    frm.ShowDialog();
        //    if (frm.DialogResult.HasValue && frm.DialogResult.Value)
        //    {
        //        _buildRunner.UserName = frm.txtUserName.Text;
        //        return true;
        //    }
        //    else
        //    {
        //        _buildRunner.UserName = "";
        //        return false;
        //    }
        //}

        public DelegateCommand<object> SaveCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void SaveCommand_Executed(object parameter)
        {
            Save(TextContent.Text, true);
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
            return CanSave;
            //return this.View.OnClearCommandCanExecute();
        }

        #endregion

        // credits http://www.softinsight.com/bnoyes/2009/12/08/DetectingTheActiveViewInAPrismApp.aspx

        public event EventHandler IsActiveChanged = delegate { };

        /// <summary>
        /// Handler for IsActiveChanged events of registered commands.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">EventArgs to pass to the event.</param>
        private void Command_IsActiveChanged(object sender, EventArgs e)
        {
            //this.OnCanExecuteChanged();
            //AddTargets(BuildScript);
        }

        internal void AddProperties(List<IBuildProperty> properties)
        {
            _propertiesView = Container.Resolve<PropertiesView>("PropertiesView");
            var view = new Uri("PropertiesView", UriKind.Relative);
            RegionManager.RequestNavigate("LeftRegion", view);

            var model = (PropertiesViewModel)_propertiesView.ViewModel;
            model.AddProperties(properties);
        }

        internal void AddTargets(IBuildScript buildScript)
        {
            _targetsView = Container.Resolve<TargetsView>("TargetsView");
            var view = new Uri("TargetsView", UriKind.Relative);
            RegionManager.RequestNavigate("LeftRegion", view);

            var model = (TargetsViewModel)_targetsView.ViewModel;
            model.ProjectName = buildScript.Name;
            model.SetTargets(buildScript.Targets);
        }

        private void LoaderDoWork(object sender, DoWorkEventArgs e)
        {
            ParseBuildFile();
            ParseBuildScript();
        }

        private void LoaderRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateDisplay();
            Command_IsActiveChanged(this, null);
        }

        private void UpdateDisplay()
        {
            //AddTargets(BuildScript);
            //AddProperties(BuildScript.Properties);
        }

        #region Implementation of IContentProvider<ISavableContent>

        public ISavableContent Content
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region Implementation of ISavableContent

        public bool CanSave
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Dirty Dependency Property

        private bool _dirty;
        public bool Dirty
        {
            get
            {
                return _dirty;
            }
            set
            {
                Notifier.Assign("Dirty", ref _dirty, value);
            }
        }

        #endregion

        private bool _newFile = true;
        public bool NewFile
        {
            get
            {
                return _newFile;
            }
            internal set
            {
                Notifier.Assign("NewFile", ref _newFile, value);
            }
        }

        public FileOperationResult Save(FileErrorAction fileErrorAction)
        {
            return SaveAux(fileErrorAction, false);
        }

        public FileOperationResult SaveAs(FileErrorAction fileErrorAction)
        {
            return SaveAux(fileErrorAction, true);
        }

        static string CreateName(string fileName)
        {
            var result = Path.GetFileName(fileName);
            return result;
        }

        string _fileName;
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                Notifier.Assign("FileName", ref _fileName, value);
            }
        }

        FileOperationResult SaveAux(FileErrorAction fileErrorAction, bool performSaveAs)
        {
            FileOperationResult result;
            string temp = _fileName;

            if (NewFile || performSaveAs)
            {
                result = SaveAux(fileErrorAction, true, ref temp);
            }
            else
            {
                result = SaveAux(fileErrorAction, false, ref temp);
            }

            if (result == FileOperationResult.Successful)
            {
                FileName = temp;
                NewFile = false;
                Title = CreateName(_fileName);
                Dirty = false;
            }

            return FileOperationResult.Successful;
        }

        FileOperationResult SaveAux(FileErrorAction fileErrorAction, bool performSaveAs, ref string tempFileName)
        {
            var fileService = Dependency.Resolve<IFileService>("");
            string nameTemp = tempFileName;

            FileOperationHandler handler =
                name =>
                {
                    string fileContent = null;
                    var dispatcher = Dependency.Resolve<Dispatcher>("");
                    dispatcher.InvokeIfRequired(delegate { fileContent = TextContent.Text; });
                    FileSystem.WriteFile(name, fileContent);
                    nameTemp = name;
                };

            FileOperationResult result = performSaveAs ? fileService.SaveAs(tempFileName, handler, fileErrorAction) : fileService.Save(tempFileName, handler, fileErrorAction);
            tempFileName = nameTemp;

            return result;
        }
    }
}
