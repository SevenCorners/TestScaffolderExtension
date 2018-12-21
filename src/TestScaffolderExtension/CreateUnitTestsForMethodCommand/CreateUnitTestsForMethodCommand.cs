namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using Microsoft.VisualStudio.Shell;
    using TestScaffolderExtension.Common.Command;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.TemplateProcessing;
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
            try
            {
                var analyzer = new MethodUnderTestAnalyzer(this.AsyncServiceProvider);
                var analyzerResult = await analyzer.AnalyzeAsync();
                await this.CreateTestsAsync(analyzerResult);
            }
            catch (CommandException e)
            {
                this.ShowError(e.Title, e.Message);
            }
        }

        private async Task CreateTestsAsync(MethodUnderTestAnalyzerResult analyzerResult)
        {
            var creationOptions = await this.ShowCreateUnitTestsForMethodModalWindowAsync(analyzerResult);
            if (creationOptions != null)
            {
                var unitTestFiles = await UnitTestTemplateInstantiator.InstantiateUnitTestTemplateAsync(creationOptions);
                foreach (var file in unitTestFiles)
                {
                    await file.OpenAsync();
                }
            }
        }

        private async Task<UnitTestCreationOptions> ShowCreateUnitTestsForMethodModalWindowAsync(MethodUnderTestAnalyzerResult analyzerResult)
        {
            var solutionModel = await this.VisualStudio.GetSolutionAsync();

            var createUnitTestsViewModel = new CreateUnitTestsForMethodViewModel(solutionModel, analyzerResult);

            var createUnitTestsForMethodWindow = new CreateUnitTestsForMethodWindow(createUnitTestsViewModel)
            {
                Owner = Application.Current.MainWindow
            };
            var dialogResult = createUnitTestsForMethodWindow.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                return createUnitTestsViewModel.UnitTestCreationOptions();
            }

            return null;
        }
    }
}