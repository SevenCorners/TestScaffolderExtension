using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class ProjectModel : ProjectModelBase
    {
        private readonly Project _project;

        public ProjectModel(SolutionModelBase parent, Project project) : base(parent)
        {
            _project = project;
        }

        protected override ModelType ItemType => ModelType.Project;

        protected override async Task<FileModel> CopyFileFromPathAsync(string tempFilePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return new FileModel(this, _project.ProjectItems.AddFromFileCopy(tempFilePath));
        }

        protected override async Task<ProjectItem> AddFolderInternalAsync(string folderName)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return _project.ProjectItems.AddFolder(folderName);
        }

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Name = _project.Name;
            if (_project.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach(ProjectItem child in _project.ProjectItems)
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