using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using NAntGui.Core;
using NAntGui.Framework;
using Microsoft.Practices.Prism.Regions;
using System.Windows;
using Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.Views;
using Microsoft.Win32;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Commands;
using ICSharpCode.AvalonEdit.Document;

namespace Techno_Fly.Tools.Dashboard.Modules.NAnt_WPF_Gui.ViewModels
{
    public class MainController : ViewModelBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly OutputWindow _outputWindow;
        private readonly TargetsViewModel _targetsViewModel;

        private enum FocusedItems
        {
            Window,
            Output,
            Other
        }

        private readonly BackgroundWorker _loader = new BackgroundWorker();
        private readonly CommandLineOptions _options;

        //private NAntGuiForm _mainForm;
        private IEditCommands _editCommands;

        //private readonly Dictionary<DocumentWindow, NAntDocument> _documents = new Dictionary<DocumentWindow, NAntDocument>();

        private FocusedItems _focusedItem = FocusedItems.Other;

        public MainController(CommandLineOptions options, IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator)
            : base(container, regionManager, eventAggregator)
        {
            _container = container;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _outputWindow = _container.Resolve<OutputViewModel>().View;
            _targetsViewModel = _container.Resolve<TargetsViewModel>();

            Assert.NotNull(options, "options");
            _options = options;
            _loader.DoWork += LoaderDoWork;
            _loader.RunWorkerCompleted += LoaderRunWorkerCompleted;
            //RecentItems.ItemAdded += RecentItemsItemAdded;
            //Application.Idle += ApplicationIdle;
 
            // Create Menu Bindings
            // Initialize commands
            this.NewCommand = new DelegateCommand<object>(NewCommand_Executed);
            Techno_Fly.Tools.Dashboard.Commands.New.RegisterCommand(this.NewCommand);
            this.NewCommand.IsActive = true;

            this.OpenCommand = new DelegateCommand<object>(OpenCommand_Executed, Command_CanExecute);
            Techno_Fly.Tools.Dashboard.Commands.Open.RegisterCommand(this.OpenCommand);
            this.OpenCommand.IsActive = true;

            this.BuildCommand = new DelegateCommand<object>(BuildCommand_Executed, Command_CanExecute);
            Techno_Fly.Tools.Dashboard.Commands.Build.RegisterCommand(this.BuildCommand);
            this.BuildCommand.IsActive = true;

            this.SaveCommand = new DelegateCommand<object>(SaveCommand_Executed, Command_CanExecute);
            Techno_Fly.Tools.Dashboard.Commands.Save.RegisterCommand(this.SaveCommand);
            this.SaveCommand.IsActive = true;


            
            
        }

        void ApplicationIdle(object sender, EventArgs e)
        {
            CheckForFileChanges();
            UpdateInterface();
        }

        private void CheckForFileChanges()
        {
            //NAntDocument[] docs = new NAntDocument[_documents.Values.Count];
            //_documents.Values.CopyTo(docs, 0);

            //foreach (NAntDocument document in docs)
            //{
            //    if (document.FileType == FileType.Existing && _mainForm.IsActive)
            //    {
            //        CheckIfFileDeleted(document);
            //        CheckIfFileModified(document);
            //    }
            //}
        }

        private void CheckIfFileDeleted(NAntDocumentWindowModel document)
        {
            //if (!File.Exists(document.FullName))
            //{
            //    OpenDocumentDeleted(document);
            //}
        }

        private void CheckIfFileModified(NAntDocumentWindowModel document)
        {
            DateTime lastWrite = File.GetLastWriteTime(document.FullName);
            if (lastWrite > document.LastModified && lastWrite != document.IgnoreModifiedDate)
            {
                //DialogResult result = Errors.ShowDocumentChangedMessage(document.FullName);

                //if (result == DialogResult.Yes)
                //{
                //    LoadDocument(document.FullName);
                //}
                //else
                //{
                //    document.IgnoreModifiedDate = lastWrite;
                //}
            }
        }

