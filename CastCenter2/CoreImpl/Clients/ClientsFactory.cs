namespace CastManager.Client
{
    using System;
    using System.Threading.Tasks;
    using CastManager.Client.Desktop;
    using CastManager.Client.Device;
    using CastManager.Client.Poll;

    internal class ClientsFactory : IClientsFactory
    {
        public Task LookupActiveLocalsAsync(Action<string> onActiveLocalIPFound) => Helper.Network.LookupActiveLocalsAsync(onActiveLocalIPFound);

        IDesktopClient IClientsFactory.GetDesktopClient(string ip)
        {
            return new DesktopClient(ip);
        }

        IDeviceClient IClientsFactory.GetDeviceClient(string address, ushort port)
        {
            return new DeviceClient(address, port);
        }

        IPollClient IClientsFactory.GetPollClient(TimeSpan timeout, CancellationTokenSource cs)
        {
            return new PollClient(timeout, cs);
        }
    }
}
