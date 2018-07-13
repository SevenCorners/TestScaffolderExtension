using System.Collections.ObjectModel;
using System.Linq;
using TestScaffolderExtension.Models.Solution;
using TestScaffolderExtension.Processors;

namespace TestScaffolderExtension.ViewModels
{
    public class CreateUnitTestsViewModel : ViewModelBase
    {
        private SolutionItemViewModel _selectedLocation;

        public CreateUnitTestsViewModel() { }

        public CreateUnitTestsViewModel(SolutionModel solution, UnitTestCreationOptions unitTestCreationOptions)
        {
            UnitTestCreationOptions = unitTestCreationOptions;
            SolutionItems = new ObservableCollection<SolutionItemViewModel>();

            IterateProjects(solution);
        }

        public UnitTestCreationOptions UnitTestCreationOptions { get; }

        public ObservableCollection<SolutionItemViewModel> SolutionItems { get; }

        public SolutionItemViewModel SelectedItem
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged(nameof(CanCreateTests));
                OnPropertyChanged(nameof(CanCreateFolder));
            }
        }

        public bool CreateFolderCheckboxChecked
        {
            get => UnitTestCreationOptions.ShouldCreateParentFolder;
            set
            {
                UnitTestCreationOptions.ShouldCreateParentFolder = value;
                OnPropertyChanged(nameof(CreateFolderCheckboxChecked));
            }
        }

        public bool CreateBaseClassCheckboxChecked
        {
            get => UnitTestCreationOptions.ShouldCreateUnitTestBaseClass;
            set
            {
                UnitTestCreationOptions.ShouldCreateUnitTestBaseClass = value;
                OnPropertyChanged(nameof(CreateBaseClassCheckboxChecked));
            }
        }

        public bool CanCreateTests => SelectedItem?.CanSelect ?? false;

        public bool CanCreateFolder => SelectedItem?.CanCreateFolder ?? false;

        private void IterateProjects(SolutionModel solution)
        {
            if (solution.Children == null) return;

            foreach (var child in solution.Children.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                SolutionItems.Add(new SolutionItemViewModel(child));
            }
        }
    }
}