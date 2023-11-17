namespace CastManager.Client
{
    using CastManager.Client.Desktop;
    using CastManager.Client.Device;
    using CastManager.Client.Poll;

    public class ClientsFactoryMock : IClientsFactory
    {
        static int deviceCount = 0;
        static int desktopsCount = 0;

        public Task LookupActiveLocalsAsync(Action<string> onActiveLocalIPFound)
        {
            onActiveLocalIPFound?.Invoke("127.0.0.1");
            onActiveLocalIPFound?.Invoke("127.0.0.2");
            //onActiveLocalIPFound?.Invoke("127.0.0.2");
            //onActiveLocalIPFound?.Invoke("127.0.0.3");
            //onActiveLocalIPFound?.Invoke("127.0.0.4");
            return Task.CompletedTask;
        }

        IDesktopClient IClientsFactory.GetDesktopClient(string ip)
        {
            return new DesktopClientMock($"{desktopsCount++}", ip);
        }

        IDeviceClient IClientsFactory.GetDeviceClient(string address, ushort port)
        {
            return new DeviceClientMock($"{deviceCount++}");
        }

        IPollClient IClientsFactory.GetPollClient(TimeSpan timeout, CancellationTokenSource cs)
        {
            return new PollClientMock(timeout, cs);
        }
    }
}
