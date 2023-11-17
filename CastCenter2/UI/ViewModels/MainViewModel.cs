namespace CastManager.ViewModels
{
    using System.Windows;
    using System.Collections.Generic;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using CastManager.Models.Desktops;
    using CastManager.Models.Device;
    using CastManager.Core;
    using CastManager.Views;

    public class MainViewModel : ViewModelBase
    {
        public IPage CurrentPage { get => _currentPage; set => Set(ref _currentPage, value); }

        /// <summary>
        /// Navigates to the specified page based on the provided page enum.
        /// </summary>
        /// <param name="num">The page enum page.</param>
        public RelayCommand<PageNum> NavigateCommand => new(num =>
        {            
            if (pages.TryGetValue(num, out Lazy<IPage> page))
            {
                CurrentPage = page.Value;
                CurrentPage.OnActive();
            }
        });

        public DeviceData SelectedDevice
        {
            get => null;
            set
            {
                if(value != null)
                {
                    value.Info.IsChecked ^= true;
                    devicesService.OnDeviceSelected(value);
                    RaisePropertyChanged(nameof(SelectedDevice));
                }
            }
        }

        public IList<DeviceData> Devices => devicesService.Devices;

        public IList<DesktopInfo> Desktops => desktopsService.Desktops;

        public RelayCommand RefreshDevicesCommand => new(lookupService.StartLookupDevices);

        public RelayCommand RefreshDesktopsCommand => new(lookupService.StartLookupDesktops);

        public RelayCommand ThemeBtnCommand => new RelayCommand(DialogsFactory.ThemeDialog.ToggleVisible);

        public RelayCommand AddHostForLookupCommand => new RelayCommand(DialogsFactory.AddHostDialog.ToggleVisible);

        public RelayCommand CloseAppCommand => new RelayCommand(Application.Current.Shutdown);

        private readonly Dictionary<PageNum, Lazy<IPage>> pages;
        private readonly ILookupService lookupService;
        private readonly IMonitorService monitorService;
        private readonly IDevicesService devicesService;
        private readonly IDesktopsService desktopsService;
        private readonly IAppGlobalEvents appGlobalEvents;
        

        private IPage _currentPage;

        public MainViewModel()
        {
            if (!IsInDesignMode)
            {
                this.lookupService = DependencyInjection.Get<ILookupService>();
                this.monitorService = DependencyInjection.Get<IMonitorService>();
                this.devicesService = DependencyInjection.Get<IDevicesService>();
                this.desktopsService = DependencyInjection.Get<IDesktopsService>();
                this.appGlobalEvents = DependencyInjection.Get<IAppGlobalEvents>();

                this.pages = new()
                {
                    {PageNum.TemplatePage,new Lazy<IPage>( () => DependencyInjection.Get<TemplateViewModel>() ) },
                    {PageNum.SettingsPage,new Lazy<IPage>( () => DependencyInjection.Get<SettingsViewModel>()  ) }
                };

                this.desktopsService.OnDesktopAction += (action, desktopInfo) =>
                {                    
                    RaisePropertyChanged(nameof(Desktops));
                };

                this.devicesService.OnDevicePropertyChanged += (action, deviceData) =>
                {
                    RaisePropertyChanged(nameof(Devices));
                };

                Task.Run(() =>
                {
                    Task.Delay(1000).Wait();
                    this.lookupService.Start();
                    this.monitorService.Start();
                });

            }

            this.NavigateCommand.Execute(PageNum.TemplatePage);

            appGlobalEvents.OnMainWindowLoaded?.Invoke();
        }


    }
}
