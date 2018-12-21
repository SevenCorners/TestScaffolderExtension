namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand.Templates
{
    using System.Collections.Generic;
    using System.Linq;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.TemplateProcessing;

    public partial class TestBaseClassTemplate
    {
        private readonly ProjectModelBase unitTestProjectLocation;
        private readonly UnitTestCreationDetails unitTestCreationOptions;

        private readonly List<string> usings = new List<string>
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "Microsoft.VisualStudio.TestTools.UnitTesting",
            "Moq",
            "SevenCorners.TestExtensions"
        };

        public TestBaseClassTemplate(ProjectModelBase unitTestProjectLocation, UnitTestCreationDetails unitTestCreationOptions)
        {
            this.unitTestProjectLocation = unitTestProjectLocation;
            this.unitTestCreationOptions = unitTestCreationOptions;

            this.usings.Add(this.unitTestCreationOptions.ClassUnderTestNamespace);
            this.usings.AddRange(this.unitTestCreationOptions.ClassUnderTestConstructor.Parameters.SelectMany(p => p.Namespaces));
        }

        private IEnumerable<string> UsingStatements => this.usings.Distinct();
    }
}