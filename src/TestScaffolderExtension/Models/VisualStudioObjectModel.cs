using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using TestScaffolderExtension.Models.Solution;

namespace TestScaffolderExtension.Models
{
    public class VisualStudioObjectModel
    {
        private readonly DTE _dte;
        private DTE2 Dte2 => (DTE2)_dte;

        public VisualStudioObjectModel(DTE dte)
        {
            _dte = dte;
        }

        public async Task<SolutionModel> GetSolutionAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var solutionModel = new SolutionModel(_dte.Solution);
            await solutionModel.IterateChildrenAsync();
            return solutionModel;
        }

        public async Task<IEnumerable<object>> GetSolutionWindowSelectedItemsAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var ui = Dte2.Windows.Item(Constants.vsWindowKindSolutionExplorer).Object as UIHierarchy;
            var selectedItems = ui.SelectedItems as object[];

            var hierarchyItemObjects = new List<object>();
            foreach (var item in selectedItems.Cast<UIHierarchyItem>())
            {
                hierarchyItemObjects.Add(item.Object);
            }
            return hierarchyItemObjects;
        }
    }
}