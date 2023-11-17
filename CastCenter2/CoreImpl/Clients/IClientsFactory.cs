namespace CastManager.Client
{
    using CastManager.Client.Desktop;
    using CastManager.Client.Device;
    using CastManager.Client.Poll;

    public interface IClientsFactory
    {
        IDeviceClient GetDeviceClient(string address, ushort port);
        IDesktopClient GetDesktopClient(string ip);
        IPollClient GetPollClient(TimeSpan timeout, CancellationTokenSource cs);

        Task LookupActiveLocalsAsync(Action<string> onActiveLocalIPFound);
    }
}
