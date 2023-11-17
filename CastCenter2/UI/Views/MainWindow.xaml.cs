
namespace CastManager.Views
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using CastManager.Core;
    using CastManager.Models.Core;
    using CastManager.Utilities;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Storyboard sbShowStatusPanel;
        Storyboard sbSpinnerGo;

        DefferedExecution defferedShowStatus = new();

        TimeSpan ShowPanelPeriod = TimeSpan.FromSeconds(0.2);

        BitmapImage spinGoImage = new BitmapImage();
        BitmapImage warningImage = new BitmapImage();
        BitmapImage commonOkImage = new BitmapImage();
        
        public MainWindow()
        {
            InitializeComponent();
            InitStoryBoard(ref sbShowStatusPanel, "sbShowStatusPanel");
            InitStoryBoard(ref sbSpinnerGo, "sbSpinnerGo");


            foreach (var anim in sbShowStatusPanel.Children)
            {
                anim.Duration = new Duration(ShowPanelPeriod);
            }

            var globalEvents = DependencyInjection.Get<IAppGlobalEvents>();

            globalEvents.OnStatusChanged += OnStatusChanged;

            spinGoImage.BeginInit();
            spinGoImage.UriSource = new Uri("/images/reset_w.png", UriKind.Relative);
            spinGoImage.EndInit();

            warningImage.BeginInit();
            warningImage.UriSource = new Uri("/images/icons/warning_error.png", UriKind.Relative);
            warningImage.EndInit();

            commonOkImage.BeginInit();
            commonOkImage.UriSource = new Uri("/images/icons/ok.png", UriKind.Relative);
            commonOkImage.EndInit();



        }

        private void InitStoryBoard(ref Storyboard sb, string name)
        {
            sb = Resources[name] as Storyboard;
            sb.Begin();
            sb.Pause();
        }

        private void OnStatusChanged(IAppStatus status)
        {
            defferedShowStatus.Exec(ShowStatusPanel, status, status.PendingDelay);
        }

        void SetIcon(ImageSource image)
        {
            m_RotateImage.Source = image;
        }

        void SetIcon(string imagePath)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(imagePath);
            bitmapImage.EndInit();
            m_RotateImage.Source = bitmapImage;
        }

        private void ShowStatusPanel(CancellationToken token, IAppStatus status)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                var isError = !string.IsNullOrEmpty(status.Error);
                var isMessage = !string.IsNullOrEmpty(status.Text);
                var isIcon = !string.IsNullOrEmpty(status.Icon);

                if (isError)
                {
                    m_StatusText.Content = status.Error;                    
                }
                else if (isMessage)
                {
                    m_StatusText.Content = status.Text;
                }
                else
                {
                    m_StatusText.Content = "";
                }

                if (status.SpinnerGo)
                {
                    SetIcon(spinGoImage);
                    sbSpinnerGo.Resume();
                }
                else
                {
                    if(isError)
                    {
                        SetIcon(warningImage);
                    }
                    else if(isIcon)
                    {
                        SetIcon(status.Icon);
                    }
                    else
                    {
                        SetIcon(commonOkImage);
                    }
                    sbSpinnerGo.Stop();
                    sbSpinnerGo.Begin();
                    sbSpinnerGo.Pause();
                }

                ShowStatusPanel();

                await Task.Delay(status.ShowDelay, token);
                HideStatusPanel();
                sbSpinnerGo.Stop();
                sbSpinnerGo.Begin();
                sbSpinnerGo.Pause();
            });
        }

        private void OnCompletedShowStatusPanel(object sender, EventArgs e)
        {
            sbSpinnerGo.Pause();
        }

        private void MinimiseButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void ShowStatusPanel()
        {
            if (sbShowStatusPanel.GetCurrentTime().TotalMilliseconds > 0)
            {
                return;
            }

            sbShowStatusPanel.AutoReverse = false;
            sbShowStatusPanel.Begin();
        }

        private void HideStatusPanel()
        {
            if (sbShowStatusPanel.GetCurrentTime().TotalMilliseconds < ShowPanelPeriod.TotalMilliseconds)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                sbShowStatusPanel.AutoReverse = true;
                sbShowStatusPanel.Begin();
                sbShowStatusPanel.Pause();
                sbShowStatusPanel.Seek(ShowPanelPeriod);
                sbShowStatusPanel.Resume();
            });
        }
    }
}
