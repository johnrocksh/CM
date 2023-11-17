using CastManager.Client.Desktop;

namespace CastManager.Models.Desktops
{
    /// <summary>
    /// Ping all Desktops if they are alive
    /// </summary>
    internal class PingDesktopsService
    {
        public Func<IList<DesktopInfo>> Desktops { get; set; }
        public Action<DesktopInfo> OnDesktopUpdated { get; set; }
        public Action<DesktopInfo> OnDesktopNotFound { get; set; }

        private readonly CancellationTokenSource _cts = new();

        public void Start(int timeout = 4000)
        {
            Task.Run( async () =>
            {
                while (true)
                {
                    try
                    {
                        Task.Delay(timeout, _cts.Token).Wait();

                        if (_cts.Token.IsCancellationRequested)
                            break;

                        var destopInfos = Desktops?.Invoke();
                        foreach (var info in destopInfos)
                        {
                            var status = await this.GetStatusAsync(info).ConfigureAwait(false);
                            switch (status)
                            {
                                case DesktopStatus.BecomeOnStandby:
                                case DesktopStatus.BecomeOnActive:
                                case DesktopStatus.Changed:
                                    OnDesktopUpdated?.Invoke(info);
                                    break;
                                case DesktopStatus.NA:
                                    OnDesktopNotFound?.Invoke(info);
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        private async Task<DesktopStatus> GetStatusAsync(DesktopInfo desktopInfo)
        {
            var client = desktopInfo.DesktopClient;
            var oldConfig = desktopInfo.DesktopConfig;

            var newConfig = await client.LoadConfigAsync().ConfigureAwait(false);

            if (newConfig == null)
            {
                return DesktopStatus.NA;
            }
            if (oldConfig.IsEqualTo(newConfig))
            {
                return oldConfig.IsActive ? DesktopStatus.Active : DesktopStatus.Standby;
            }

            desktopInfo.DesktopConfig = newConfig;

            if (newConfig.IsActive && oldConfig.IsPassive)
            {
                return DesktopStatus.BecomeOnActive;
            }
            if (oldConfig.IsActive && newConfig.IsPassive)
            {
                return DesktopStatus.BecomeOnStandby;
            }
            return DesktopStatus.Changed;
        }
    }
}
