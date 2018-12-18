namespace TestScaffolderExtension.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using TestScaffolderExtension.Models.Solution;

    public class SolutionItemViewModel : ViewModelBase
    {
        private bool isSelected;

        public SolutionItemViewModel()
        {
        }

        public SolutionItemViewModel(SolutionModelBase item)
        {
            this.Item = item;
            this.Children = new ObservableCollection<SolutionItemViewModel>();
            foreach (var child in item.Children.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                this.Children.Add(new SolutionItemViewModel(child));
            }
        }

        public SolutionModelBase Item { get; }

        public string ItemType => this.Item.GetType().ToString();

        public string Name => this.Item.Name;

        public string DisplayName => this.Name;

        public bool CanSelect => this.Item.CanAddFile;

        public bool CanCreateFolder => this.Item.CanAddFolder;

        public ObservableCollection<SolutionItemViewModel> Children { get; }

        public bool IsSelected
        {
            get => this.CanSelect && this.isSelected;
            set
            {
                if (this.CanSelect && value != this.isSelected)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged(nameof(this.IsSelected));
                }
            }
        }

        public async Task<SolutionItemViewModel> CreateFolderAsync(string newFolderName)
        {
            if (this.CanCreateFolder && this.Item is ProjectModelBase project)
            {
                var newFolder = await project.AddFolderAsync(newFolderName);
                var newFolderViewModel = new SolutionItemViewModel(newFolder);
                this.Children.Add(newFolderViewModel);
                return newFolderViewModel;
            }

            throw new InvalidOperationException("Unable to create a folder from this object.");
        }
    }
}