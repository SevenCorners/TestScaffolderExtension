namespace TestScaffolderExtension.Models.Solution
{
    using System.Threading.Tasks;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Task = System.Threading.Tasks.Task;

    public sealed class ProjectFolderModel : ProjectModelBase
    {
        private readonly ProjectItem projectFolder;

        public ProjectFolderModel(SolutionModelBase parent, ProjectItem projectFolder)
            : base(parent)
        {
            this.projectFolder = projectFolder;
        }

        protected override ModelType ItemType => ModelType.ProjectFolder;

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.Name = this.projectFolder.Name;

            if (this.projectFolder.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (ProjectItem child in this.projectFolder.ProjectItems)
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
            return this.projectFolder.ProjectItems.AddFolder(folderName);
        }

        protected override async Task<FileModel> CopyFileFromPathAsync(string tempFilePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return new FileModel(this, this.projectFolder.ProjectItems.AddFromFileCopy(tempFilePath));
        }
    }
}