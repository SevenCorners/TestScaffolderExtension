namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using System.Windows;
    using System.Windows.Controls;

    /// <inheritdoc cref="BaseDialogWindow" />
    /// <summary>
    /// Interaction logic for CreateUnitTestsForMethodWindow.xaml
    /// </summary>
    public partial class CreateUnitTestsForMethodWindow
    {
        private readonly CreateUnitTestsForMethodViewModel viewModel;

        public CreateUnitTestsForMethodWindow(CreateUnitTestsForMethodViewModel viewModel)
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
            this.DialogResult = this.viewModel.SelectedItem != null;
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

        private void TreeView_Expanded(object sender, RoutedEventArgs e)
        {
            var treeViewItem = e.OriginalSource as TreeViewItem;
            if (!treeViewItem.IsSelected)
            {
                return;
            }

            var windowBounds = new Rect(0, 0, this.Width, this.Height * .5);
            treeViewItem?.BringIntoView(windowBounds);
            treeViewItem?.Focus();
        }
    }
}