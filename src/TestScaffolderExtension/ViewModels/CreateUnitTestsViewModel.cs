namespace TestScaffolderExtension.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.LanguageServices;
    using TestScaffolderExtension.Models.Solution;
    using TestScaffolderExtension.Processors;

    public class CreateUnitTestsViewModel : ViewModelBase
    {
        private SolutionItemViewModel selectedLocation;

        public CreateUnitTestsViewModel()
        {
        }

        public CreateUnitTestsViewModel(
            SolutionModel solution,
            UnitTestCreationOptions unitTestCreationOptions,
            VisualStudioWorkspace workspace,
            string filePath)
        {
            this.UnitTestCreationOptions = unitTestCreationOptions;
            this.SolutionItems = new ObservableCollection<SolutionItemViewModel>();

            this.IterateProjects(solution);
            this.SetRecommendedTestFilePath(workspace, filePath);
        }

        private void SetRecommendedTestFilePath(VisualStudioWorkspace workspace, string filePath)
        {
            // search solution tree for class under test
            var documentToTest = workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
            var projectToTest = workspace.CurrentSolution.GetProject(documentToTest.ProjectId);

            var unitTestProject = this.GetUnitTestProject(workspace, projectToTest);
            var matchingProject = this.GetMatchingProjectViewModel(unitTestProject.Name);
            if (matchingProject == null)
            {
                return;
            }

            var pathAfterProject = this.GetInnerPathFromProject(projectToTest);
            if (string.IsNullOrEmpty(pathAfterProject))
            {
                matchingProject.SelectAndExpandParents();
                return;
            }

            var pathAfterProjectParts = pathAfterProject.Split('.');
            var currentIndex = 0;
            var currentItem = matchingProject;
            while (currentIndex < pathAfterProjectParts.Length)
            {
                var itemToRecommend = matchingProject.Children.FirstOrDefault(i => i.Name == pathAfterProjectParts[currentIndex]);
                if (itemToRecommend == null)
                {
                    return;
                }

                currentIndex++;
                currentItem = itemToRecommend;
            }

            currentItem.SelectAndExpandParents();
        }

        private SolutionItemViewModel GetMatchingProjectViewModel(string name)
        {
            var children = this.SolutionItems.ToList();
            while (children.Any())
            {
                var matchingProject = children.Where(c => c.Item is ProjectModel).FirstOrDefault(f => f.Name == name);
                if (matchingProject != null)
                {
                    return matchingProject;
                }
                else
                {
                    children = children.SelectMany(c => c.Children).ToList();
                }
            }

            return null;
        }

        private string GetInnerPathFromProject(Project projectToTest)
        {
            string namespaceAfterProject = string.Empty;
            var classUnderTestNamespace = this.UnitTestCreationOptions.ClassUnderTestNamespace;
            if (classUnderTestNamespace.Contains(projectToTest.Name))
            {
                namespaceAfterProject = classUnderTestNamespace.Substring(classUnderTestNamespace.IndexOf(projectToTest.Name) + projectToTest.Name.Length + 1);
            }
            else if (classUnderTestNamespace.Contains(projectToTest.AssemblyName))
            {
                namespaceAfterProject = classUnderTestNamespace.Substring(classUnderTestNamespace.IndexOf(projectToTest.AssemblyName) + projectToTest.AssemblyName.Length + 1);
            }

            return namespaceAfterProject;

        }

        private Project GetUnitTestProject(VisualStudioWorkspace workspace, Project projectToTest)
        {
            // find project with same name then .UnitTest
            // or closest match then .UnitTest
            return workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name == $"{projectToTest.Name}.UnitTest")
                ?? workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name == $"{projectToTest.AssemblyName}.UnitTest")
                ?? workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name.EndsWith(".UnitTest") && projectToTest.Name.Contains(p.Name.Substring(0, p.Name.LastIndexOf("."))))
                ?? workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name.EndsWith(".UnitTest") && projectToTest.AssemblyName.Contains(p.Name.Substring(0, p.Name.LastIndexOf("."))));
        }

        public UnitTestCreationOptions UnitTestCreationOptions { get; }

        public ObservableCollection<SolutionItemViewModel> SolutionItems { get; }

        public SolutionItemViewModel SelectedItem
        {
            get => this.selectedLocation;
            set
            {
                this.selectedLocation = value;
                this.OnPropertyChanged(nameof(this.CanCreateTests));
                this.OnPropertyChanged(nameof(this.CanCreateFolder));
            }
        }

        public bool CreateFolderCheckboxChecked
        {
            get => this.UnitTestCreationOptions?.ShouldCreateParentFolder ?? true;
            set
            {
                this.UnitTestCreationOptions.ShouldCreateParentFolder = value;
                this.OnPropertyChanged(nameof(this.CreateFolderCheckboxChecked));
            }
        }

        public bool CreateBaseClassCheckboxChecked
        {
            get => this.UnitTestCreationOptions?.ShouldCreateUnitTestBaseClass ?? true;
            set
            {
                this.UnitTestCreationOptions.ShouldCreateUnitTestBaseClass = value;
                this.OnPropertyChanged(nameof(this.CreateBaseClassCheckboxChecked));
            }
        }

        public bool CanCreateTests => this.SelectedItem?.CanSelect ?? false;

        public bool CanCreateFolder => this.SelectedItem?.CanCreateFolder ?? false;

        private void IterateProjects(SolutionModel solution)
        {
            if (solution.Children == null)
            {
                return;
            }

            foreach (var child in solution.Children.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                this.SolutionItems.Add(new SolutionItemViewModel(child));
            }
        }
    }
}