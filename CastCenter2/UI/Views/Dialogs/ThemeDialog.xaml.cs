namespace CastManager.Views
{
    using System.Windows;
    using System.Windows.Input;

    public partial class ThemeDialog : Window
    {
        public ThemeDialog()
        {
            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void CloseWindowClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
