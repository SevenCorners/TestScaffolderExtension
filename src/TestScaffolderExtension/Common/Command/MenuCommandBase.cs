namespace TestScaffolderExtension.Common.Command
{
    using System;
    using System.ComponentModel.Design;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using TestScaffolderExtension.Common.Extensions;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models;
    using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
    using Task = System.Threading.Tasks.Task;

    internal abstract class MenuCommandBase
    {
        protected MenuCommandBase(AsyncPackage asyncPackage)
        {
            this.AsyncServiceProvider = asyncPackage ?? throw new ArgumentNullException(nameof(asyncPackage));
            this.ServiceProvider = asyncPackage as IServiceProvider ?? throw new ArgumentException($"Unable to cast {nameof(asyncPackage)} as {nameof(IServiceProvider)}.");
        }

        protected IAsyncServiceProvider AsyncServiceProvider { get; }

        protected abstract int CommandId { get; }

        protected abstract Guid CommandSet { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected VisualStudioObjectModel VisualStudio { get; private set; }

        protected async Task AddCommandToMenuAsync()
        {
            var commandService = await this.AsyncServiceProvider.GetAsAsync<IMenuCommandService, OleMenuCommandService>();
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(this.CommandSet, this.CommandId);
            var menuCommand = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuCommand);
        }

        protected abstract Task ExecuteCommandAsync(OleMenuCommand menuCommand);

        protected async Task InitializeInternalAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            await this.AddCommandToMenuAsync();
            this.VisualStudio = new VisualStudioObjectModel(await this.AsyncServiceProvider.GetAsync<DTE>());
        }

        protected void ShowError(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                   this.ServiceProvider,
                   message,
                   title,
                   OLEMSGICON.OLEMSGICON_WARNING,
                   OLEMSGBUTTON.OLEMSGBUTTON_OK,
                   OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
#pragma warning disable SA1119 // StatementMustNotUseUnnecessaryParenthesis
            // these parenthesis are necessary, issue is fixed in upcoming StyleCop release
            if (!(sender is OleMenuCommand menuCommand))
            {
                return;
            }
#pragma warning restore SA1119 // StatementMustNotUseUnnecessaryParenthesis

            try
            {
                await this.ExecuteCommandAsync(menuCommand);
            }
            catch (Exception ex)
            {
                this.ShowError("Something Went Wrong", ex.Message);
            }
        }
    }
}