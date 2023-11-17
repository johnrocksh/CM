namespace CastManager.ViewModels
{
    using System.Reflection;

    using GalaSoft.MvvmLight;

    using CastManager.Helper;
    using CastManager.StringsResources;

    public class InfoViewModel : ViewModelBase
    {
        public string LocalIp { get; set; }
        public string AssemblyVersion { get; set; }
        public InfoViewModel()
        {
            if (IsInDesignMode)
            {
                LocalIp = "Design mode: 127.0.0.1";
                AssemblyVersion = "Design mode: v1.0.1.2023";
            }
            else
            {
                var ip = Network.GetLocalIPAddress();
                var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                AssemblyVersion = Strings.ById("Version_Str") + version;
                LocalIp = Strings.IPAddress + ip;
            }
        }
    }
}
