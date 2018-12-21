namespace TestScaffolderExtension.CreateUnitTestsForMethodCommand
{
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.Models.Solution;
    using TestScaffolderExtension.CreateUnitTestsForMethodCommand.TemplateProcessing;

    public class UnitTestCreationOptions
    {
        public UnitTestCreationOptions(
            ProjectModelBase locationForTest,
            UnitTestCreationDetails unitTestCreationDetails,
            bool shouldCreateParentFolder,
            bool shouldCreateUnitTestBaseClass)
        {
            this.LocationForTest = locationForTest;
            this.UnitTestCreationDetails = unitTestCreationDetails;
            this.ShouldCreateParentFolder = shouldCreateParentFolder;
            this.ShouldCreateUnitTestBaseClass = shouldCreateUnitTestBaseClass;
        }

        public ProjectModelBase LocationForTest { get; internal set; }

        public bool ShouldCreateParentFolder { get; }

        public bool ShouldCreateUnitTestBaseClass { get; }

        public UnitTestCreationDetails UnitTestCreationDetails { get; }
    }
}