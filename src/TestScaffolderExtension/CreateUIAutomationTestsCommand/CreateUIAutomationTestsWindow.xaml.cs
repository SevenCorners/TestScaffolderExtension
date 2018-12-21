namespace TestScaffolderExtension.CreateUIAutomationTestsCommand
{
    /// <summary>
    /// Interaction logic for CreateUIAutomationTestsWindow.xaml
    /// </summary>
    public partial class CreateUIAutomationTestsWindow
    {
        public CreateUIAutomationTestsWindow(CreateUIAutomationTestsViewModel createAutomationTestsViewModel)
        {
            this.InitializeComponent();
            this.DataContext = createAutomationTestsViewModel;
        }

        private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Create_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}