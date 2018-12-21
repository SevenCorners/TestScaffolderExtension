namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Analysis
{
    using System.Collections.Generic;
    using System.Linq;

    public class ParameterInformation
    {
        public string Name { get; internal set; }

        public string SimpleTypeName { get; internal set; }

        public IEnumerable<string> Namespaces { get; internal set; }

        public ConstructorInformation Constructor { get; internal set; }

        public string NameAsPrivateField => $"_{char.ToLowerInvariant(this.Name.First())}{this.Name.Substring(1)}";

        public string NameAsProtectedField => $"{char.ToUpperInvariant(this.Name.First())}{this.Name.Substring(1)}";

        public string NameAsLocalVariable => $"{char.ToLowerInvariant(this.Name.First())}{this.Name.Substring(1)}";
    }
}