        private void UpdateInterface()
        {
            //if (_documents.Count == 0)
            //    _mainForm.NoDocumentsOpen();
            //else
            //    _mainForm.DocumentsOpen();

            //// TODO: If running, this overrides the above to set the run button to 
            //// disabled. Should consolidate the two to prevent the duplication
            //_mainForm.UpdateBuildControls();

            //if (_focusedItem == FocusedItems.Output)
            //{
            //    _mainForm.UndoEnabled = false;
            //    _mainForm.RedoEnabled = false;
            //    _mainForm.OutputWindowHasFocus();
            //    _editCommands = _outputWindow;
            //}
            //else if (_focusedItem == FocusedItems.Window)
            //{
            //    _mainForm.UndoEnabled = ActiveWindow.CanUndo;
            //    _mainForm.RedoEnabled = ActiveWindow.CanRedo;
            //    _mainForm.ReloadEnabled = (ActiveDocument.FileType == FileType.Existing);
            //    _editCommands = ActiveWindow.EditCommands;
            //    _mainForm.EnableEditCommands();
            //}
            //else
            //{
            //    _mainForm.UndoEnabled = false;
            //    _mainForm.RedoEnabled = false;
            //    _mainForm.DisableEditCommands();
            //}
        }

        private void OpenDocumentDeleted(NAntDocumentWindowModel document)
        {
            //DocumentWindow window = FindDocumentWindow(document.FullName);
            //DialogResult result = Errors.ShowDocumentDeletedMessage(document.FullName);

            //if (result == DialogResult.No)
            //{
            //    window.Close();
            //}
            //else
            //{
            //    window.TabText = Utils.AddAsterisk(window.TabText);
            //    document.FileType = FileType.New;
            //}
        }

        private void RecentItemsItemAdded(object sender, RecentItemsEventArgs e)
        {
            //_mainForm.CreateRecentItemsMenu();
        }

        internal void NewBlankDocument()
        {
            NAntDocumentWindowModel doc = new NAntDocumentWindowModel(_outputWindow, _options, _container ,_regionManager ,_eventAggregator);
            //DocumentWindow window = new DocumentWindow(doc.FullName);
            //SetupWindow(window, doc);
        }

        internal void NewNAntProjectClicked()
        {
            //NewProjectForm form = new NewProjectForm();
            //form.NewClicked += CreateNewProject;
            //form.Show();
        }

        private void CreateNewProject(object sender, NewProjectEventArgs e)
        {
            //NAntDocument doc = new NAntDocument(_outputWindow, _options);
            //DocumentWindow window = new DocumentWindow(doc.FullName);
            //SetupWindow(window, doc);
            //window.Contents = Utils.GetNewDocumentContents(e.Info);
            //ParseBuildFile(doc);
        }

        internal void Run(List<IBuildTarget> targets)
        {
            if (IsDirty(ActiveWindow))
            {
                SaveDocument();
            }

            NAntDocumentWindowModel activeModel = (NAntDocumentWindowModel)ActiveWindow.DataContext;

            activeModel.SetTargets(targets);
            activeModel.Run();
        }

        internal void Stop()
        {
            ActiveDocument.Stop();
        }

        internal void SaveDocument()
        {
            SaveDocument(ActiveWindow);
        }


        internal void SaveDocumentAs()
        {
            SaveDocumentAs(ActiveWindow);
        }

        internal void SaveAllDocuments()
        {
            //foreach (DocumentWindow window in _mainForm.DockPanel.Documents)
            //{
            //    SaveDocument(window);
            //}
        }

        internal void ReloadActiveDocument()
        {
            //if (!IsDirty(ActiveWindow) || Errors.ReloadUnsaved() == DialogResult.Yes)
            //{
            //    try
            //    {
            //        TextLocation position = ActiveWindow.CaretPosition;
            //        ActiveDocument.Reload();
            //        ActiveWindow.Contents = ActiveDocument.Contents;
            //        ActiveWindow.MoveCaretTo(position.Line, position.Column);
            //        UpdateDisplay();
            //    }
            //    catch (Exception ex)
            //    {
            //        Errors.CouldNotLoadFile(ActiveDocument.FullName, ex.Message);
            //    }
            //}
        }

