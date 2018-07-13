using System.Collections.Generic;
using TestScaffolderExtension.Models.Solution;
using TestScaffolderExtension.Processors.UIAutomationTest;

namespace TestScaffolderExtension.Templates.UIAutomationTest
{
    public partial class TestTemplate
    {
        private readonly ProjectModelBase _testLocation;
        private readonly UIAutomationTestCreationOptions _automationTestOptions;

        private readonly List<string> Usings = AutomationTemplateDetails.Usings;

        public TestTemplate(ProjectModelBase testLocation, UIAutomationTestCreationOptions automationTestOptions)
        {
            _testLocation = testLocation;
            _automationTestOptions = automationTestOptions;
        }
    }
}