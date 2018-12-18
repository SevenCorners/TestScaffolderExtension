namespace TestScaffolderExtension.Processors.UIAutomationTest
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TestScaffolderExtension.Models.Solution;
    using TestScaffolderExtension.Templates.UIAutomationTest;

    internal static class UIAutomationTestTemplateInstantiator
    {
        internal static async Task<IEnumerable<FileModel>> InstantiateAsync(ProjectModelBase selectedProjectNode, UIAutomationTestCreationOptions automationTestOptions)
        {
            var testFolder = await AddAutomationTestFolderAsync(selectedProjectNode, automationTestOptions);

            return await Task.WhenAll(
                AddAutomationTestClassAsync(testFolder, automationTestOptions),
                AddAutomationPageClassAsync(testFolder, automationTestOptions),
                AddAutomationPageElementMapClassAsync(testFolder, automationTestOptions),
                AddAutomationPageValidatorClassAsync(testFolder, automationTestOptions));
        }

        private static async Task<FileModel> AddAutomationPageValidatorClassAsync(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new PageValidatorTemplate(testFolder, automationTestOptions);
            return await testFolder.AddFileAsync($"{automationTestOptions.PageValidatorClassName}.cs", unitTestClass.TransformText());
        }

        private static async Task<FileModel> AddAutomationPageElementMapClassAsync(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new PageElementMapTemplate(testFolder, automationTestOptions);
            return await testFolder.AddFileAsync($"{automationTestOptions.PageElementMapClassName}.cs", unitTestClass.TransformText());
        }

        private static async Task<FileModel> AddAutomationPageClassAsync(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new PageTemplate(testFolder, automationTestOptions);
            return await testFolder.AddFileAsync($"{automationTestOptions.PageClassName}.cs", unitTestClass.TransformText());
        }

        private static async Task<FileModel> AddAutomationTestClassAsync(ProjectModelBase testFolder, UIAutomationTestCreationOptions automationTestOptions)
        {
            var unitTestClass = new TestTemplate(testFolder, automationTestOptions);
            return await testFolder.AddFileAsync($"{automationTestOptions.TestClassName}.cs", unitTestClass.TransformText());
        }

        private static async Task<ProjectModelBase> AddAutomationTestFolderAsync(ProjectModelBase locationForTest, UIAutomationTestCreationOptions creationOptions)
        {
            return await locationForTest.AddFolderAsync(creationOptions.TestFolderName);
        }
    }
}