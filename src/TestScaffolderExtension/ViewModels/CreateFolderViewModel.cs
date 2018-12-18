namespace TestScaffolderExtension.ViewModels
{
    public class CreateFolderViewModel : ViewModelBase
    {
        private string newFolderName;

        public bool CanCreateFolder => !string.IsNullOrEmpty(this.NewFolderName);

        public string NewFolderName
        {
            get => this.newFolderName;
            set
            {
                this.newFolderName = value;
                this.OnPropertyChanged(nameof(this.NewFolderName));
                this.OnPropertyChanged(nameof(this.CanCreateFolder));
            }
        }
    }
}