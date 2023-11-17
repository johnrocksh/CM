namespace CastManager.Models.Desktops
{
    using System.Linq;
    using Newtonsoft.Json;

    using CastManager.Client;
    using CastManager.Client.Desktop;
    using CastManager.StringsResources;

    public class DesktopInfo : NotifyPropertyBase
    {
        public IDesktopClient DesktopClient => desktopClient.Value;

        public DesktopConfig DesktopConfig { get; internal set; }

        private static string VisibleSource { get; } = "/images/desktop.png";

        private static string DisconnectedSource { get; } = "/images/desktopdisconnected.png";

        public string Source => DesktopConfig.IsActive ? VisibleSource : DisconnectedSource;

        public string TatamiText => DesktopConfig.TatamiName ?? Strings.Expected;

        public string DisciplineText => DesktopConfig.DisciplineText;

        public string Ip => DesktopConfig.Ip;

        public List<TatamiTabloInfo> TatamiConfigUrls => DesktopConfig.TatamiConfigUrls;

        private readonly Lazy<IDesktopClient> desktopClient;

        public DesktopInfo(IClientsFactory clientsFactory, string ip, string jsonConfig)
        {
            desktopClient = new Lazy<IDesktopClient>(() =>
            {
                var client = clientsFactory.GetDesktopClient(ip);
                if (client == null)
                {
                    throw new ArgumentNullException(nameof(DesktopClient));
                }
                return client;
            });

            DesktopConfig = JsonConvert.DeserializeObject<DesktopConfig>(jsonConfig);

            FilterDesktopConfigUrls(new List<string> { "castTvInfo" });            
        }
        public bool IsSameIp(DesktopInfo info) => Ip == info.Ip;

        void FilterDesktopConfigUrls(List<string> filter)
        {     
            var urls = DesktopConfig.TatamiConfigUrls
                .Where(tatami => filter.Count(text => tatami.Url.Contains(text)) == 0);

            DesktopConfig.TatamiConfigUrls = urls.ToList();
        }

    }
}
