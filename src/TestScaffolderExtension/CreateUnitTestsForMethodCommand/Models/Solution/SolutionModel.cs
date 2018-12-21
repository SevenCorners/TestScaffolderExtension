namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution
{
    using System;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;
    using Task = System.Threading.Tasks.Task;

    public sealed class SolutionModel : SolutionModelBase
    {
        private readonly Solution solution;

        public SolutionModel(Solution solution)
            : base(null)
        {
            this.solution = solution;
        }

        protected override ModelType ItemType => ModelType.Solution;

        public override async Task IterateChildrenAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.Name = this.GetName(this.solution.FullName);

            if (this.solution.Projects?.GetEnumerator().MoveNext() ?? false)
            {
                foreach (Project child in this.solution.Projects)
                {
                    var childItem = await SolutionModelFactory.BuildHierarchyTreeDownAsync(this, child);
                    if (childItem != null)
                    {
                        this.Children.Add(childItem);
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