namespace CastManager.CoreImpl
{
    using CastManager.Core;
    using CastManager.Models.Core;

    public class AppGlobalEvents : IAppGlobalEvents
    {
        public Action OnMainWindowLoaded { get; set; }
        public Action<IAppStatus> OnStatusChanged { get; set; }
    }
}
