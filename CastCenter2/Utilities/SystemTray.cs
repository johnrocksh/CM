namespace CastManager.Utilities
{
    using CastManager.Properties;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Forms = System.Windows.Forms;

    /// <summary>
    /// SystemTray  Allows the application to work with System Tray
    /// </summary>
    public class SystemTray
    {
        public Forms.NotifyIcon _notifyIcon;
        public Window _mainWindow;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notifyIcon"></param>
        public SystemTray(Forms.NotifyIcon notifyIcon)
        {

            _notifyIcon = notifyIcon;
        }

        /// <summary>
        /// Put Application To System Tray
        /// </summary>
        /// <param name="_notifyIcon"></param>
        public void PutApplicationToSystemTray(Forms.NotifyIcon _notifyIcon)
        {
            String CastManager_Str = Application.Current.FindResource("CastManager_Str").ToString();
            _notifyIcon.Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/images/LOGO.ico")).Stream);           
            _notifyIcon.Text = CastManager_Str;
            _notifyIcon.Click += NotifyIcon_Click;
            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.Visible = true;
        }
       
        /// <summary>
        /// 
        /// </summary>
        private void OnCloseProgramClicked(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void NotifyIcon_Click(object sender, EventArgs e)
        {
            var w = Window.GetWindow(App.Current.MainWindow) as Window;
            w.WindowState = WindowState.Normal;
            w.Activate();
        }

    }
}
