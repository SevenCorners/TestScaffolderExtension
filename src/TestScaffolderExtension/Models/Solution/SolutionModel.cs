using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using Task = System.Threading.Tasks.Task;

namespace TestScaffolderExtension.Models.Solution
{
    public sealed class SolutionModel : SolutionModelBase
    {
        private readonly EnvDTE.Solution _solution;

        public SolutionModel(EnvDTE.Solution solution) : base(null)
        {
            _solution = solution;
        }

        protected override ModelType ItemType => ModelType.Solution;

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Name = GetName(_solution.FullName);

            if (_solution.Projects?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (Project child in _solution.Projects)
                {
                    var childItem = await SolutionModelFactory.BuildHierarchyTreeDownAsync(this, child);
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