namespace TestScaffolderExtension.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using TestScaffolderExtension.Models.Solution;

    public class SolutionItemViewModel : ViewModelBase
    {
        private bool isExpanded;
        private bool isSelected;

        public SolutionItemViewModel()
        {
        }

        public SolutionItemViewModel(SolutionModelBase item, SolutionItemViewModel parent = null)
        {
            this.Item = item;
            this.Parent = parent;
            this.Children = new ObservableCollection<SolutionItemViewModel>();
            foreach (var child in item.Children.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                this.Children.Add(new SolutionItemViewModel(child, this));
            }
        }

        public bool CanCreateFolder => this.Item.CanAddFolder;

        public bool CanSelect => this.Item.CanAddFile;

        public ObservableCollection<SolutionItemViewModel> Children { get; }

        public string DisplayName => this.Name;

        public bool IsExpanded
        {
            get => this.isExpanded;
            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    this.OnPropertyChanged(nameof(this.IsExpanded));
                }
            }
        }

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

        public SolutionModelBase Item { get; }

        public string ItemType => this.Item.GetType().ToString();

        public string Name => this.Item.Name;

        public SolutionItemViewModel Parent { get; }

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

        internal void SelectAndExpandParents()
        {
            this.ExpandParents();
            this.IsSelected = true;
        }

        private void ExpandParents()
        {
            if (this.Parent != null)
            {
                this.Parent.IsExpanded = true;
                this.Parent.ExpandParents();
            }
        }
    }
}