        internal void ReloadWindow(NAntDocumentWindowModel window)
        {
            //if (!IsDirty(window) || Errors.ReloadUnsaved() == DialogResult.Yes)
            //{
            //    NAntDocument document = _documents[window];

            //    try
            //    {
            //        TextLocation position = window.CaretPosition;
            //        document.Reload();
            //        window.Contents = document.Contents;
            //        window.MoveCaretTo(position.Line, position.Column);
            //        UpdateDisplay();
            //    }
            //    catch (Exception ex)
            //    {
            //        Errors.CouldNotLoadFile(document.FullName, ex.Message);
            //    }
            //}
        }

        internal void OpenDocument()
        {
            //foreach (string filename in BuildFileBrowser.BrowseForLoad())
            //{
            //    LoadDocument(filename);
            //}

            // Show a file open dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "NAnt Build files (*.*)|*.Build";
            if (dialog.ShowDialog() == true)
            {
                LoadDocument(dialog.FileName);
            }

        }

        /// <summary>
        /// Called with the Close File menu item is clicked.
        /// </summary>
        internal void Close()
        {
            ActiveWindow.Close();
        }

        internal void CloseAllDocuments()
        {
            //for (int index = _mainForm.DockPanel.Contents.Count - 1; index >= 0; index--)
            //{
            //    if (_mainForm.DockPanel.Contents[index] is DocumentWindow)
            //    {
            //        DocumentWindow window = (DocumentWindow)_mainForm.DockPanel.Contents[index];
            //        window.Close();
            //    }
            //}
        }

        internal static void ShowAboutForm()
        {
            //About about = new About();
            //about.ShowDialog();
        }

        internal void MainFormClosing()
        {
            // Don't need this event while closing (should be in CloseAllDocuments)
            //_mainForm.DockPanel.ActiveDocumentChanged -= DockPanelActiveDocumentChanged;
        }

        internal void SelectAll()
        {
            _editCommands.SelectAll();
        }

        internal void Copy()
        {
            _editCommands.Copy();
        }

        internal void Paste()
        {
            _editCommands.Paste();
        }

        internal static void ShowNAntDocumentation()
        {
            const string nantHelp = @"\..\nant-docs\help\index.html";
            Utils.LoadHelpFile(Utils.RunDirectory + nantHelp);
        }

        internal static void ShowNAntContribDocumentation()
        {
            const string nantContribHelp = @"\..\nantcontrib-docs\help\index.html";
            Utils.LoadHelpFile(Utils.RunDirectory + nantContribHelp);
        }

        internal static void ShowNAntSdkHelp()
        {
            const string nantHelpPath = @"\..\nant-docs\sdk\";
            const string nantSdkHelp = "NAnt-SDK.chm";
            string filename = Utils.RunDirectory + nantHelpPath + nantSdkHelp;

            Utils.LoadHelpFile(filename);
        }

        internal static void ShowOptions()
        {
            //OptionsForm optionsForm = new OptionsForm();
            //optionsForm.ShowDialog();
        }

        internal void LoadDocument(string filename)
        {
            NAntDocumentWindow window = null;
            //DocumentWindow window = FindDocumentWindow(filename);

            if (window != null)
            {
                //window.Select();
                //ReloadWindow(window);
            }
            else if (!File.Exists(filename))
            {
                Errors.FileNotFound(filename);
            }
            else
            {
                //NAntDocument doc = new NAntDocument(filename, _outputWindow, _options);
                //doc.BuildFinished += _mainForm.SetStateStopped;

                //Settings.Default.OpenScriptDir = doc.Directory;
                //Settings.Default.Save();

                //window = new DocumentWindow(doc.FullName);
                //SetupWindow(window, doc);

                //RecentItems.Add(doc.FullName);

                // Parse the file in the background
                //_loader.RunWorkerAsync();


                NAntDocumentWindowModel model = _container.Resolve<NAntDocumentWindowModel>(new ParameterOverride("fileName", filename), new ParameterOverride("logger",_outputWindow));
                //_regionManager.Regions["DocumentRegion"].Add(model.View);
            }
        }

