namespace CastManager.Client.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IDesktopsDiscovery
    {
        Task<bool> FindDesktopsAsync(Action<IDesktopClient> onDiscovered);

        void Cancel();
    }
}
