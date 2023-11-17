namespace CastManager.Client.Device
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Net.Http.Json;

    using CastManager.Logger;

    class DeviceClient : IDeviceClient
    {
        private readonly string address;

        private readonly ushort port;

        public string makeEndpointUrl(string suffix)
        {
            return $"http://{address}:{port}/{suffix}";
        }

        internal DeviceClient(string address, ushort port)
        {
            this.address = address;
            this.port = port;
        }

        public async Task WifiAccessPointMode(AccessPointConfig accessPointConfig)
        {
            string endpointUrl = makeEndpointUrl("wifi/apMode");

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsJsonAsync(endpointUrl, accessPointConfig);
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        async Task<string> IDeviceClient.GetVersionAsync()
        {
            string endpointUrl = makeEndpointUrl("version");

            using (var client = new HttpClient())
            {
                var httpResult = await client.GetAsync(endpointUrl);

                httpResult.EnsureSuccessStatusCode();

                return await httpResult
                  .Content
                  .ReadAsStringAsync();
            }
        }

        Task<string> IDeviceClient.GetInfoAsync()
        {
            string endpointUrl = makeEndpointUrl("info");

            using (var client = new HttpClient())
            {
                var task = client.GetAsync(endpointUrl);
                if (task.Wait(1750))
                {
                    var httpResult = task.Result;

                    if (httpResult.IsSuccessStatusCode)
                    {
                        httpResult.EnsureSuccessStatusCode();

                        return httpResult.Content.ReadAsStringAsync();
                    }
                }
            }
            return Task.FromResult("");
        }

        async Task<string> IDeviceClient.GetConfigAsync()
        {
            string endpointUrl = makeEndpointUrl("config");
            using (var client = new HttpClient())
            {
                try
                {
                    var httpResult = await client.GetAsync(endpointUrl);
                    httpResult.EnsureSuccessStatusCode();

                    return await httpResult.Content.ReadAsStringAsync();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
                return string.Empty;
            }
        }

        async Task IDeviceClient.SetConfigAsync(string config)
        {
            Debug.WriteLine($"casterConfig= {config}");

            string endpointUrl = makeEndpointUrl("config");

            using (var client = new HttpClient())
            {
                Uri u = new Uri(endpointUrl);
                Debug.WriteLine($"LOG u={u}");

                HttpContent c = new StringContent(config, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpointUrl, c);

                response.EnsureSuccessStatusCode();
            }
        }

        bool IDeviceClient.IsAlive()
        {
            try
            {
                var t = ((IDeviceClient)this).GetVersionAsync();
                t.Wait();
                return t.Result != null;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public async Task<bool> ConnectToWifiNetwork(WifiConnectData connectData)
        {
            try
            {
                string endpointUrl = makeEndpointUrl("wifi/connect");
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync(endpointUrl, connectData);
                    response.EnsureSuccessStatusCode();
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.WriteLine($"ConnectToWifiNetwork Exception : {e.ToString()}");
            }
            return false;
        }
    }
}
