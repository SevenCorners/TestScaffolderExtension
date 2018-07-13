using TestScaffolderExtension.Models;
using TestScaffolderExtension.Processors.UIAutomationTest;

namespace TestScaffolderExtension.ViewModels
{
    public class CreateUIAutomationTestsViewModel : ViewModelBase
    {
        public UIAutomationTestCreationOptions TestCreationOptions { get; }

        public CreateUIAutomationTestsViewModel() { }

        public CreateUIAutomationTestsViewModel(UIAutomationTestCreationOptions testCreationOptions)
        {
            TestCreationOptions = testCreationOptions;
        }

        public AutomationTestType TestType
        {
            get
            {
                return TestCreationOptions.TestType;
            }

            set
            {
                TestCreationOptions.TestType = value;
                OnPropertyChanged(nameof(TestType));
            }
        }

        public string PageName
        {
            get
            {
                return TestCreationOptions.PageName;
            }
            set
            {
                TestCreationOptions.PageName = value;

                OnPropertyChanged(nameof(PageName));
                OnPropertyChanged(nameof(TestFolderName));
                OnPropertyChanged(nameof(TestClassName));
                OnPropertyChanged(nameof(PageClassName));
                OnPropertyChanged(nameof(PageElementMapClassName));
                OnPropertyChanged(nameof(PageValidatorClassName));
                OnPropertyChanged(nameof(CanCreate));
            }
        }


        public string TestFolderName => TestCreationOptions.TestFolderName;
        public string TestClassName => TestCreationOptions.TestClassName;
        public string PageClassName => TestCreationOptions.PageClassName;
        public string PageElementMapClassName => TestCreationOptions.PageElementMapClassName;
        public string PageValidatorClassName => TestCreationOptions.PageValidatorClassName;

        public bool CanCreate => !string.IsNullOrEmpty(PageName);
    }
}