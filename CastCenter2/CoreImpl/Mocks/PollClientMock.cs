namespace CastManager.Client.Poll
{
    using CastManager.Client.Desktop;
    using CastManager.Client.Device;
    using Nancy;
    using Newtonsoft.Json;
    using NLog.Targets.Wrappers;

    public class PollClientMock : IPollClient
    {
        Dictionary<string, DesktopClientMock> desktops = new();
        Dictionary<string, DeviceClientMock> devices = new();

        static int _CountDevices = 0;
        static int _CountDesktops = 0;
        static object lockObj = new object();

        public PollClientMock(TimeSpan timeout, CancellationTokenSource cs)
        {

        }

        public async Task<string> GetAsync(string url)
        {
            Random rnd = new Random();
            await Task.Delay(rnd.Next(500, 1000)); // simulate pending

            lock (lockObj)
            {
                var Url = new Url(url);
                if (url.Contains("Te/config/"))
                {
                    DesktopClientMock desktopClient;
                    if (desktops.TryGetValue(url, out desktopClient) == false)
                    {
                        desktopClient = new DesktopClientMock($"poll_{_CountDesktops++}", $"{Url.HostName}");
                        desktops.Add(url, desktopClient);
                    }
                    var config = desktopClient.LoadConfigAsync().Result;
                    return JsonConvert.SerializeObject(config);
                }
                if (url.Contains("/info"))
                {
                    DeviceClientMock deviceClient;
                    if (devices.TryGetValue(url, out deviceClient) == false)
                    {
                        deviceClient = new DeviceClientMock($"poll_{_CountDevices++}");
                        devices.Add(url, deviceClient);
                    }
                    IDeviceClient client = deviceClient;
                    return client.GetInfoAsync().Result;
                }
            }
            return null;
        }
    }
}
