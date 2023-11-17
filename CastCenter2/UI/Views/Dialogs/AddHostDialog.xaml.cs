namespace CastManager.Views
{
    using System.Windows;
    using System.Windows.Input;

    public partial class AddHostDialog : Window
    {
        public AddHostDialog()
        {
            InitializeComponent();
            this.Topmost = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void OnCloseDialog(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
