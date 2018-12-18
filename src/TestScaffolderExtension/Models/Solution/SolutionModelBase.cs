namespace TestScaffolderExtension.Models.Solution
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class SolutionModelBase
    {
        private readonly SolutionModelBase parent;

        protected SolutionModelBase(SolutionModelBase parent)
        {
            this.parent = parent;
            this.Children = new List<SolutionModelBase>();
        }

        protected enum ModelType
        {
            Solution,
            SolutionFolder,
            ProjectFolder,
            Project,
            File
        }

        public virtual bool CanAddFile => false;

        public virtual bool CanAddFolder => false;

        public IList<SolutionModelBase> Children { get; protected set; }

        public string Name { get; protected set; }

        public int SortOrder => (int)this.ItemType;

        protected abstract ModelType ItemType { get; }

        public virtual string GetFullPathForNamespace()
        {
            if (this.parent == null)
            {
                return this.Name;
            }

            return $"{this.parent.GetFullPathForNamespace()}.{this.Name}";
        }

        public abstract Task IterateChildrenAsync();
    }
}