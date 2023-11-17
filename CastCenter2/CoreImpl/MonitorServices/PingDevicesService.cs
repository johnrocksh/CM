namespace CastManager.Models.Device
{
    using CastManager.Logger;

    /// <summary>
    /// Ping all Devices if they are alive
    /// </summary>
    internal class PingDevicesService
    {
        public Func<IList<DeviceData>> Devices { get; set; }

        public Action<DeviceData> OnDeviceLost{ get; set; }

        private readonly CancellationTokenSource _cts = new();

        public void Start(int timeout = 4000)
        {            
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Task.Delay(timeout, _cts.Token).Wait();

                        if (_cts.Token.IsCancellationRequested)
                            break;

                        var devices = Devices?.Invoke();
                        foreach(var d in devices)
                        {
                            if (!d.Client.IsAlive())
                            {
                                OnDeviceLost?.Invoke(d);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteException(ex.Message);
                    }
                }
            });
        }
        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
