using EnvDTE;

namespace TestScaffolderExtension.Models.Solution
{
    public class FileModel : SolutionModelBase
    {
        private readonly ProjectItem _file;

        public FileModel(SolutionModelBase parent, ProjectItem file) : base(parent)
        {
            _file = file;
        }

        public override string Name => _file.Name;
        protected override ModelType ItemType => ModelType.File;

        public override void IterateChildren()
        {
        }

        internal void Open()
        {
            if (_file.IsOpen)
            {
                return;
            }

            var window = _file.Open();
            window.Activate();
        }
    }
}