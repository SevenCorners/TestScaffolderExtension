namespace TestScaffolderExtension.CreateUIAutomationTestsCommand.Templates
{
    using System.Collections.Generic;
    using TestScaffolderExtension.CreateUIAutomationTestsCommand.TemplateProcessing;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution;

    public partial class PageValidatorTemplate
    {
        private readonly ProjectModelBase testLocation;
        private readonly UIAutomationTestCreationOptions automationTestOptions;

        private readonly List<string> usings = AutomationTemplateDetails.Usings;

        public PageValidatorTemplate(ProjectModelBase testLocation, UIAutomationTestCreationOptions automationTestOptions)
        {
            this.testLocation = testLocation;
            this.automationTestOptions = automationTestOptions;
        }
    }
}