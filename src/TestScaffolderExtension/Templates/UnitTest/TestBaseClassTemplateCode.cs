using System.Collections.Generic;
using System.Linq;
using TestScaffolderExtension.Models.Solution;
using TestScaffolderExtension.Processors;

namespace TestScaffolderExtension.Templates.UnitTest
{
    public partial class TestBaseClassTemplate
    {
        private readonly ProjectModelBase _unitTestProjectLocation;
        private readonly UnitTestCreationOptions _unitTestCreationOptions;

        private readonly List<string> _usings = new List<string>
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
            _unitTestProjectLocation = unitTestProjectLocation;
            _unitTestCreationOptions = unitTestCreationOptions;

            _usings.Add(_unitTestCreationOptions.ClassUnderTestNamespace);
            _usings.AddRange(_unitTestCreationOptions.ClassUnderTestConstructor.Parameters.SelectMany(p => p.Namespaces));
        }

        private IEnumerable<string> UsingStatements => _usings.Distinct();
    }
}