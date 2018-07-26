using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class ProjectFolderModel : ProjectModelBase
    {
        private readonly ProjectItem _projectFolder;

        public ProjectFolderModel(SolutionModelBase parent, ProjectItem projectFolder) : base(parent)
        {
            _projectFolder = projectFolder;
        }

        protected override ModelType ItemType => ModelType.ProjectFolder;

        protected override async Task<FileModel> CopyFileFromPathAsync(string tempFilePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return new FileModel(this, _projectFolder.ProjectItems.AddFromFileCopy(tempFilePath));
        }

        public override async Task<ProjectFolderModel> AddFolderAsync(string folderName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var newFolder = new ProjectFolderModel(this, _projectFolder.ProjectItems.AddFolder(folderName));
            Children.Add(newFolder);
            return newFolder;
        }

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Name = _projectFolder.Name;

            if(_projectFolder.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach(ProjectItem child in _projectFolder.ProjectItems)
                {
                    var childItem = await SolutionModelFactory.BuildHierarchyTreeDownAsync(this, child);
                    if (childItem != null)
                    {
                        Children.Add(childItem);
                    }
                }
            }
        }
    }
}