namespace CastManager.Core
{
    using CastManager.Models.Core;
    public interface IAppGlobalEvents
    {
        Action OnMainWindowLoaded { get; set; }

        Action<IAppStatus> OnStatusChanged { get; set; }
    }
}
