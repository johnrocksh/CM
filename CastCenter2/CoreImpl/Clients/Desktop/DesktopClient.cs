namespace CastManager.Client.Desktop
{
    using CastManager.Models.Desktops;
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class DesktopClient : IDesktopClient
    {
        private object lockObj = new();

        private CancellationTokenSource _cts;

        private string ip;

        private string Url => $"http://{ip}:80/Te/config/";

        public DesktopClient(string ip)
        {
            this.ip = ip;
        }

        public void Cancel()
        {
            lock (lockObj)
            {
                _cts?.Cancel();
            }
        }

        public async Task<DesktopConfig> LoadConfigAsync()
        {
            using (var client = new HttpClient())
            {
                lock (lockObj)
                {
                    _cts = new();
                }
                client.Timeout = TimeSpan.FromSeconds(5);

                try
                {
                    var httpResult = await client
                        .GetAsync(Url, _cts.Token)
                        .ConfigureAwait(false);

                    httpResult.EnsureSuccessStatusCode();

                    var configString = await httpResult.Content
                        .ReadAsStringAsync()
                        .ConfigureAwait(false);

                    if (string.IsNullOrEmpty(configString) == false)
                    {
                        return JsonConvert.DeserializeObject<DesktopConfig>(configString);
                    }
                }
                catch (Exception ec)
                {
                    Debug.WriteLine(ec.Message);
                }

                return null;
            }
        }
    }
}