        internal NAntDocumentWindowModel GetWindow(string filename)
        {
            NAntDocumentWindowModel window = null;

            //if (File.Exists(filename))
            //{
            //    NAntDocument doc = new NAntDocument(filename, _outputWindow, _options);
            //    doc.BuildFinished += _mainForm.SetStateStopped;

            //    window = new DocumentWindow(doc.FullName);
            //    SetupWindow(window, doc);

            //    RecentItems.Add(doc.FullName);

            //    ParseBuildFile(doc);
            //}
            //else
            //{
            //    Errors.FileNotFound(filename);
            //}

            return window;
        }

        internal void DragDrop(DragEventArgs e)
        {
            //LoadDocument(Utils.GetDragFilename(e));
        }

        internal static void DragEnter(DragEventArgs e)
        {
           // e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        internal void OutputWindowEnter(object sender, EventArgs e)
        {
            _focusedItem = FocusedItems.Output;
        }

        internal void OutputWindowLeave(object sender, EventArgs e)
        {
            _focusedItem = FocusedItems.Other;
        }

        internal void Cut()
        {
            _editCommands.Cut();
        }

        internal void Delete()
        {
            _editCommands.Delete();
        }

        internal void SetCursor(int lineNumber, int columnNumber)
        {
            //if (ActiveWindow != null) // can be null when the app is loading
            //    ActiveWindow.MoveCaretTo(lineNumber - 1, columnNumber - 1);
        }

        internal void Undo()
        {
            //ActiveWindow.Undo();
        }

        internal void Redo()
        {
            //ActiveWindow.Redo();
        }

        //internal void SetControls(NAntGuiForm mainForm, OutputWindow outputWindow)
        //{
        //    // may decouple the form and the controller (using events) later
        //    _mainForm = mainForm;
        //    _mainForm.DockPanel.ActiveDocumentChanged += DockPanelActiveDocumentChanged;

        //    _outputWindow = outputWindow;
        //    _outputWindow.Enter += OutputWindowEnter;
        //    _outputWindow.Leave += OutputWindowLeave;
        //    _outputWindow.FileNameClicked += OutputWindowFileNameClicked;

        //}

        //public void SelectWindow(string filename)
        //{
        //    DocumentWindow window = FindDocumentWindow(filename);

        //    if (window != null)
        //    {
        //        window.Activate();
        //    }
        //}


        #region Private Methods

        private void OutputWindowFileNameClicked(object sender, FileNameEventArgs e)
        {
            LoadDocument(e.FileName);
            //SetCursor(e.Point.X, e.Point.Y);
        }

        private void CloseDocument(object sender, EventArgs e)
        {
            //DocumentWindow window;

            //if (sender is DocumentWindow)
            //    window = sender as DocumentWindow;
            //else
            //    window = ActiveWindow;

            //NAntDocument document = _documents[window];

            //if (document.FileType == FileType.New)
            //{
            //    DialogResult result = Errors.DocumentNotSaved(document.Name);

            //    if (result == DialogResult.Yes)
            //    {
            //        SaveDocumentAs(window);
            //    }
            //    else if (result == DialogResult.Cancel)
            //    {
            //        e.Cancel = true;
            //    }
            //}
            //else if (IsDirty(window))
            //{
            //    DialogResult result = Errors.DocumentNotSaved(document.Name);

            //    if (result == DialogResult.Yes)
            //    {
            //        try
            //        {
            //            document.Save(window.Contents, false);
            //        }
            //        catch (Exception ex)
            //        {
            //            Errors.CouldNotSave(document.Name, ex.Message);
            //        }
            //    }
            //    else if (result == DialogResult.Cancel)
            //    {
            //        e.Cancel = true;
            //    }
            //}

            //if (!e.Cancel)
            //    _mainForm.RemoveDocumentMenuItem(document);

        }

        private NAntDocumentWindowModel FindDocumentWindow(string file)
        {
            //foreach (DocumentWindow window in _mainForm.DockPanel.Documents)
            //    if (_documents[window].FullName == file)
            //        return window;

            return null;
        }

        private void CloseAllButThisClicked()
        {
            //for (int index = _mainForm.DockPanel.Contents.Count - 1; index >= 0; index--)
            //{
            //    IDockContent content = _mainForm.DockPanel.Contents[index];
            //    if (content is DocumentWindow && content != ActiveWindow)
            //    {
            //        DocumentWindow window = content as DocumentWindow;
            //        window.Close();
            //    }
            //}
        }

        //private void SetupWindow(DocumentWindow window, NAntDocument doc)
        //{
        //    _documents.Add(window, doc);
        //    _mainForm.AddDocumentMenuItem(doc);

        //    window.Contents = doc.Contents;
        //    window.TabText = doc.Name;

        //    window.DocumentChanged += WindowDocumentChanged;
        //    window.FormClosing += CloseDocument;
        //    window.FormClosed += WindowFormClosed;
        //    window.CloseClicked += delegate { Close(); };
        //    window.CloseAllClicked += delegate { CloseAllDocuments(); };
        //    window.CloseAllButThisClicked += delegate { CloseAllButThisClicked(); };
        //    window.SaveClicked += delegate { SaveDocument(); };
        //    window.Leave += WindowLeave;
        //    window.Enter += WindowEnter;
        //    window.Show(_mainForm.DockPanel);
        //}

        void WindowEnter(object sender, EventArgs e)
        {
            _focusedItem = FocusedItems.Window;
        }

        void WindowLeave(object sender, EventArgs e)
        {
            _focusedItem = FocusedItems.Other;
        }

        private void UpdateDisplay()
        {
            //_mainForm.Text = string.Format("NAnt-Gui - {0}", ActiveWindow.TabText);
            //_mainForm.AddTargets(ActiveDocument.BuildScript);
            //_mainForm.AddProperties(ActiveDocument.BuildScript.Properties);

            // The following is required because when a file is loaded, update display gets called from ActiveDocumentChanged
            // before the document is finished loading and these values have been parsed.
            string description = String.Empty;

            // TODO: Figure out why this unused variable exists.
            string name = String.Empty;

            if (!String.IsNullOrEmpty(ActiveDocument.BuildScript.Description))
                description = ActiveDocument.BuildScript.Description.Replace(Environment.NewLine, String.Empty);
            if (!String.IsNullOrEmpty(ActiveDocument.BuildScript.Name))
                name = ActiveDocument.BuildScript.Name;

            //_mainForm.SetStatus(ActiveDocument.BuildScript.Name, description, ActiveDocument.FullName);
        }

        //private void WindowFormClosed(object sender, FormClosedEventArgs e)
        //{
        //    if (sender is DocumentWindow)
        //    {
        //        DocumentWindow window = sender as DocumentWindow;

        //        _documents[window].Close();
        //        _documents.Remove(window);
        //    }
        //}

        private bool IsDirty(NAntDocumentWindow window)
        {
            return false;
            //return window.Contents != _documents[window].Contents;
        }

        private NAntDocumentWindow ActiveWindow
        {
            get
            {
                IViewsCollection vc = _regionManager.Regions["DocumentRegion"].ActiveViews;
                if (vc != null) return (NAntDocumentWindow)vc.FirstOrDefault();
                else return null;
                //return _mainForm.DockPanel.ActiveDocument as DocumentWindow;
            }
        }

        private NAntDocumentWindowModel ActiveDocument
        {
            get
            {
                return null;
                //return (ActiveWindow != null) ? _documents[ActiveWindow] : null;
            }
        }

        //private void DockPanelActiveDocumentChanged(object sender, EventArgs e)
        //{
        //    if (ActiveWindow != null)
        //    {
        //        _mainForm.CheckActiveDocument(_documents[ActiveWindow]);
        //        UpdateDisplay();
        //    }
        //}

        private void LoaderDoWork(object sender, DoWorkEventArgs e)
        {
            ParseBuildFile(ActiveDocument);
        }

        private void LoaderRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateDisplay();
        }

