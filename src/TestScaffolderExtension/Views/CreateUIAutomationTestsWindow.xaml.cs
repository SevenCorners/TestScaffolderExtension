using TestScaffolderExtension.ViewModels;

namespace TestScaffolderExtension.Views
{
    /// <summary>
    /// Interaction logic for CreateUIAutomationTestsWindow.xaml
    /// </summary>
    public partial class CreateUIAutomationTestsWindow
    {
        public CreateUIAutomationTestsWindow(CreateUIAutomationTestsViewModel createAutomationTestsViewModel)
        {
            InitializeComponent();
            DataContext = createAutomationTestsViewModel;
        }

        private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Create_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}