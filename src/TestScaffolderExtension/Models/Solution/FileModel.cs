namespace TestScaffolderExtension.Models.Solution
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Task = System.Threading.Tasks.Task;

    public class FileModel : SolutionModelBase
    {
        private readonly ProjectItem file;

        public FileModel(SolutionModelBase parent, ProjectItem file)
            : base(parent)
        {
            this.file = file;
        }

        protected override ModelType ItemType => ModelType.File;

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.Name = this.file.Name;
        }

        internal async Task OpenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (this.file.IsOpen)
            {
                return;
            }

            var window = this.file.Open();
            window.Activate();
        }
    }
}