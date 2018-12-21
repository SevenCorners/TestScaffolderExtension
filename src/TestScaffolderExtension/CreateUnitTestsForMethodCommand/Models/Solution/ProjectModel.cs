namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution
{
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Task = System.Threading.Tasks.Task;

    public sealed class ProjectModel : ProjectModelBase
    {
        private readonly Project project;

        public ProjectModel(SolutionModelBase parent, Project project)
            : base(parent)
        {
            this.project = project;
        }

        protected override ModelType ItemType => ModelType.Project;

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.Name = this.project.Name;
            if (this.project.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (ProjectItem child in this.project.ProjectItems)
                {
                    var childItem = await SolutionModelFactory.BuildHierarchyTreeDownAsync(this, child);
                    if (childItem != null)
                    {
                        this.Children.Add(childItem);
                    }
                }
            }
        }

        protected override async Task<ProjectItem> AddFolderInternalAsync(string folderName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return this.project.ProjectItems.AddFolder(folderName);
        }

        protected override async Task<FileModel> CopyFileFromPathAsync(string tempFilePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return new FileModel(this, this.project.ProjectItems.AddFromFileCopy(tempFilePath));
        }
    }
}