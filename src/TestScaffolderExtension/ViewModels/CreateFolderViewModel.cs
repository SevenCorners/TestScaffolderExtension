namespace TestScaffolderExtension.ViewModels
{
    public class CreateFolderViewModel : ViewModelBase
    {
        private string _newFolderName;

        public bool CanCreateFolder => !string.IsNullOrEmpty(NewFolderName);

        public string NewFolderName
        {
            get => _newFolderName;
            set
            {
                _newFolderName = value;
                OnPropertyChanged(nameof(NewFolderName));
                OnPropertyChanged(nameof(CanCreateFolder));
            }
        }
    }
}