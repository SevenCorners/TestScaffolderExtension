namespace TestScaffolderExtension.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using TestScaffolderExtension.Models.Solution;
    using TestScaffolderExtension.Processors;

    public class CreateUnitTestsViewModel : ViewModelBase
    {
        private SolutionItemViewModel selectedLocation;

        public CreateUnitTestsViewModel()
        {
        }

        public CreateUnitTestsViewModel(SolutionModel solution, UnitTestCreationOptions unitTestCreationOptions)
        {
            this.UnitTestCreationOptions = unitTestCreationOptions;
            this.SolutionItems = new ObservableCollection<SolutionItemViewModel>();

            this.IterateProjects(solution);
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
            get => this.UnitTestCreationOptions.ShouldCreateParentFolder;
            set
            {
                this.UnitTestCreationOptions.ShouldCreateParentFolder = value;
                this.OnPropertyChanged(nameof(this.CreateFolderCheckboxChecked));
            }
        }

        public bool CreateBaseClassCheckboxChecked
        {
            get => this.UnitTestCreationOptions.ShouldCreateUnitTestBaseClass;
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