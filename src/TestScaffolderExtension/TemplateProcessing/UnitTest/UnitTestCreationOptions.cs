using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using TestScaffolderExtension.Models.Analysis;

namespace TestScaffolderExtension.Processors
{
    public class UnitTestCreationOptions
    {
        public UnitTestCreationOptions()
        {
            ShouldCreateParentFolder = true;
            ShouldCreateUnitTestBaseClass = true;
        }

        public UnitTestCreationOptions(MethodDeclarationSyntax method, SemanticModel semanticModel) : this()
        {
            SetRoslynValues(method, semanticModel);
        }

        public bool ShouldCreateParentFolder { get; set; }
        public bool ShouldCreateUnitTestBaseClass { get; set; }


        public string ClassUnderTestNamespace { get; private set; }
        public string ClassUnderTestName { get; private set; }
        public string UnitTestBaseClassName { get; private set; }

        public ConstructorInformation ClassUnderTestConstructor { get; private set; }

        public string UnitTestFolderName { get; private set; }
        public string UnitTestBaseClassFileName { get; private set; }

        public readonly List<string> OtherNamespaces = new List<string>();

        public string MethodUnderTestName { get; private set; }
        public string UnitTestClassName { get; private set; }
        public string UnitTestClassFileName { get; private set; }
        public string MethodUnderTestReturnTypeName { get; private set; }
        public string MethodUnderTestReturnTypeNamespace { get; private set; }

        public List<ParameterInformation> MethodUnderTestParameters { get; private set; }

        private void SetRoslynValues(MethodDeclarationSyntax method, SemanticModel semanticModel)
        {
            MethodUnderTestName = method.Identifier.ValueText;
            UnitTestClassName = MethodUnderTestName;
            UnitTestClassFileName = $"{UnitTestClassName}.cs";

            MethodUnderTestReturnTypeName = method.ReturnType.ToString();
            OtherNamespaces.AddRange(GetNamespaces(method.ReturnType, semanticModel));

            var classUnderTest = semanticModel.GetDeclaredSymbol(method.Parent);
            var classDeclaration = method.Parent as TypeDeclarationSyntax;
            ClassUnderTestConstructor = GetConstructor(classDeclaration, semanticModel);

            ClassUnderTestName = classUnderTest.Name;
            ClassUnderTestNamespace = classUnderTest.ContainingNamespace.ToDisplayString();



            UnitTestFolderName = $"{ClassUnderTestName}Tests";
            UnitTestBaseClassName = $"{ClassUnderTestName}TestsBase";
            UnitTestBaseClassFileName = $"{UnitTestBaseClassName}.cs";
            MethodUnderTestParameters = GetFullParameterInfo(method.ParameterList.Parameters, semanticModel);
        }

        private ConstructorInformation GetConstructor(TypeDeclarationSyntax classDeclaration, SemanticModel semanticModel)
        {
            var constructorType = classDeclaration is ClassDeclarationSyntax ? ConstructorType.New : ConstructorType.Default;

            if(classDeclaration.Identifier.ValueText.Equals("string", System.StringComparison.OrdinalIgnoreCase))
            {
                return new ConstructorInformation(classDeclaration.Identifier.ValueText, ConstructorType.Default);
            }

            var simplestConstructor = classDeclaration.Members.OfType<ConstructorDeclarationSyntax>().OrderBy(c => c.ParameterList.Parameters.Count).FirstOrDefault();
            if (simplestConstructor == null)
            {
                return new ConstructorInformation(classDeclaration.Identifier.ValueText, constructorType);
            }

            return new ConstructorInformation(simplestConstructor.Identifier.ValueText, constructorType, GetSimpleParameterInfo(simplestConstructor.ParameterList.Parameters, semanticModel));
        }

        private ConstructorInformation GetConstructor(ParameterSyntax parameter, SemanticModel semanticModel)
        {
            var parameterTypeSymbol = semanticModel.GetDeclaredSymbol(parameter).Type;
            var constructorType = parameterTypeSymbol.IsValueType ? ConstructorType.Default : ConstructorType.New;

            if(parameterTypeSymbol.ToDisplayString().Equals("string", System.StringComparison.OrdinalIgnoreCase))
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

            var constructorName = GetConstructorName(parameterTypeSymbol);
            return new ConstructorInformation(constructorName, constructorType, GetSimpleParameterInfo(constructorParameters));
        }

        private string GetConstructorName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsReferenceType && typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                return namedTypeSymbol.IsGenericType ? $"{namedTypeSymbol.Name}<{string.Join(", ", namedTypeSymbol.TypeArguments.Select(t => t.Name))}>" : namedTypeSymbol.Name;
            }

            return typeSymbol.ToDisplayString();
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

        private List<ParameterInformation> GetFullParameterInfo(SeparatedSyntaxList<ParameterSyntax> parameters, SemanticModel semanticModel)
        {
            return parameters.Select(p => new ParameterInformation
            {
                Name = p.Identifier.ValueText,
                SimpleTypeName = p.Type.ToString(),
                Namespaces = GetNamespaces(p.Type, semanticModel),
                Constructor = GetConstructor(p, semanticModel)
            }).ToList();
        }

        private List<ParameterInformation> GetSimpleParameterInfo(SeparatedSyntaxList<ParameterSyntax> parameters, SemanticModel semanticModel)
        {
            return parameters.Select(p => new ParameterInformation
            {
                Name = p.Identifier.ValueText,
                SimpleTypeName = p.Type.ToString(),
                Namespaces = GetNamespaces(p.Type, semanticModel)
            }).ToList();
        }

        private IEnumerable<string> GetNamespaces(TypeSyntax type, SemanticModel model)
        {
            yield return model.GetTypeInfo(type).Type.ContainingNamespace.ToDisplayString();
            if (type is GenericNameSyntax generic)
            {
                foreach (var argNamespace in generic.TypeArgumentList.Arguments.SelectMany(a => GetNamespaces(a, model)))
                {
                    yield return argNamespace;
                }
            }
        }
    }
}