        private void ParseBuildFile(NAntDocumentWindowModel document)
        {
            try
            {
                document.ParseBuildScript();
            }
            catch (BuildFileLoadException error)
            {
                Errors.CouldNotLoadFile(document.Name, error.InnerException.Message);
                SetCursor(error.LineNumber, error.ColumnNumber);
            }
        }

        private void WindowDocumentChanged(object sender, DocumentChangeEventArgs e)
        {
            
            /* 
                * The following is commented because the script is only parsed when the 
                * the document is saved.  Could change this, but will have to suppress
                * the errors.
                */
            //// Can't parse a file that doesn't exist on the harddrive
            //if (this.ActiveDocument.SourceFile.FileType == FileType.Existing &&
            //    !_loader.IsBusy)
            //{
            //    // Parse the file in the background
            //    _loader.RunWorkerAsync();
            //}

            UpdateTitle();
        }

        private void UpdateTitle()
        {
            //string name = IsDirty(ActiveWindow) && !Utils.HasAsterisk(ActiveDocument.Name) ? Utils.AddAsterisk(ActiveDocument.Name) : ActiveDocument.Name;
            //ActiveWindow.TabText = name;
            //_mainForm.UpdateDocumentMenuItem(ActiveDocument, name);
            //_mainForm.Text = String.Format("NAnt-Gui - {0}", name);
        }

