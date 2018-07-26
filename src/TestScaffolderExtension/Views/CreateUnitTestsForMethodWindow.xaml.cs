using System.Windows;
using TestScaffolderExtension.ViewModels;

namespace TestScaffolderExtension.Views
{
    /// <inheritdoc cref="BaseDialogWindow" />
    /// <summary>
    /// Interaction logic for CreateUnitTestsForMethodWindow.xaml
    /// </summary>
    public partial class CreateUnitTestsForMethodWindow
    {
        private readonly CreateUnitTestsViewModel _viewModel;

        public CreateUnitTestsForMethodWindow(CreateUnitTestsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = viewModel;
        }

        private void SolutionTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var newItem = e.NewValue as SolutionItemViewModel;

            if (newItem?.CanSelect ?? false)
            {
                _viewModel.SelectedItem = newItem;
            }
            else
            {
                _viewModel.SelectedItem = null;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private async void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedItem.CanCreateFolder)
            {
                var createFolderViewModel = new CreateFolderViewModel();
                var createFolderWindow = new CreateFolderWindow(createFolderViewModel)
                {
                    Owner = this
                };
                var createFolderResult = createFolderWindow.ShowDialog();

                if (createFolderResult.HasValue && createFolderResult.Value)
                {
                    var newFolder = await _viewModel.SelectedItem.CreateFolderAsync(createFolderViewModel.NewFolderName);
                    newFolder.IsSelected = true;
                }
            }
        }

        //private void TreeView_Expanded(object sender, RoutedEventArgs e)
        //{
        //    var treeViewItem = e.OriginalSource as TreeViewItem;
        //    var treeView = sender as TreeView;
        //    treeViewItem.BringIntoView(new Rect(0, 0, treeView.ActualWidth, treeView.ActualHeight/2));
        //}
    }
}