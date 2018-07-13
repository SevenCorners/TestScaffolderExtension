using System.Collections.Generic;
using TestScaffolderExtension.Models.Solution;
using TestScaffolderExtension.Templates.UIAutomationTest;

namespace TestScaffolderExtension.Processors.UIAutomationTest
{
    internal static class UIAutomationTestTemplateInstantiator
    {
        internal static IEnumerable<FileModel> Instantiate(ProjectModelBase selectedProjectNode, UIAutomationTestCreationOptions automationTestOptions)
        {
            var testFolder = AddAutomationTestFolder(selectedProjectNode, automationTestOptions);
            yield return AddAutomationTestClass(testFolder, automationTestOptions);
            yield return AddAutomationPageClass(testFolder, automationTestOptions);
            yield return AddAutomationPageElementMapClass(testFolder, automationTestOptions);
            yield return AddAutomationPageValidatorClass(testFolder, automationTestOptions);
        }

        private static FileModel AddAutomationPageValidatorClass(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new PageValidatorTemplate(testFolder, automationTestOptions);
            return testFolder.AddFile($"{automationTestOptions.PageValidatorClassName}.cs", unitTestClass.TransformText());
        }

        private static FileModel AddAutomationPageElementMapClass(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new PageElementMapTemplate(testFolder, automationTestOptions);
            return testFolder.AddFile($"{automationTestOptions.PageElementMapClassName}.cs", unitTestClass.TransformText());
        }

        private static FileModel AddAutomationPageClass(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new PageTemplate(testFolder, automationTestOptions);
            return testFolder.AddFile($"{automationTestOptions.PageClassName}.cs", unitTestClass.TransformText());
        }

        private static FileModel AddAutomationTestClass(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new TestTemplate(testFolder, automationTestOptions);
            return testFolder.AddFile($"{automationTestOptions.TestClassName}.cs", unitTestClass.TransformText());
        }

        private static ProjectModelBase AddAutomationTestFolder(ProjectModelBase locationForTest, UIAutomationTestCreationOptions creationOptions)
        {
            return locationForTest.AddFolder(creationOptions.TestFolderName);
        }
    }
}
