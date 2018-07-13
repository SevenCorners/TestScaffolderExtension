using EnvDTE;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class ProjectModel : ProjectModelBase
    {
        private readonly Project _project;

        public ProjectModel(SolutionModelBase parent, Project project) : base(parent)
        {
            _project = project;
        }

        public override string Name => _project.Name;
        protected override ModelType ItemType => ModelType.Project;

        protected override FileModel CopyFileFromPath(string tempFilePath)
        {
            return new FileModel(this, _project.ProjectItems.AddFromFileCopy(tempFilePath));
        }

        public override ProjectFolderModel AddFolder(string folderName)
        {
            return new ProjectFolderModel(this, _project.ProjectItems.AddFolder(folderName));
        }

        public override void IterateChildren()
        {
            if (_project.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach(ProjectItem child in _project.ProjectItems)
                {
                    var childItem = SolutionModelFactory.BuildHierarchyTreeDown(this, child);
                    if (childItem != null)
                    {
                        Children.Add(childItem);
                    }
                }
            }
        }
    }
}