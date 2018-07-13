using EnvDTE;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows;
using TestScaffolderExtension.Extensions;
using TestScaffolderExtension.Models;
using TestScaffolderExtension.Models.Solution;
using TestScaffolderExtension.Processors;
using TestScaffolderExtension.ViewModels;
using TestScaffolderExtension.Views;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CreateUnitTestsForMethodCommand : MenuCommandBase
    {
        private static readonly Guid _commandSet = new Guid("fd172596-85cf-4600-937d-4e3aa4401eb2");

        protected override int CommandId => 0x0100;
        protected override Guid CommandSet => _commandSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUnitTestsForMethodCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public CreateUnitTestsForMethodCommand(AsyncPackage package, OleMenuCommandService commandService)
            : base(package, commandService)
        {
            AddCommandToMenu(commandService);
        }

        protected override void AddBeforeQueryStatus(OleMenuCommand menuCommand)
        {
            menuCommand.BeforeQueryStatus += MenuCommand_BeforeQueryStatus;
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in CreateUIAutomationTestsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var commandService = await package.GetAsAsync<IMenuCommandService, OleMenuCommandService>();
            Instance = new CreateUnitTestsForMethodCommand(package, commandService);
        }

        private void MenuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (!(sender is OleMenuCommand menuCommand)) return;

            menuCommand.Visible = false;
            menuCommand.Enabled = false;

            var dte = ServiceProvider.Get<DTE>();
            var currentSelection = new CurrentSelection(dte?.ActiveDocument.Selection as TextSelection);
            if (currentSelection.SelectedMethod == null)
            {
                return;
            }

            menuCommand.Visible = true;
            menuCommand.Enabled = true;
        }

        protected override async Task ExecuteCommandAsync(OleMenuCommand menuCommand)
        {
            var snapshotPoint = await GetCaretPositionAsync();

            var document = snapshotPoint
                .Snapshot
                .GetOpenDocumentInCurrentContextWithChanges();

            var root = await document.GetSyntaxRootAsync();
            var semanticModel = await document.GetSemanticModelAsync();
            var method = root.FindToken(snapshotPoint).Parent as MethodDeclarationSyntax;

            var unitTestCreationOptions = new UnitTestCreationOptions(method, semanticModel);

            await CreateTestsAsync(unitTestCreationOptions);
        }

        private async Task<SnapshotPoint> GetCaretPositionAsync()
        {
            var componentModel = await AsyncServiceProvider
                .GetAsAsync<SComponentModel, IComponentModel>();

            var editor = componentModel
                .GetService<IVsEditorAdaptersFactoryService>();

            var textManager = await AsyncServiceProvider
                .GetAsAsync<SVsTextManager, IVsTextManager>();

            textManager.GetActiveView(1, null, out var textView);

            return editor
                .GetWpfTextView(textView)
                .Caret
                .Position
                .BufferPosition;
        }

        private async Task CreateTestsAsync(UnitTestCreationOptions unitTestCreationOptions)
        {
            var locationForTest = await ShowCreateUnitTestsForMethodModalWindowAsync(unitTestCreationOptions);
            if (locationForTest != null)
            {
                var unitTestFiles = UnitTestTemplateInstantiator.InstantiateUnitTestTemplate(locationForTest, unitTestCreationOptions);
                foreach (var file in unitTestFiles)
                {
                    file.Open();
                }
            }
        }

        private async Task<ProjectModelBase> ShowCreateUnitTestsForMethodModalWindowAsync(UnitTestCreationOptions unitTestCreationOptions)
        {
            var dte = await AsyncServiceProvider.GetAsync<DTE>();

            var createUnitTestsViewModel = new CreateUnitTestsViewModel(new SolutionModel(dte.Solution), unitTestCreationOptions);

            var createUnitTestsForMethodWindow = new CreateUnitTestsForMethodWindow(createUnitTestsViewModel)
            {
                Owner = Application.Current.MainWindow
            };
            var dialogResult = createUnitTestsForMethodWindow.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                return createUnitTestsViewModel.SelectedItem.Item as ProjectModelBase;
            }

            return null;
        }
    }
}