        private void SaveDocument(NAntDocumentWindow window)
        {
            //NAntDocument document = _documents[window];

            //if (document.FileType == FileType.New)
            //{
            //    SaveDocumentAs(window);
            //}
            //else if (IsDirty(window))
            //{
            //    try
            //    {
            //        document.Save(window.Contents, true);

            //        if (window == ActiveWindow)
            //        {
            //            List<IBuildTarget> targets = _mainForm.SelectedTargets;
            //            UpdateTitle();
            //            UpdateDisplay();
            //            _mainForm.SelectedTargets = targets;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Errors.CouldNotSave(document.Name, ex.Message);
            //    }
            //}
        }

        private void SaveDocumentAs(NAntDocumentWindow window)
        {
            //string filename = BuildFileBrowser.BrowseForSave();
            //if (filename != null)
            //{
            //    NAntDocument document = _documents[window];

            //    try
            //    {
            //        document.SaveAs(filename, window.Contents);
            //        document.BuildFinished += _mainForm.SetStateStopped;

            //        Settings.Default.SaveScriptInitialDir = document.Directory;
            //        Settings.Default.Save();

            //        RecentItems.Add(filename);
            //        _mainForm.CreateRecentItemsMenu();
            //        UpdateTitle();
            //        UpdateDisplay();
            //    }
            //    catch (Exception ex)
            //    {
            //        Errors.CouldNotSave(document.Name, ex.Message);
            //    }
            //}
        }

        #endregion


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        #region Commands

        public DelegateCommand<object> NewCommand { get; private set; }
        public DelegateCommand<object> OpenCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void NewCommand_Executed(object parameter)
        {
            NewBlankDocument();
        }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void OpenCommand_Executed(object parameter)
        {
            OpenDocument();
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

        public DelegateCommand<object> BuildCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void BuildCommand_Executed(object parameter)
        {
            //tab.Save(false);
            //tab.SetProperties(_propertyGrid.GetProperties());
            //tab.SetTargets(_targetsTree.GetTargets());		

            Run(_targetsViewModel.SelectedTargets);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void SaveCommand_Executed(object parameter)
        {
            //Save(View.textEditor.Text, true);
        }


        #endregion

    }

}