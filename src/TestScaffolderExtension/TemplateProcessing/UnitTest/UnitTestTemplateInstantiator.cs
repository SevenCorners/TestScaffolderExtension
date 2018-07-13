using System.Collections.Generic;
using TestScaffolderExtension.Models.Solution;
using TestBaseClassTemplate = TestScaffolderExtension.Templates.UnitTest.TestBaseClassTemplate;
using TestClassTemplate = TestScaffolderExtension.Templates.UnitTest.TestClassTemplate;

namespace TestScaffolderExtension.Processors
{
    internal static class UnitTestTemplateInstantiator
    {
        internal static IEnumerable<FileModel> InstantiateUnitTestTemplate(ProjectModelBase locationForTest, UnitTestCreationOptions unitTestCreationOptions)
        {
            var unitTestProjectLocation = GetUnitTestProjectLocation(locationForTest, unitTestCreationOptions);

            if (unitTestCreationOptions.ShouldCreateUnitTestBaseClass)
            {
                yield return AddUnitTestBaseClass(unitTestProjectLocation, unitTestCreationOptions);
            }

            yield return AddUnitTestClass(unitTestProjectLocation, unitTestCreationOptions);
        }

        private static ProjectModelBase GetUnitTestProjectLocation(ProjectModelBase locationForTest, UnitTestCreationOptions creationOptions)
        {
            if (creationOptions.ShouldCreateParentFolder)
            {
                return locationForTest.AddFolder(creationOptions.UnitTestFolderName);
            }
            return locationForTest;
        }

        private static FileModel AddUnitTestBaseClass(ProjectModelBase unitTestProjectLocation, UnitTestCreationOptions creationOptions)
        {
            var unitTestBaseClass = new TestBaseClassTemplate(unitTestProjectLocation, creationOptions);
            return unitTestProjectLocation.AddFile(creationOptions.UnitTestBaseClassFileName, unitTestBaseClass.TransformText());
        }

        private static FileModel AddUnitTestClass(ProjectModelBase unitTestProjectLocation, UnitTestCreationOptions unitTestCreationOptions)
        {
            var unitTestClass = new TestClassTemplate(unitTestProjectLocation, unitTestCreationOptions);
            return unitTestProjectLocation.AddFile(unitTestCreationOptions.UnitTestClassFileName, unitTestClass.TransformText());
        }
    }
}