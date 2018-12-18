using System.Collections.Generic;
using System.Threading.Tasks;
using TestScaffolderExtension.Models.Solution;
using TestBaseClassTemplate = TestScaffolderExtension.Templates.UnitTest.TestBaseClassTemplate;
using TestClassTemplate = TestScaffolderExtension.Templates.UnitTest.TestClassTemplate;

namespace TestScaffolderExtension.Processors
{
    internal static class UnitTestTemplateInstantiator
    {
        internal static async Task<IEnumerable<FileModel>> InstantiateUnitTestTemplateAsync(ProjectModelBase locationForTest, UnitTestCreationOptions unitTestCreationOptions)
        {
            var unitTestProjectLocation = await GetUnitTestProjectLocationAsync(locationForTest, unitTestCreationOptions);

            var instantiationTasks = new List<Task<FileModel>>();
            if (unitTestCreationOptions.ShouldCreateUnitTestBaseClass)
            {
                instantiationTasks.Add(AddUnitTestBaseClassAsync(unitTestProjectLocation, unitTestCreationOptions));
            }

            instantiationTasks.Add(AddUnitTestClassAsync(unitTestProjectLocation, unitTestCreationOptions));

            return await Task.WhenAll(instantiationTasks);
        }

        private static async Task<ProjectModelBase> GetUnitTestProjectLocationAsync(ProjectModelBase locationForTest, UnitTestCreationOptions creationOptions)
        {
            if (creationOptions.ShouldCreateParentFolder)
            {
                return await locationForTest.AddFolderAsync(creationOptions.UnitTestFolderName);
            }
            return locationForTest;
        }

        private static async Task<FileModel> AddUnitTestBaseClassAsync(ProjectModelBase unitTestProjectLocation, UnitTestCreationOptions creationOptions)
        {
            var unitTestBaseClass = new TestBaseClassTemplate(unitTestProjectLocation, creationOptions);
            return await unitTestProjectLocation.AddFileAsync(creationOptions.UnitTestBaseClassFileName, unitTestBaseClass.TransformText());
        }

        private static async Task<FileModel> AddUnitTestClassAsync(ProjectModelBase unitTestProjectLocation, UnitTestCreationOptions unitTestCreationOptions)
        {
            var unitTestClass = new TestClassTemplate(unitTestProjectLocation, unitTestCreationOptions);
            return await unitTestProjectLocation.AddFileAsync(unitTestCreationOptions.UnitTestClassFileName, unitTestClass.TransformText());
        }
    }
}