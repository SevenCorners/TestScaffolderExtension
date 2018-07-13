using EnvDTE;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class SolutionFolderModel : SolutionModelBase
    {
        private readonly Project _solutionFolder;

        public SolutionFolderModel(SolutionModelBase parent, Project solutionFolder) : base(parent)
        {
            _solutionFolder = solutionFolder;
        }

        public override string Name => _solutionFolder.Name;
        public override string GetFullPathForNamespace()
        {
            return string.Empty;
        }

        protected override ModelType ItemType => ModelType.SolutionFolder;

        public override void IterateChildren()
        {
            if (_solutionFolder.ProjectItems?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (ProjectItem item in _solutionFolder.ProjectItems)
                {
                    var childItem = SolutionModelFactory.BuildHierarchyTreeDown(this, item);
                    if (childItem != null)
                    {
                        Children.Add(childItem);
                    }
                }
            }
        }
    }
}