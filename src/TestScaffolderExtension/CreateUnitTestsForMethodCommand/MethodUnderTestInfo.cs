namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public partial class MethodUnderTestAnalyzer
    {
        public class MethodUnderTestInfo
        {
            public MethodUnderTestInfo(MethodDeclarationSyntax method, SemanticModel semanticModel, Document document)
            {
                this.Method = method;
                this.SemanticModel = semanticModel;
                this.Document = document;
            }

            public Document Document { get; }

            internal MethodDeclarationSyntax Method { get; }

            internal SemanticModel SemanticModel { get; }
        }
    }
}