namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Editor;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.TextManager.Interop;
    using TestScaffolderExtension.Common.Command;
    using TestScaffolderExtension.Common.Extensions;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.TemplateProcessing;

    public partial class MethodUnderTestAnalyzer
    {
        private readonly IAsyncServiceProvider asyncServiceProvider;

        public MethodUnderTestAnalyzer(IAsyncServiceProvider asyncServiceProvider)
        {
            this.asyncServiceProvider = asyncServiceProvider;
        }

        public async Task<MethodUnderTestAnalyzerResult> AnalyzeAsync()
        {
            var componentModel = await this.asyncServiceProvider.GetAsAsync<SComponentModel, IComponentModel>();
            var methodUnderTest = await this.GetMethodUnderTestAsync(componentModel);
            var unitTestCreationDetails = new UnitTestCreationDetails(methodUnderTest);
            var recommendedLocation = this.CalculateRecommendedUnitTestLocation(methodUnderTest, unitTestCreationDetails, componentModel);

            return new MethodUnderTestAnalyzerResult(unitTestCreationDetails, recommendedLocation);
        }

        private RecommendedUnitTestLocationInfo CalculateRecommendedUnitTestLocation(MethodUnderTestInfo methodUnderTest, UnitTestCreationDetails unitTestCreationDetails, IComponentModel componentModel)
        {
            var workspace = componentModel.GetService<VisualStudioWorkspace>();
            var documentToTest = workspace.CurrentSolution.GetDocumentIdsWithFilePath(methodUnderTest.Document.FilePath).FirstOrDefault();
            var projectUnderTest = workspace.CurrentSolution.GetProject(documentToTest.ProjectId);
            var recommendedUnitTestProject = this.GetRecommendedUnitTestProject(workspace, projectUnderTest);
            var recommendedUnitTestPathFromProject = this.GetInnerPathFromProject(projectUnderTest, unitTestCreationDetails);

            return new RecommendedUnitTestLocationInfo(recommendedUnitTestProject, recommendedUnitTestPathFromProject);
        }

        private async Task<SnapshotPoint> GetCaretPositionAsync(IComponentModel componentModel)
        {
            var editor = componentModel
                .GetService<IVsEditorAdaptersFactoryService>();

            var textManager = await this.asyncServiceProvider
                .GetAsAsync<SVsTextManager, IVsTextManager>();

            textManager.GetActiveView(1, null, out var textView);

            return editor
                .GetWpfTextView(textView)
                .Caret
                .Position
                .BufferPosition;
        }

        private string GetInnerPathFromProject(Project projectUnderTest, UnitTestCreationDetails unitTestCreationDetails)
        {
            string namespaceAfterProject = string.Empty;
            var classUnderTestNamespace = unitTestCreationDetails.ClassUnderTestNamespace;
            if (classUnderTestNamespace.Contains(projectUnderTest.Name))
            {
                namespaceAfterProject = this.GetNamespaceSubstring(projectUnderTest.Name, classUnderTestNamespace);
            }
            else if (classUnderTestNamespace.Contains(projectUnderTest.AssemblyName))
            {
                namespaceAfterProject = this.GetNamespaceSubstring(projectUnderTest.AssemblyName, classUnderTestNamespace);
            }

            return namespaceAfterProject;
        }

        private async Task<MethodUnderTestInfo> GetMethodUnderTestAsync(IComponentModel componentModel)
        {
            var snapshotPoint = await this.GetCaretPositionAsync(componentModel);

            var documentUnderTest = snapshotPoint
                .Snapshot
                .GetOpenDocumentInCurrentContextWithChanges();

            var root = await documentUnderTest.GetSyntaxRootAsync();
            var semanticModel = await documentUnderTest.GetSemanticModelAsync();

            if (root.FindToken(snapshotPoint).Parent is MethodDeclarationSyntax method)
            {
                return new MethodUnderTestInfo(method, semanticModel, documentUnderTest);
            }

            throw new CommandException("Invalid Selection", "Please select a method to test.");
        }

        private string GetNamespaceSubstring(string projectUnderTestName, string classUnderTestNamespace)
        {
            var indexAfterProjectName = classUnderTestNamespace.IndexOf(projectUnderTestName) + projectUnderTestName.Length + 1;
            if (indexAfterProjectName > classUnderTestNamespace.Length)
            {
                return string.Empty;
            }

            return classUnderTestNamespace.Substring(indexAfterProjectName);
        }

        private Project GetRecommendedUnitTestProject(VisualStudioWorkspace workspace, Project projectToTest)
        {
            // find project with same name then .UnitTest
            // or closest match then .UnitTest
            return workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name == $"{projectToTest.Name}.UnitTest")
                ?? workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name == $"{projectToTest.AssemblyName}.UnitTest")
                ?? workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name.EndsWith(".UnitTest") && projectToTest.Name.Contains(p.Name.Substring(0, p.Name.LastIndexOf("."))))
                ?? workspace.CurrentSolution.Projects.FirstOrDefault(p => p.Name.EndsWith(".UnitTest") && projectToTest.AssemblyName.Contains(p.Name.Substring(0, p.Name.LastIndexOf("."))));
        }
    }
}