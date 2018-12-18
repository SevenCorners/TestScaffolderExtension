namespace TestScaffolderExtension.Templates.UIAutomationTest
{
    using System.Collections.Generic;

    internal static class AutomationTemplateDetails
    {
        internal static List<string> Usings => new List<string>
        {
            "AxisCoreAutomationHelpers.Utilities",
            "PageObjectModel",
            "AxisCoreAutomationHelpers",
            "NUnit.Framework",
            "OpenQA.Selenium"
        };
    }
}