//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Techno_Fly.Tools.Dashboard.IO;
using Techno_Fly.Tools.Dashboard.Modules.ILSpy.ViewModels;
using Techno_Fly.Tools.Dashboard.Modules.ILSpy.Views;
using Techno_Fly.Tools.Dashboard.Services;

namespace Techno_Fly.Tools.Dashboard.Modules.ILSpy
{
    [Module(ModuleName = "ILSpy")]
    public class Module : ModuleBase
    {
        //private readonly CommandLineOptions _options = new CommandLineOptions();
        //private OutputView _outputView;
        //private int _newFileCount;
        private const string FileFilter = "NAnt files (*.build)|*.build";

        public Module()
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            this.RegisterViewsAndServices();
            this.InitializeCommands();
        }

        private void InitializeCommands()
        {
            NewCommand = new DelegateCommand<object>(NewCommand_Executed);
            Dashboard.Commands.New.RegisterCommand(NewCommand);
            NewCommand.IsActive = true;

            OpenCommand = new DelegateCommand<object>(OpenCommand_Executed, Command_CanExecute);
            Dashboard.Commands.Open.RegisterCommand(OpenCommand);
            OpenCommand.IsActive = true;
        }

        private void RegisterViewsAndServices()
        {
            try
            {
                System.Diagnostics.Trace.WriteLine("NAnt_WPF_Gui.Initialize ...");

                Container.RegisterType<ModuleTaskButton>(new ContainerControlledLifetimeManager());
                RegionManager.Regions["TaskButtonRegion"].Add(Container.Resolve<ModuleTaskButton>());

                Container.RegisterType<Object, ModuleRibbonTab>("DeployServersModuleRibbonTab");

                //Container.RegisterType<Object, NAntDocumentWindow>("NAntDocumentWindow");
                //Container.RegisterType<Object, TargetsView>("TargetsView", new ContainerControlledLifetimeManager());
                //Container.RegisterType<Object, PropertiesView>("PropertiesView", new ContainerControlledLifetimeManager());
                //Container.RegisterType<Object, OutputView>("OutputView", new ContainerControlledLifetimeManager());
                //Container.RegisterType<Object, MainController>("MainController", new ContainerControlledLifetimeManager());

                //Container.RegisterType<ILogsMessage, OutputView>();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
                throw;
            }
        }

        bool OpenTextFile()
        {
            //Log.Info("Attempting to open a text file.");

            var fileService = Dependency.Resolve<IFileService>("");

            string fileNameUsed = null;
            string textFromFile = null;

            FileOperationResult operationResult = fileService.Open(
                name =>
                {
                    textFromFile = File.ReadAllText(name);
                    fileNameUsed = name;
                }, FileErrorAction.InformOnly, FileFilter);

            if (operationResult == FileOperationResult.Successful)
            {
                //_outputView = Container.Resolve<OutputView>("OutputView");
                //var view = new NAntDocumentWindow(_outputView, _options);
                //ShowView(RegionNames.DocumentRegion, view, true);
                //((NAntDocumentWindowModel)view.ViewModel).CreateOpened(fileNameUsed, textFromFile);
            }

            return true;
        }

        bool CreateNewTextFile()
        {
            //var view = new NAntDocumentWindow(_outputView, _options);
            //ShowView(RegionNames.DocumentRegion, view, true);
            //string newFileName = string.Format("Untitled{0}.build", ++_newFileCount);
            //((NAntDocumentWindowModel)view.ViewModel).CreateNew(newFileName);

            return true;
        }

        public DelegateCommand<object> NewCommand { get; private set; }
        public DelegateCommand<object> OpenCommand { get; private set; }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void NewCommand_Executed(object parameter)
        {
            CreateNewTextFile();
        }

        /// <summary>
        /// Occurs when the <see cref="ICommand"/> is executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        private void OpenCommand_Executed(object parameter)
        {
            OpenTextFile();
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

    }
}
