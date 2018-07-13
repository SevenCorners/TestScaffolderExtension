using System.Linq;
using TestScaffolderExtension.Models;

namespace TestScaffolderExtension.Processors.UIAutomationTest
{
    public class UIAutomationTestCreationOptions
    {
        public string PageName { get; set; }

        public AutomationTestType TestType { get; set; }

        public string TestFolderName => string.IsNullOrEmpty(PageName) ? string.Empty : $"{PageName}Tests";
        public string TestClassName => string.IsNullOrEmpty(PageName) ? string.Empty : $"{PageName}Tests";
        public string PageClassName => string.IsNullOrEmpty(PageName) ? string.Empty : $"{PageName}Page";
        public string PageElementMapClassName => string.IsNullOrEmpty(PageName) ? string.Empty : $"{PageName}PageElementMap";
        public string PageValidatorClassName => string.IsNullOrEmpty(PageName) ? string.Empty : $"{PageName}PageValidator";

        public string PageTypeNameAsPrivateField => $"_{char.ToLowerInvariant(PageClassName.First())}{PageClassName.Substring(1)}";

        public string TestTypeCategory
        {
            get
            {
                switch (TestType)
                {
                    case AutomationTestType.FunctionalTest:
                        return "Functional";
                    case AutomationTestType.SmokeTest:
                    default:
                        return "Smoke";
                }
            }
        }

        public string TestTypeBaseClassName
        {
            get
            {
                switch (TestType)
                {
                    case AutomationTestType.FunctionalTest:
                        return "FunctionalTestBase";
                    case AutomationTestType.SmokeTest:
                    default:
                        return "SmokeTestBase";
                }
            }
        }
    }
}