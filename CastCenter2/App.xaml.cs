namespace CastManager
{

    using System.Windows;

    using NotifyIcon = System.Windows.Forms.NotifyIcon;

    using CastManager.Utilities;
    using CastManager.Core;

    public partial class App : System.Windows.Application
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly SystemTray _systemTray;

        public App()
        {
            try
            {
                _notifyIcon = new NotifyIcon();
                _systemTray = new SystemTray(_notifyIcon);

                DependencyInjection.Init();
                DependencyInjection.Instantiate<ITemplateHttpServer>();
            }
            catch (Exception ex)
            {
                Logger.Logger.WriteException(ex.Message);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _systemTray.PutApplicationToSystemTray(_notifyIcon);
            }
            catch (Exception ex)
            {
                Logger.Logger.WriteException(ex.Message);
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DependencyInjection.Exit();
            _notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
