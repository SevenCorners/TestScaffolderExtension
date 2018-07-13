using System.Windows;
using TestScaffolderExtension.ViewModels;

namespace TestScaffolderExtension.Views
{
    /// <inheritdoc cref="BaseDialogWindow" />
    /// <summary>
    /// Interaction logic for CreateFolderWindow.xaml
    /// </summary>
    public partial class CreateFolderWindow
    {
        public CreateFolderWindow(CreateFolderViewModel createFolderViewModel)
        {
            InitializeComponent();
            DataContext = createFolderViewModel;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
