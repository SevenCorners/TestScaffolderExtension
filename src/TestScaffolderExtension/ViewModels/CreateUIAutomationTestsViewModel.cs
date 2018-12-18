namespace TestScaffolderExtension.ViewModels
{
    using TestScaffolderExtension.Models;
    using TestScaffolderExtension.Processors.UIAutomationTest;

    public class CreateUIAutomationTestsViewModel : ViewModelBase
    {
        public CreateUIAutomationTestsViewModel()
        {
        }

        public CreateUIAutomationTestsViewModel(UIAutomationTestCreationOptions testCreationOptions)
        {
            this.TestCreationOptions = testCreationOptions;
        }

        public bool CanCreate => !string.IsNullOrEmpty(this.PageName);

        public string PageClassName => this.TestCreationOptions.PageClassName;

        public string PageElementMapClassName => this.TestCreationOptions.PageElementMapClassName;

        public string PageName
        {
            get
            {
                return this.TestCreationOptions.PageName;
            }

            set
            {
                this.TestCreationOptions.PageName = value;

                this.OnPropertyChanged(nameof(this.PageName));
                this.OnPropertyChanged(nameof(this.TestFolderName));
                this.OnPropertyChanged(nameof(this.TestClassName));
                this.OnPropertyChanged(nameof(this.PageClassName));
                this.OnPropertyChanged(nameof(this.PageElementMapClassName));
                this.OnPropertyChanged(nameof(this.PageValidatorClassName));
                this.OnPropertyChanged(nameof(this.CanCreate));
            }
        }

        public string PageValidatorClassName => this.TestCreationOptions.PageValidatorClassName;

        public string TestClassName => this.TestCreationOptions.TestClassName;

        public UIAutomationTestCreationOptions TestCreationOptions { get; }

        public string TestFolderName => this.TestCreationOptions.TestFolderName;

        public AutomationTestType TestType
        {
            get
            {
                return this.TestCreationOptions.TestType;
            }

            set
            {
                this.TestCreationOptions.TestType = value;
                this.OnPropertyChanged(nameof(this.TestType));
            }
        }
    }
}