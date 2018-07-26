using System;
using System.IO;
using System.Threading.Tasks;

namespace TestScaffolderExtension.Models.Solution
{
    public abstract class ProjectModelBase : SolutionModelBase
    {
        protected ProjectModelBase(SolutionModelBase parent) : base(parent) { }

        public override bool CanAddFile => true;
        public override bool CanAddFolder => true;

        public abstract Task<ProjectFolderModel> AddFolderAsync(string folderName);

        public async Task<FileModel> AddFileAsync(string fileName, string fileContents)
        {
            var tempFolderPath = GetTempFolderPath();
            var tempFilePath = GetTempFilePath(tempFolderPath, fileName);

            await WriteTempFileContentsAsync(fileContents, tempFilePath);
            var addedFile = await CopyFileFromPathAsync(tempFilePath);

            RemoveTempFolder(tempFolderPath);

            return addedFile;
        }

        protected abstract Task<FileModel> CopyFileFromPathAsync(string tempFilePath);

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

        protected static string GetTempFolderPath()
        {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())).FullName;
        }

        protected static string GetTempFilePath(string tempFolderPath, string fileName)
        {
            return Path.Combine(tempFolderPath, fileName);
        }
    }
}