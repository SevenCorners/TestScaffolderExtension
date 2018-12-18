namespace TestScaffolderExtension.Views
{
    using System.Windows;
    using TestScaffolderExtension.ViewModels;

    /// <inheritdoc cref="BaseDialogWindow" />
    /// <summary>
    /// Interaction logic for CreateFolderWindow.xaml
    /// </summary>
    public partial class CreateFolderWindow
    {
        public CreateFolderWindow(CreateFolderViewModel createFolderViewModel)
        {
            this.InitializeComponent();
            this.DataContext = createFolderViewModel;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
