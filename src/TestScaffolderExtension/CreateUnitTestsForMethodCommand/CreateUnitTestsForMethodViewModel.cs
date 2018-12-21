namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using TestScaffolderExtension.Common.View;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution;

    public class CreateUnitTestsForMethodViewModel : ViewModelBase
    {
        private readonly MethodUnderTestAnalyzerResult analyzerResult;
        private SolutionItemViewModel selectedItem;
        private bool shouldCreateParentFolder;
        private bool shouldCreateUnitTestBaseClass;

        public CreateUnitTestsForMethodViewModel()
        {
            this.shouldCreateUnitTestBaseClass = true;
            this.shouldCreateParentFolder = true;
        }

        public CreateUnitTestsForMethodViewModel(
            SolutionModel solution,
            MethodUnderTestAnalyzerResult analyzerResult)
            : this()
        {
            this.analyzerResult = analyzerResult;

            this.CreateSolutionTreeViewModelCollection(solution);
            this.SelectRecommendedUnitTestLocation(analyzerResult);
        }

        public bool CanCreateFolder => this.SelectedItem?.CanCreateFolder ?? false;

        public bool CanCreateTests => this.SelectedItem?.CanSelect ?? false;

        public bool CreateBaseClassCheckboxChecked
        {
            get => this.shouldCreateUnitTestBaseClass;
            set
            {
                this.shouldCreateUnitTestBaseClass = value;
                this.OnPropertyChanged(nameof(this.CreateBaseClassCheckboxChecked));
            }
        }

        public string UnitTestBaseClassName => this.analyzerResult?.UnitTestCreationDetails?.UnitTestBaseClassName;

        public string UnitTestFolderName => this.analyzerResult?.UnitTestCreationDetails?.UnitTestFolderName;

        public bool CreateFolderCheckboxChecked
        {
            get => this.shouldCreateParentFolder;
            set
            {
                this.shouldCreateParentFolder = value;
                this.OnPropertyChanged(nameof(this.CreateFolderCheckboxChecked));
            }
        }

        public SolutionItemViewModel SelectedItem
        {
            get => this.selectedItem;
            set
            {
                this.selectedItem = value;
                this.OnPropertyChanged(nameof(this.CanCreateTests));
                this.OnPropertyChanged(nameof(this.CanCreateFolder));
            }
        }

        public ObservableCollection<SolutionItemViewModel> SolutionItems { get; } = new ObservableCollection<SolutionItemViewModel>();

        public UnitTestCreationOptions UnitTestCreationOptions()
        {
            return new UnitTestCreationOptions(
                this.SelectedItem.Item as ProjectModelBase,
                this.analyzerResult.UnitTestCreationDetails,
                this.shouldCreateParentFolder,
                this.shouldCreateUnitTestBaseClass);
        }

        private void CreateSolutionTreeViewModelCollection(SolutionModel solution)
        {
            if (solution.Children == null)
            {
                return;
            }

            foreach (var child in solution.Children.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                this.SolutionItems.Add(new SolutionItemViewModel(child, null));
            }
        }

        private SolutionItemViewModel GetMatchingProjectViewModel(string recommendedProjectName)
        {
            var solutionItemsCurrentLevel = this.SolutionItems.ToList();
            while (solutionItemsCurrentLevel.Any())
            {
                var matchingProject = solutionItemsCurrentLevel.Where(c => c.Item is ProjectModel).FirstOrDefault(f => f.Name == recommendedProjectName);
                if (matchingProject != null)
                {
                    return matchingProject;
                }
                else
                {
                    solutionItemsCurrentLevel = solutionItemsCurrentLevel.SelectMany(c => c.Children).ToList();
                }
            }

            return null;
        }

        private void SelectRecommendedUnitTestLocation(MethodUnderTestAnalyzerResult analyzerResult)
        {
            if (string.IsNullOrEmpty(analyzerResult.RecommendedUnitTestLocationInfo.RecommendedUnitTestProjectName))
            {
                return;
            }

            var matchingProject = this.GetMatchingProjectViewModel(analyzerResult.RecommendedUnitTestLocationInfo.RecommendedUnitTestProjectName);
            if (matchingProject == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(analyzerResult.RecommendedUnitTestLocationInfo.RecommendedUnitTestPathFromProject))
            {
                matchingProject.SelectAndExpandParents();
                return;
            }

            var pathFromProject = analyzerResult.RecommendedUnitTestLocationInfo.RecommendedUnitTestPathFromProject.Split('.');
            var currentIndex = 0;
            var currentItem = matchingProject;
            while (currentIndex < pathFromProject.Length)
            {
                var itemToRecommend = currentItem.Children.FirstOrDefault(i => i.Name == pathFromProject[currentIndex]);
                if (itemToRecommend == null)
                {
                    break;
                }

                currentIndex++;
                currentItem = itemToRecommend;
            }

            currentItem.SelectAndExpandParents();
        }
    }
}