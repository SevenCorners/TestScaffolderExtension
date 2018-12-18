namespace TestScaffolderExtension.Templates.UIAutomationTest
{
    using System.Collections.Generic;
    using TestScaffolderExtension.Models.Solution;
    using TestScaffolderExtension.Processors.UIAutomationTest;

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