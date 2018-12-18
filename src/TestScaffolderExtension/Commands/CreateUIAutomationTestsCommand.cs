using System;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using TestScaffolderExtension.Models;
using TestScaffolderExtension.Models.Solution;
using TestScaffolderExtension.Processors.UIAutomationTest;
using TestScaffolderExtension.ViewModels;
using TestScaffolderExtension.Views;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Commands
{
    internal sealed class CreateUIAutomationTestsCommand : MenuCommandBase
    {
        private static readonly Guid _commandSet = new Guid("fed91778-c97a-4759-8037-97a8e009c9fe");

        protected override int CommandId => 256;
        protected override Guid CommandSet => _commandSet;

        private CreateUIAutomationTestsCommand(AsyncPackage package)
            : base(package)
        { }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var instance = new CreateUIAutomationTestsCommand(package);
            await instance.InitializeInternalAsync();
        }

        protected override async Task ExecuteCommandAsync(OleMenuCommand menuCommand)
        {
            var selectedItems = await VisualStudio.GetSolutionWindowSelectedItemsAsync();
            if (selectedItems.Count() != 1)
            {
                ShowError("Invalid Selection", "Please select only one item.");
                return;
            }
            var selectedProjectNode = await SolutionModelFactory.BuildHierarchyPathUpAsync(selectedItems.Single()) as ProjectModelBase;

            var automationTestOptions = ShowCreateUIAutomationTestsWindow();
            var automationTestFiles = await UIAutomationTestTemplateInstantiator.InstantiateAsync(selectedProjectNode, automationTestOptions);
            foreach (var file in automationTestFiles)
            {
                await file.OpenAsync();
            }
        }
        private UIAutomationTestCreationOptions ShowCreateUIAutomationTestsWindow()
        {
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
    }
}