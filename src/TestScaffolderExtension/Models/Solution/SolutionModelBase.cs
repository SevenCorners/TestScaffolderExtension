using System.Collections.Generic;

namespace TestScaffolderExtension.Models.Solution
{
    public abstract class SolutionModelBase
    {
        protected SolutionModelBase(SolutionModelBase parent)
        {
            _parent = parent;
            Children = new List<SolutionModelBase>();
        }

        public virtual bool CanAddFile => false;
        public virtual bool CanAddFolder => false;
        public int SortOrder => (int)ItemType;
        public virtual string GetFullPathForNamespace()
        {
            if (_parent == null)
            {
                return Name;
            }

            return $"{_parent.GetFullPathForNamespace()}.{Name}";
        }

        public abstract string Name { get; }
        protected abstract ModelType ItemType { get; }

        private readonly SolutionModelBase _parent;

        public IList<SolutionModelBase> Children { get; protected set; }

        public abstract void IterateChildren();

        protected enum ModelType
        {
            Solution,
            SolutionFolder,
            ProjectFolder,
            Project,
            File
        }
    }
}