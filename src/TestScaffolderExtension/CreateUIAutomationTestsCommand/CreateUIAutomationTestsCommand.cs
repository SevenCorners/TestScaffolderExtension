namespace TestScaffolderExtension.CreateUIAutomationTestsCommand
{
    using System;
    using System.Linq;
    using System.Windows;
    using Microsoft.VisualStudio.Shell;
    using TestScaffolderExtension.Common.Command;
    using TestScaffolderExtension.CreateUIAutomationTestsCommand.TemplateProcessing;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution;
    using Task = System.Threading.Tasks.Task;

    internal sealed class CreateUIAutomationTestsCommand : MenuCommandBase
    {
        private static readonly Guid CommandSetValue = new Guid("fed91778-c97a-4759-8037-97a8e009c9fe");

        private CreateUIAutomationTestsCommand(AsyncPackage package)
            : base(package)
        {
        }

        protected override int CommandId => 256;

        protected override Guid CommandSet => CommandSetValue;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var instance = new CreateUIAutomationTestsCommand(package);
            await instance.InitializeInternalAsync();
        }

        protected override async Task ExecuteCommandAsync(OleMenuCommand menuCommand)
        {
            var selectedItems = await this.VisualStudio.GetSolutionWindowSelectedItemsAsync();
            if (selectedItems.Count() != 1)
            {
                this.ShowError("Invalid Selection", "Please select only one item.");
                return;
            }

            var selectedProjectNode = await SolutionModelFactory.BuildHierarchyPathUpAsync(selectedItems.Single()) as ProjectModelBase;

            var automationTestOptions = this.ShowCreateUIAutomationTestsWindow();
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