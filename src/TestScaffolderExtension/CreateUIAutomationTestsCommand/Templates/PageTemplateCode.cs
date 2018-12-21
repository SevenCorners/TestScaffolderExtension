namespace TestScaffolderExtension.CreateUIAutomationTestsCommand.Templates
{
    using System.Collections.Generic;
    using TestScaffolderExtension.CreateUIAutomationTestsCommand.TemplateProcessing;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution;

    public partial class PageTemplate
    {
        private readonly ProjectModelBase testLocation;
        private readonly UIAutomationTestCreationOptions automationTestOptions;
        private readonly List<string> usings = AutomationTemplateDetails.Usings;

        public PageTemplate(ProjectModelBase testLocation, UIAutomationTestCreationOptions automationTestOptions)
        {
            this.testLocation = testLocation;
            this.automationTestOptions = automationTestOptions;
        }
    }
}