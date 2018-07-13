using System.ComponentModel;

namespace TestScaffolderExtension.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}