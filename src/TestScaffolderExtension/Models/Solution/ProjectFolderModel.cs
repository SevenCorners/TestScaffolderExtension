using EnvDTE;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class ProjectFolderModel : ProjectModelBase
    {
        private readonly ProjectItem _projectFolder;

        public ProjectFolderModel(SolutionModelBase parent, ProjectItem projectFolder) : base(parent)
        {
            _projectFolder = projectFolder;
        }

        public override string Name => _projectFolder.Name;
        protected override ModelType ItemType => ModelType.ProjectFolder;

        protected override FileModel CopyFileFromPath(string tempFilePath)
        {
            return new FileModel(this, _projectFolder.ProjectItems.AddFromFileCopy(tempFilePath));
        }

        public override ProjectFolderModel AddFolder(string folderName)
        {
            var newFolder = new ProjectFolderModel(this, _projectFolder.ProjectItems.AddFolder(folderName));
            Children.Add(newFolder);
            return newFolder;
        }

        public override void IterateChildren()
        {
            if(_projectFolder.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach(ProjectItem child in _projectFolder.ProjectItems)
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