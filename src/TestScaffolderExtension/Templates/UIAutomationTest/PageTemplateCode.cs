namespace TestScaffolderExtension.Templates.UIAutomationTest
{
    using System.Collections.Generic;
    using TestScaffolderExtension.Models.Solution;
    using TestScaffolderExtension.Processors.UIAutomationTest;

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