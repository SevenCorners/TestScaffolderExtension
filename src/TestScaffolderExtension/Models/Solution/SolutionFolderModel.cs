using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class SolutionFolderModel : SolutionModelBase
    {
        private readonly Project _solutionFolder;

        public SolutionFolderModel(SolutionModelBase parent, Project solutionFolder) : base(parent)
        {
            _solutionFolder = solutionFolder;
        }

        public override string GetFullPathForNamespace()
        {
            return string.Empty;
        }

        protected override ModelType ItemType => ModelType.SolutionFolder;

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Name = _solutionFolder.Name;

            if (_solutionFolder.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (ProjectItem item in _solutionFolder.ProjectItems)
                {
                    var childItem = await SolutionModelFactory.BuildHierarchyTreeDownAsync(this, item);
                    if (childItem != null)
                    {
                        Children.Add(childItem);
                    }
                }
            }
        }
    }
}