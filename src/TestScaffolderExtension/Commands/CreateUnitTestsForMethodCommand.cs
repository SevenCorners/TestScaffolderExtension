namespace TestScaffolderExtension.Commands
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.VisualStudio.Threading;
    using TestScaffolderExtension.Extensions;
    using TestScaffolderExtension.Models.Solution;
    using TestScaffolderExtension.Processors;
    using TestScaffolderExtension.ViewModels;
    using TestScaffolderExtension.Views;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CreateUnitTestsForMethodCommand : MenuCommandBase
    {
        private static readonly Guid CommandSetValue = new Guid("fd172596-85cf-4600-937d-4e3aa4401eb2");

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUnitTestsForMethodCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public CreateUnitTestsForMethodCommand(AsyncPackage package)
            : base(package)
        {
        }

        protected override int CommandId => 0x0100;

        protected override Guid CommandSet => CommandSetValue;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var instance = new CreateUnitTestsForMethodCommand(package);
            await instance.InitializeInternalAsync();
        }

        protected override async Task ExecuteCommandAsync(OleMenuCommand menuCommand)
        {
            var snapshotPoint = await this.GetCaretPositionAsync();

            var document = snapshotPoint
                .Snapshot
                .GetOpenDocumentInCurrentContextWithChanges();

            var root = await document.GetSyntaxRootAsync();
            var semanticModel = await document.GetSemanticModelAsync();

#pragma warning disable SA1119 // StatementMustNotUseUnnecessaryParenthesis

            // these parenthesis are necessary, issue is fixed in upcoming StyleCop release
            if (!(root.FindToken(snapshotPoint).Parent is MethodDeclarationSyntax method))
            {
                this.ShowError("Invalid Selection", "Please select a method to test.");
                return;
            }
#pragma warning restore SA1119 // StatementMustNotUseUnnecessaryParenthesis

            var unitTestCreationOptions = new UnitTestCreationOptions(method, semanticModel);

            await this.CreateTestsAsync(document.FilePath, unitTestCreationOptions);
        }

        private async Task CreateTestsAsync(string filePath, UnitTestCreationOptions unitTestCreationOptions)
        {
            var locationForTest = await this.ShowCreateUnitTestsForMethodModalWindowAsync(filePath, unitTestCreationOptions);
            if (locationForTest != null)
            {
                var unitTestFiles = await UnitTestTemplateInstantiator.InstantiateUnitTestTemplateAsync(locationForTest, unitTestCreationOptions);
                foreach (var file in unitTestFiles)
                {
                    await file.OpenAsync();
                }
            }
        }

        private async Task<SnapshotPoint> GetCaretPositionAsync()
        {
            var componentModel = await this.AsyncServiceProvider
                .GetAsAsync<SComponentModel, IComponentModel>();

            var editor = componentModel
                .GetService<IVsEditorAdaptersFactoryService>();

            var textManager = await this.AsyncServiceProvider
                .GetAsAsync<SVsTextManager, IVsTextManager>();

            textManager.GetActiveView(1, null, out var textView);

            return editor
                .GetWpfTextView(textView)
                .Caret
                .Position
                .BufferPosition;
        }

        private async Task<ProjectModelBase> ShowCreateUnitTestsForMethodModalWindowAsync(string filePath, UnitTestCreationOptions unitTestCreationOptions)
        {
            var solutionModel = await this.VisualStudio.GetSolutionAsync();

            var componentModel = await this.AsyncServiceProvider
                .GetAsAsync<SComponentModel, IComponentModel>();
            var workspace = componentModel.GetService<VisualStudioWorkspace>();
            var createUnitTestsViewModel = new CreateUnitTestsViewModel(solutionModel, unitTestCreationOptions, workspace, filePath);

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