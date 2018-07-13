using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        protected override void AddBeforeQueryStatus(OleMenuCommand menuCommand)
        {
            menuCommand.BeforeQueryStatus += MenuCommand_BeforeQueryStatus;
        }

        private void MenuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (!(sender is OleMenuCommand menuCommand)) return;

            menuCommand.Visible = false;
            menuCommand.Enabled = false;

            var dte = ServiceProvider.GetAs<DTE, DTE2>();
            var selectedItems = GetSolutionWindowSelectedItems(dte);

            if (selectedItems.Count() != 1)
            {
                return;
            }

            menuCommand.Visible = true;
            menuCommand.Enabled = true;
        }

        protected override async Task ExecuteCommandAsync(OleMenuCommand menuCommand)
        {
            var dte = await AsyncServiceProvider.GetAsAsync<DTE, DTE2>();
            var selectedItems = GetSolutionWindowSelectedItems(dte);
            var selectedProjectNode = SolutionModelFactory.BuildHierarchyPathUp(selectedItems.Single()) as ProjectModelBase;

            var automationTestOptions = await ShowCreateUIAutomationTestsWindowAsync(selectedProjectNode);
            foreach(var file in UIAutomationTestTemplateInstantiator.Instantiate(selectedProjectNode, automationTestOptions))
            {
                file.Open();
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

        private IEnumerable<object> GetSolutionWindowSelectedItems(DTE2 dte)
        {
            var ui = dte.Windows.Item(Constants.vsWindowKindSolutionExplorer).Object as UIHierarchy;
            var selectedItems = ui.SelectedItems as object[];

            return selectedItems.Cast<UIHierarchyItem>().Select(h => h.Object);
        }
    }
}