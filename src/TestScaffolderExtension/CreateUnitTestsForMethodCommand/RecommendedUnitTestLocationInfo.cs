namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using Microsoft.CodeAnalysis;

    public class RecommendedUnitTestLocationInfo
    {
        public RecommendedUnitTestLocationInfo(Project recommendedUnitTestProject, string recommendedUnitTestPathFromProject)
        {
            this.RecommendedUnitTestProject = recommendedUnitTestProject;
            this.RecommendedUnitTestPathFromProject = recommendedUnitTestPathFromProject;
        }

        public string RecommendedUnitTestPathFromProject { get; set; }

        public Project RecommendedUnitTestProject { get; set; }

        public string RecommendedUnitTestProjectName => this.RecommendedUnitTestProject?.Name;
    }
}