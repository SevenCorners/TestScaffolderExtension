namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using EnvDTE;

    public abstract class ProjectModelBase : SolutionModelBase
    {
        protected ProjectModelBase(SolutionModelBase parent)
            : base(parent)
        {
        }

        public override bool CanAddFile => true;

        public override bool CanAddFolder => true;

        public async Task<FileModel> AddFileAsync(string fileName, string fileContents)
        {
            var tempFolderPath = GetTempFolderPath();
            var tempFilePath = GetTempFilePath(tempFolderPath, fileName);

            await WriteTempFileContentsAsync(fileContents, tempFilePath);
            var addedFile = await this.CopyFileFromPathAsync(tempFilePath);

            RemoveTempFolder(tempFolderPath);

            return addedFile;
        }

        public async Task<ProjectFolderModel> AddFolderAsync(string folderName)
        {
            var newFolder = new ProjectFolderModel(this, await this.AddFolderInternalAsync(folderName));
            await newFolder.IterateChildrenAsync();
            this.Children.Add(newFolder);
            return newFolder;
        }

        protected static string GetTempFilePath(string tempFolderPath, string fileName)
        {
            return Path.Combine(tempFolderPath, fileName);
        }

        protected static string GetTempFolderPath()
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())).FullName;
        }

        protected static void RemoveTempFolder(string tempFolderPath)
        {
            Directory.Delete(tempFolderPath, true);
        }

        protected static async Task WriteTempFileContentsAsync(string fileContents, string tempFilePath)
        {
            using (var writer = new StreamWriter(tempFilePath))
            {
                await writer.WriteAsync(fileContents);
            }
        }

        protected abstract Task<ProjectItem> AddFolderInternalAsync(string folderName);

        protected abstract Task<FileModel> CopyFileFromPathAsync(string tempFilePath);
    }
}