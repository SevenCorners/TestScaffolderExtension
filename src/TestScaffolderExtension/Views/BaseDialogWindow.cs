using Microsoft.VisualStudio.PlatformUI;

namespace TestScaffolderExtension.Views
{
    public class BaseDialogWindow : DialogWindow
    {
        public BaseDialogWindow()
        {
            HasMaximizeButton = true;
            HasMinimizeButton = true;
        }
    }
}