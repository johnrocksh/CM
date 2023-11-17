namespace CastManager.Models.Desktops
{
    using CastManager.Helper;
    using Newtonsoft.Json;
    using System.ComponentModel;
    using System.Windows.Media.Imaging;

    public class TatamiTabloInfo : NotifyPropertyBase
    {
        public string Url { get; set; }
        public string Title { get; set; }

        public void Udate()
        {
            RaisePropertyChanged(nameof(Url));
            RaisePropertyChanged(nameof(Title));
        }
    }
}
