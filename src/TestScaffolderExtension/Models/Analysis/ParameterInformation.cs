using System.Collections.Generic;
using System.Linq;

namespace TestScaffolderExtension.Models.Analysis
{
    public class ParameterInformation
    {
        public string Name { get; internal set; }
        public string SimpleTypeName { get; internal set; }
        public IEnumerable<string> Namespaces { get; internal set; }
        public ConstructorInformation Constructor { get; internal set; }

        public string NameAsPrivateField => $"_{char.ToLowerInvariant(Name.First())}{Name.Substring(1)}";
        public string NameAsProtectedField => $"{char.ToUpperInvariant(Name.First())}{Name.Substring(1)}";
        public string NameAsLocalVariable => $"{char.ToLowerInvariant(Name.First())}{Name.Substring(1)}";
    }
}