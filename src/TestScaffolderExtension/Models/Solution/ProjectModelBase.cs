using System;
using System.IO;

namespace TestScaffolderExtension.Models.Solution
{
    public abstract class ProjectModelBase : SolutionModelBase
    {
        protected ProjectModelBase(SolutionModelBase parent) : base(parent) { }

        public override bool CanAddFile => true;
        public override bool CanAddFolder => true;

        public abstract ProjectFolderModel AddFolder(string folderName);

        public FileModel AddFile(string fileName, string fileContents)
        {
            var tempFolderPath = GetTempFolderPath();
            var tempFilePath = GetTempFilePath(tempFolderPath, fileName);

            WriteTempFileContents(fileContents, tempFilePath);
            var addedFile = CopyFileFromPath(tempFilePath);

            RemoveTempFolder(tempFolderPath);

            return addedFile;
        }

        protected abstract FileModel CopyFileFromPath(string tempFilePath);

        protected static void RemoveTempFolder(string tempFolderPath)
        {
            Directory.Delete(tempFolderPath, true);
        }

        protected static void WriteTempFileContents(string fileContents, string tempFilePath)
        {
            using (var writer = new StreamWriter(tempFilePath))
            {
                writer.Write(fileContents);
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