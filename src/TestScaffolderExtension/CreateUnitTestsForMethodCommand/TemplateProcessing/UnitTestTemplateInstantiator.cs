namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand.TemplateProcessing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution;
    using TestBaseClassTemplate = Templates.TestBaseClassTemplate;
    using TestClassTemplate = Templates.TestClassTemplate;

    internal static class UnitTestTemplateInstantiator
    {
        internal static async Task<IEnumerable<FileModel>> InstantiateUnitTestTemplateAsync(UnitTestCreationOptions unitTestCreationOptions)
        {
            var unitTestProjectLocation = await GetUnitTestProjectLocationAsync(
                unitTestCreationOptions.LocationForTest,
                unitTestCreationOptions.UnitTestCreationDetails,
                unitTestCreationOptions.ShouldCreateParentFolder);

            var instantiationTasks = new List<Task<FileModel>>();
            if (unitTestCreationOptions.ShouldCreateUnitTestBaseClass)
            {
                instantiationTasks.Add(AddUnitTestBaseClassAsync(unitTestProjectLocation, unitTestCreationOptions.UnitTestCreationDetails));
            }

            instantiationTasks.Add(AddUnitTestClassAsync(unitTestProjectLocation, unitTestCreationOptions.UnitTestCreationDetails));

            return await Task.WhenAll(instantiationTasks);
        }

        private static async Task<FileModel> AddUnitTestBaseClassAsync(ProjectModelBase unitTestProjectLocation, UnitTestCreationDetails creationOptions)
        {
            var unitTestBaseClass = new TestBaseClassTemplate(unitTestProjectLocation, creationOptions);
            return await unitTestProjectLocation.AddFileAsync(creationOptions.UnitTestBaseClassFileName, unitTestBaseClass.TransformText());
        }

        private static async Task<FileModel> AddUnitTestClassAsync(ProjectModelBase unitTestProjectLocation, UnitTestCreationDetails unitTestCreationOptions)
        {
            var unitTestClass = new TestClassTemplate(unitTestProjectLocation, unitTestCreationOptions);
            return await unitTestProjectLocation.AddFileAsync(unitTestCreationOptions.UnitTestClassFileName, unitTestClass.TransformText());
        }

        private static async Task<ProjectModelBase> GetUnitTestProjectLocationAsync(ProjectModelBase locationForTest, UnitTestCreationDetails creationOptions, bool shouldCreateParentFolder)
        {
            if (shouldCreateParentFolder)
            {
                return await locationForTest.AddFolderAsync(creationOptions.UnitTestFolderName);
            }

            return locationForTest;
        }
    }
}