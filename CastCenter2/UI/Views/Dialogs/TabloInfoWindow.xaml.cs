
namespace CastManager.View.Dialogs
{
    using System.Windows;
    /// <summary>
    /// Interaction logic for PopUpTabloWnd.xaml
    /// </summary>
    public partial class InfoWnd : Window
    {
        private static InfoWnd instance;
        public static InfoWnd getInstance()
        {
            if (instance == null)
                instance = new InfoWnd();
            //if window was hide then show it
            if (!instance.ShowActivated) instance.Show();
            return instance;
        }

        public InfoWnd()
        {
            InitializeComponent();
           // this.DataContext = MainWindow.vm;
            this.Topmost = true;
        }
    }
}
