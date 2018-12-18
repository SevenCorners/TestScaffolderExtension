namespace TestScaffolderExtension.Models.Solution
{
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Task = System.Threading.Tasks.Task;

    public sealed class SolutionFolderModel : SolutionModelBase
    {
        private readonly Project solutionFolder;

        public SolutionFolderModel(SolutionModelBase parent, Project solutionFolder)
            : base(parent)
        {
            this.solutionFolder = solutionFolder;
        }

        protected override ModelType ItemType => ModelType.SolutionFolder;

        public override string GetFullPathForNamespace()
        {
            return string.Empty;
        }

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.Name = this.solutionFolder.Name;

            if (this.solutionFolder.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (ProjectItem item in this.solutionFolder.ProjectItems)
                {
                    var childItem = await SolutionModelFactory.BuildHierarchyTreeDownAsync(this, item);
                    if (childItem != null)
                    {
                        this.Children.Add(childItem);
                    }
                }
            }
        }
    }
}