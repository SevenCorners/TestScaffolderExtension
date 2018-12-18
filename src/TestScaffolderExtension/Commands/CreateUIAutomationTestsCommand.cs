using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using TestScaffolderExtension.Extensions;
using TestScaffolderExtension.Models;
using TestScaffolderExtension.Models.Solution;
using TestScaffolderExtension.Processors.UIAutomationTest;
using TestScaffolderExtension.ViewModels;
using TestScaffolderExtension.Views;
using Constants = EnvDTE.Constants;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Commands
{
    internal sealed class CreateUIAutomationTestsCommand : MenuCommandBase
    {
        private static readonly Guid _commandSet = new Guid("fed91778-c97a-4759-8037-97a8e009c9fe");

        protected override int CommandId => 256;
        protected override Guid CommandSet => _commandSet;

        private CreateUIAutomationTestsCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService)
        {
            AddCommandToMenu(commandService);
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in CreateUIAutomationTestsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var commandService = await package.GetAsAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new CreateUIAutomationTestsCommand(package, commandService);
        }

        protected override async Task ExecuteCommandAsync(OleMenuCommand menuCommand)
        {
            var dte = await AsyncServiceProvider.GetAsAsync<DTE, DTE2>();
            var selectedItems = await GetSolutionWindowSelectedItemsAsync(dte);
            var selectedProjectNode = await SolutionModelFactory.BuildHierarchyPathUpAsync(selectedItems.Single()) as ProjectModelBase;

            var automationTestOptions = await ShowCreateUIAutomationTestsWindowAsync(selectedProjectNode);
            var automationTestFiles = await UIAutomationTestTemplateInstantiator.InstantiateAsync(selectedProjectNode, automationTestOptions);
            foreach (var file in automationTestFiles)
            {
                await file.OpenAsync();
            }
        }

        private async Task<UIAutomationTestCreationOptions> ShowCreateUIAutomationTestsWindowAsync(ProjectModelBase selectedProjectNode)
        {
            var dte = await AsyncServiceProvider.GetAsync<DTE>();

            var createAutomationTestsViewModel = new CreateUIAutomationTestsViewModel(new UIAutomationTestCreationOptions());

            var createUIAutomationTestsWindow = new CreateUIAutomationTestsWindow(createAutomationTestsViewModel)
            {
                Owner = Application.Current.MainWindow
            };

            var dialogResult = createUIAutomationTestsWindow.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                return createAutomationTestsViewModel.TestCreationOptions;
            }

            return null;
        }

        private async Task<IEnumerable<object>> GetSolutionWindowSelectedItemsAsync(DTE2 dte)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var ui = dte.Windows.Item(Constants.vsWindowKindSolutionExplorer).Object as UIHierarchy;
            var selectedItems = ui.SelectedItems as object[];

            var hierarchyItemObjects = new List<object>();
            foreach (var item in selectedItems.Cast<UIHierarchyItem>())
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                hierarchyItemObjects.Add(item.Object);
            }
            return hierarchyItemObjects;
        }
    }
}