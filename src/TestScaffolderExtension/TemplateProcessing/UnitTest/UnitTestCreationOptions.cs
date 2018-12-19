namespace TestScaffolderExtension.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using TestScaffolderExtension.Models.Analysis;

    public class UnitTestCreationOptions
    {
        public UnitTestCreationOptions()
        {
            this.ShouldCreateParentFolder = true;
            this.ShouldCreateUnitTestBaseClass = true;
        }

        public UnitTestCreationOptions(MethodDeclarationSyntax method, SemanticModel semanticModel)
            : this()
        {
            this.SetUnitTestCreationValues(method, semanticModel);
        }

        public ConstructorInformation ClassUnderTestConstructor { get; private set; }

        public string ClassUnderTestName { get; private set; }

        public string ClassUnderTestNamespace { get; private set; }

        public string MethodUnderTestName { get; private set; }

        public List<ParameterInformation> MethodUnderTestParameters { get; private set; }

        public string MethodUnderTestReturnTypeName { get; private set; }

        public string MethodUnderTestReturnTypeNamespace { get; private set; }

        public List<string> OtherNamespaces { get; } = new List<string>();

        public bool ShouldCreateParentFolder { get; set; }

        public bool ShouldCreateUnitTestBaseClass { get; set; }

        public string UnitTestBaseClassFileName { get; private set; }

        public string UnitTestBaseClassName { get; private set; }

        public string UnitTestClassFileName { get; private set; }

        public string UnitTestClassName { get; private set; }

        public string UnitTestFolderName { get; private set; }

        private ConstructorInformation GetConstructor(TypeDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var constructorType = classDeclaration is ClassDeclarationSyntax ? ConstructorType.New : ConstructorType.Default;

            if (classDeclaration.Identifier.ValueText.Equals("string", System.StringComparison.OrdinalIgnoreCase))
            {
                return new ConstructorInformation(classDeclaration.Identifier.ValueText, ConstructorType.Default);
            }

            var simplestConstructor = classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().OrderBy(c => c.ParameterList.Parameters.Count).FirstOrDefault();
            if (simplestConstructor == null)
            {
                return new ConstructorInformation(classDeclaration.Identifier.ValueText, constructorType);
            }

            return new ConstructorInformation(simplestConstructor.Identifier.ValueText, constructorType, this.GetSimpleParameterInfo(simplestConstructor.ParameterList.Parameters, semanticModel));
        }

        private ConstructorInformation GetConstructor(ParameterSyntax parameter, SemanticModel semanticModel)
        {
            var parameterTypeSymbol = semanticModel.GetDeclaredSymbol(parameter).Type;
            var constructorType = parameterTypeSymbol.IsValueType ? ConstructorType.Default : ConstructorType.New;

            if (parameterTypeSymbol.ToDisplayString().Equals("string", System.StringComparison.OrdinalIgnoreCase))
            {
                constructorType = ConstructorType.Default;
            }

            if (constructorType == ConstructorType.Default)
            {
                return new ConstructorInformation(parameterTypeSymbol.ToDisplayString(), constructorType);
            }

            var constructors = parameterTypeSymbol.GetMembers().OfType<IMethodSymbol>().Where(m => m.MethodKind == MethodKind.Constructor).OrderBy(c => c.Parameters.Count());
            var simplestConstructor = constructors.FirstOrDefault();
            var constructorParameters = simplestConstructor?.Parameters.AsEnumerable() ?? Enumerable.Empty<IParameterSymbol>();

            var constructorName = this.GetConstructorName(parameterTypeSymbol);
            return new ConstructorInformation(constructorName, constructorType, this.GetSimpleParameterInfo(constructorParameters));
        }

        private string GetConstructorName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsReferenceType && typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                return namedTypeSymbol.IsGenericType ? $"{namedTypeSymbol.Name}<{string.Join(", ", namedTypeSymbol.TypeArguments.Select(t => t.Name))}>" : namedTypeSymbol.Name;
            }

            return typeSymbol.ToDisplayString();
        }

        private List<ParameterInformation> GetFullParameterInfo(SeparatedSyntaxList<ParameterSyntax> parameters, SemanticModel semanticModel)
        {
            return parameters.Select(p => new ParameterInformation
            {
                Name = p.Identifier.ValueText,
                SimpleTypeName = p.Type.ToString(),
                Namespaces = this.GetNamespaces(p.Type, semanticModel),
                Constructor = this.GetConstructor(p, semanticModel)
            }).ToList();
        }

        private IEnumerable<string> GetNamespaces(TypeSyntax type, SemanticModel model)
        {
            yield return model.GetTypeInfo(type).Type.ContainingNamespace.ToDisplayString();
            if (type is GenericNameSyntax generic)
            {
                foreach (var argNamespace in generic.TypeArgumentList.Arguments.SelectMany(a => this.GetNamespaces(a, model)))
                {
                    yield return argNamespace;
                }
            }
        }

        private List<ParameterInformation> GetSimpleParameterInfo(IEnumerable<IParameterSymbol> constructorParameters)
        {
            return constructorParameters.Select(p => new ParameterInformation
            {
                Name = p.Name,
                SimpleTypeName = p.Type.ToDisplayString(),
                Namespaces = new List<string> { p.Type.ContainingNamespace.ToDisplayString() }
            }).ToList();
        }

        private List<ParameterInformation> GetSimpleParameterInfo(SeparatedSyntaxList<ParameterSyntax> parameters, SemanticModel semanticModel)
        {
            return parameters.Select(p => new ParameterInformation
            {
                Name = p.Identifier.ValueText,
                SimpleTypeName = p.Type.ToString(),
                Namespaces = this.GetNamespaces(p.Type, semanticModel)
            }).ToList();
        }

        private void SetUnitTestCreationValues(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            this.MethodUnderTestName = method.Identifier.ValueText;
            this.UnitTestClassName = this.MethodUnderTestName;
            this.UnitTestClassFileName = $"{this.UnitTestClassName}.cs";

            this.MethodUnderTestReturnTypeName = method.ReturnType.ToString();
            this.OtherNamespaces.AddRange(this.GetNamespaces(method.ReturnType, semanticModel));

            var classUnderTest = semanticModel.GetDeclaredSymbol(method.Parent);
            var classDeclaration = method.Parent as TypeDeclarationSyntax;
            this.ClassUnderTestConstructor = this.GetConstructor(classDeclaration, semanticModel);

            this.ClassUnderTestName = classUnderTest.Name;
            this.ClassUnderTestNamespace = classUnderTest.ContainingNamespace.ToDisplayString();

            this.UnitTestFolderName = $"{this.ClassUnderTestName}Tests";
            this.UnitTestBaseClassName = $"{this.ClassUnderTestName}TestsBase";
            this.UnitTestBaseClassFileName = $"{this.UnitTestBaseClassName}.cs";
            this.MethodUnderTestParameters = this.GetFullParameterInfo(method.ParameterList.Parameters, semanticModel);
        }
    }
}