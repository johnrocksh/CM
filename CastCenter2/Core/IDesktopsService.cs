namespace CastManager.Core
{
    using CastManager.Models.Desktops;
    using CastManager.Models.Core;

    public interface IDesktopsService
    {
        Action<ItemAction, DesktopInfo> OnDesktopAction { get; set; }

        IList<DesktopInfo> Desktops { get; }

    }
}
