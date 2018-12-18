using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Commands
{
    internal abstract class MenuCommandBase
    {
        protected abstract int CommandId { get; }
        protected abstract Guid CommandSet { get; }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static MenuCommandBase Instance { get; protected set; }

        /// <summary>
        /// Gets the asynchronous service provider from the owner package.
        /// </summary>
        protected IAsyncServiceProvider AsyncServiceProvider { get; }

        /// <summary>
        /// Gets the synchronous service provider from the owner package.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        protected MenuCommandBase(AsyncPackage asyncPackage, OleMenuCommandService commandService)
        {
            AsyncServiceProvider = asyncPackage ?? throw new ArgumentNullException(nameof(asyncPackage));
            ServiceProvider = asyncPackage as IServiceProvider ?? throw new ArgumentException($"Unable to cast {nameof(asyncPackage)} as {nameof(IServiceProvider)}.");
        }

        protected void AddCommandToMenu(OleMenuCommandService commandService)
        {
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuCommand = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuCommand);
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
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (!(sender is OleMenuCommand menuCommand)) return;

            try
            {
                await ExecuteCommandAsync(menuCommand);
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    (IServiceProvider)AsyncServiceProvider,
                    ex.Message,
                    "Not a project or project folder",
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        protected abstract Task ExecuteCommandAsync(OleMenuCommand menuCommand);
    }
}