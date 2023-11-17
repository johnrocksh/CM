namespace CastManager.Client.Desktop
{
    using CastManager.Models.Desktops;
    using System.Threading.Tasks;

    public interface IDesktopClient
    {
        Task<DesktopConfig> LoadConfigAsync();

        void Cancel();
    }
}
