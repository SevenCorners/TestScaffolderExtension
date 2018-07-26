using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Models.Solution
{
    public class FileModel : SolutionModelBase
    {
        private readonly ProjectItem _file;

        public FileModel(SolutionModelBase parent, ProjectItem file) : base(parent)
        {
            _file = file;
        }

        protected override ModelType ItemType => ModelType.File;

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Name = _file.Name;
        }

        internal async Task OpenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (_file.IsOpen)
            {
                return;
            }

            var window = _file.Open();
            window.Activate();
        }
    }
}