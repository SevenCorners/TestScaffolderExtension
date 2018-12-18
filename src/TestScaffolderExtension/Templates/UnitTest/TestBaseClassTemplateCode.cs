namespace TestScaffolderExtension.Templates.UnitTest
{
    using System.Collections.Generic;
    using System.Linq;
    using TestScaffolderExtension.Models.Solution;
    using TestScaffolderExtension.Processors;

    public partial class TestBaseClassTemplate
    {
        private readonly ProjectModelBase unitTestProjectLocation;
        private readonly UnitTestCreationOptions unitTestCreationOptions;

        private readonly List<string> usings = new List<string>
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "Microsoft.VisualStudio.TestTools.UnitTesting",
            "Moq",
            "SevenCorners.TestExtensions"
        };

        public TestBaseClassTemplate(ProjectModelBase unitTestProjectLocation, UnitTestCreationOptions unitTestCreationOptions)
        {
            this.unitTestProjectLocation = unitTestProjectLocation;
            this.unitTestCreationOptions = unitTestCreationOptions;

            this.usings.Add(this.unitTestCreationOptions.ClassUnderTestNamespace);
            this.usings.AddRange(this.unitTestCreationOptions.ClassUnderTestConstructor.Parameters.SelectMany(p => p.Namespaces));
        }

        private IEnumerable<string> UsingStatements => this.usings.Distinct();
    }
}