namespace TestScaffolderExtension.Processors.UIAutomationTest
{
    using System.Linq;
    using TestScaffolderExtension.Models;

    public class UIAutomationTestCreationOptions
    {
        public string PageName { get; set; }

        public AutomationTestType TestType { get; set; }

        public string TestFolderName => string.IsNullOrEmpty(this.PageName) ? string.Empty : $"{this.PageName}Tests";

        public string TestClassName => string.IsNullOrEmpty(this.PageName) ? string.Empty : $"{this.PageName}Tests";

        public string PageClassName => string.IsNullOrEmpty(this.PageName) ? string.Empty : $"{this.PageName}Page";

        public string PageElementMapClassName => string.IsNullOrEmpty(this.PageName) ? string.Empty : $"{this.PageName}PageElementMap";

        public string PageValidatorClassName => string.IsNullOrEmpty(this.PageName) ? string.Empty : $"{this.PageName}PageValidator";

        public string PageTypeNameAsPrivateField => $"_{char.ToLowerInvariant(this.PageClassName.First())}{this.PageClassName.Substring(1)}";

        public string TestTypeCategory
        {
            get
            {
                switch (this.TestType)
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
                switch (this.TestType)
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