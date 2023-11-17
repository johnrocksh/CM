namespace CastManager.Views
{
    using System.Windows;
    using System.Windows.Controls;
    /// <summary>
    /// Interaction logic for HomeControl.xaml
    /// </summary>
    public partial class HomeControl : UserControl
    {       
        public HomeControl()
        {                       
            InitializeComponent();          
        }

        public void Wifi_Click(object sender, EventArgs e)
        {
            TabloContainer.Visibility = Visibility.Collapsed;
            PicasterContainer.Show();
            PicasterContainer.Completed += delegate { TabloContainer.Visibility = Visibility.Visible; };
        }
    }
}






