namespace CastManager.Views
{
    using System.Windows;
    using System.Windows.Controls;    
    using System.Windows.Media.Animation;

    public partial class PicasterControl : UserControl
    {
        public Action Completed;
        public PicasterControl()
        {
            InitializeComponent();           
        }

        public void Back_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["sbHidePatchPanel"] as Storyboard;
            sb.Completed += delegate { Visibility = Visibility.Hidden; Completed?.Invoke(); };
            sb.Begin(PatchPanel);
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            Storyboard sb = Resources["sbShowPatchPanel"] as Storyboard;
            sb.Begin(PatchPanel);
        }

    }
}
