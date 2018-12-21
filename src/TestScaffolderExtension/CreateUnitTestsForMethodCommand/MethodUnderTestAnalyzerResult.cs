namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.TemplateProcessing;

    public class MethodUnderTestAnalyzerResult
    {
        public MethodUnderTestAnalyzerResult(UnitTestCreationDetails unitTestCreationDetails, RecommendedUnitTestLocationInfo recommendedLocation)
        {
            this.UnitTestCreationDetails = unitTestCreationDetails;
            this.RecommendedUnitTestLocationInfo = recommendedLocation;
        }

        public UnitTestCreationDetails UnitTestCreationDetails { get; set; }

        public RecommendedUnitTestLocationInfo RecommendedUnitTestLocationInfo { get; }
    }
}