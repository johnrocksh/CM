namespace CastManager.Client.Desktop
{
    using CastManager.Models.Desktops;
    using System.Threading.Tasks;

    internal class DesktopClientMock : IDesktopClient
    {
        protected string mockId = string.Empty;
        protected string mockIp = string.Empty;

        public DesktopClientMock(string mockId, string ip)
        {
            this.mockId = mockId;
            mockIp = ip;
        }

        public Task<DesktopConfig> LoadConfigAsync()
        {
            var desktopConfig = new DesktopConfig()
            {
                DesktopVersion = "Mock_v1",
                Ip = $"{mockIp}",
                TatamiName = $"Mock_{mockId}"
            };

            if (mockId.Contains("poll"))
            {
                desktopConfig.TatamiName = null;
                return Task.FromResult(desktopConfig);
            }

            desktopConfig.TatamiConfigUrls.Add(
                new TatamiTabloInfo()
                {
                    Url = "http://127.0.0.1:9000/notactual_config.png",
                    Title = $"Mock{mockId}_Title_1"
                });
            desktopConfig.TatamiConfigUrls.Add(
                new TatamiTabloInfo()
                {
                    Url = "http://127.0.0.1:9000/mock.png",
                    Title = $"Mock{mockId}_Title_2"
                });
            desktopConfig.TatamiConfigUrls.Add(
                new TatamiTabloInfo()
                {
                    Url = "http://127.0.0.1:9000/HorizontalTemplate.png",
                    Title = $"Mock{mockId}_Title_3"
                });
            desktopConfig.TatamiConfigUrls.Add(
                new TatamiTabloInfo()
                {
                    Url = "http://127.0.0.1:9000/VerticalTemplate.png",
                    Title = $"Mockv{mockId}_Title_4"
                });

            desktopConfig.TatamiConfigUrls.Add(
                new TatamiTabloInfo()
                {
                    Url = "http://192.168.56.1/Te/B_currentSkirmish.png",
                    Title = $"Mock{mockId}_URL"
                });


            return Task.FromResult(desktopConfig);
        }
        public void Cancel()
        {

        }

    }
}
