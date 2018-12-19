namespace TestScaffolderExtension.Views
{
    using System.Windows;
    using TestScaffolderExtension.ViewModels;

    /// <inheritdoc cref="BaseDialogWindow" />
    /// <summary>
    /// Interaction logic for CreateUnitTestsForMethodWindow.xaml
    /// </summary>
    public partial class CreateUnitTestsForMethodWindow
    {
        private readonly CreateUnitTestsViewModel viewModel;

        public CreateUnitTestsForMethodWindow(CreateUnitTestsViewModel viewModel)
        {
            this.InitializeComponent();
            this.viewModel = viewModel;
            this.DataContext = viewModel;
        }

        private void SolutionTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var newItem = e.NewValue as SolutionItemViewModel;

            if (newItem?.CanSelect ?? false)
            {
                this.viewModel.SelectedItem = newItem;
            }
            else
            {
                this.viewModel.SelectedItem = null;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private async void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.SelectedItem.CanCreateFolder)
            {
                var createFolderViewModel = new CreateFolderViewModel();
                var createFolderWindow = new CreateFolderWindow(createFolderViewModel)
                {
                    Owner = this
                };
                var createFolderResult = createFolderWindow.ShowDialog();

                if (createFolderResult.HasValue && createFolderResult.Value)
                {
                    var newFolder = await this.viewModel.SelectedItem.CreateFolderAsync(createFolderViewModel.NewFolderName);
                    newFolder.SelectAndExpandParents();
                }
            }
        }
    }
}