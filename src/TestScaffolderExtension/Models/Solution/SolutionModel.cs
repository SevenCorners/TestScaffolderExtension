using System;
using EnvDTE;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class SolutionModel : SolutionModelBase
    {
        private readonly EnvDTE.Solution _solution;

        public SolutionModel(EnvDTE.Solution solution) : base(null)
        {
            _solution = solution;
            Name = GetName(solution.FullName);
            IterateChildren();
        }

        public override string Name { get; }
        protected override ModelType ItemType => ModelType.Solution;


        public override void IterateChildren()
        {
            if (_solution.Projects?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (Project child in _solution.Projects)
                {
                    var childItem = SolutionModelFactory.BuildHierarchyTreeDown(this, child);
                    if (childItem != null)
                    {
                        Children.Add(childItem);
                    }
                }
            }
        }

        public override string GetFullPathForNamespace()
        {
            return string.Empty;
        }

        private string GetName(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf(@"\", StringComparison.Ordinal) + 1, fullName.LastIndexOf(".", StringComparison.Ordinal) - fullName.LastIndexOf(@"\", StringComparison.Ordinal) - 1);
        }
    }
}