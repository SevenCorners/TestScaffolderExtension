using System;
using System.Collections.ObjectModel;
using System.Linq;
using TestScaffolderExtension.Models.Solution;

namespace TestScaffolderExtension.ViewModels
{
    public class SolutionItemViewModel : ViewModelBase
    {
        public SolutionItemViewModel() { }

        public SolutionItemViewModel(SolutionModelBase item)
        {
            Item = item;
            Children = new ObservableCollection<SolutionItemViewModel>();
            foreach (var child in item.Children.OrderBy(c => c.SortOrder).ThenBy(c => c.Name))
            {
                Children.Add(new SolutionItemViewModel(child));
            }
        }

        public SolutionModelBase Item { get; }
        public string ItemType => Item.GetType().ToString();
        public string Name => Item.Name;
        public string DisplayName => Name;
        public bool CanSelect => Item.CanAddFile;
        public bool CanCreateFolder => Item.CanAddFolder;

        public ObservableCollection<SolutionItemViewModel> Children { get; }

        public bool IsSelected
        {
            get => CanSelect && _isSelected;
            set
            {
                if (CanSelect && value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        
        private bool _isSelected;

        public SolutionItemViewModel CreateFolder(string newFolderName)
        {
            if (CanCreateFolder && Item is ProjectModelBase project)
            {
                var newFolder = new SolutionItemViewModel(project.AddFolder(newFolderName));
                Children.Add(newFolder);
                return newFolder;
            }

            throw new InvalidOperationException("Unable to create a folder from this object.");
        }
    }
}