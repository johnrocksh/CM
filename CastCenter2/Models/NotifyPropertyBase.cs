namespace CastManager.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class NotifyPropertyBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
