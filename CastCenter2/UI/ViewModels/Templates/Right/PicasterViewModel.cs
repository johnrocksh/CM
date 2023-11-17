namespace CastManager.ViewModels
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Collections.ObjectModel;

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using CastManager.Helper;
    using CastManager.Models;
    using CastManager.Extentions;
    using CastManager.Core;
    using CastManager.StringsResources;
    using CastManager.Logger;
    using System.Diagnostics;
    using CastManager.Models.Core;
    using Windows.Devices.WiFi;

    public class PicasterViewModel : ViewModelBase
    {
        CancellationTokenSource _cts = new();
        /// <summary>
        /// The Picaster patcher
        /// </summary>
        private IPicasterPatcher picasterPatcher;

        #region Picaster
        /// <summary>
        /// Picaster patch status
        /// </summary>
        enum PiPacherStatus
        {
            Searching,
            DeviceFound,
            NotFound,
            Finished,
            Registration,
            Connecting
        }

        /// <summary>
        /// Flag to show in progress animation
        /// </summary>
        public bool SpinnerGo => picasterStatus == PiPacherStatus.Searching ||
                                 picasterStatus == PiPacherStatus.Registration ||
                                 picasterStatus == PiPacherStatus.Connecting;

        private bool IsErrorStatus => picasterStatus == PiPacherStatus.NotFound;

        /// <summary>
        /// Current picaster patch step status
        /// </summary>
        private PiPacherStatus picasterStatus;

        /// <summary>
        /// Enable or disable Patch (Ok) button during searching
        /// </summary>        
        public bool PatchCasterButtonIsEnabled => picasterStatus != PiPacherStatus.Searching;

        /// <summary>
        /// Selected Wifi Item from Picaster Wifi Networks List
        /// </summary>
        private PicasterWifiItem _selectedWifi;

        /// <summary>
        /// Selected Wifi Item from Picaster Wifi Networks List
        /// </summary>
        public PicasterWifiItem SelectedWifi
        {
            get => _selectedWifi;
            set
            {
                if (_selectedWifi != value && value != null)
                {
                    Set(ref _selectedWifi, value);
                }
            }
        }

        /// <summary>
        /// WiFi network name        
        /// </summary>
        private string _currentEssid = Network.GetCurrentWifiNetworkName();

        /// <summary>
        /// WiFi network name, that binding at UI TextBox
        /// </summary>
        public string CurrentEssid
        {
            get => _currentEssid;
            set => Set(ref _currentEssid, value);
        }

        /// <summary>
        /// Picaster network wifi list
        /// </summary>
        public ObservableCollection<PicasterWifiItem> PicasterList { get; private set; } = new();

        /// <summary>
        /// Flag to show/hide wifi button
        /// </summary>
        public bool IsAvailable => picasterPatcher.IsWifiAdapter;

        #endregion

        private IAppGlobalEvents appGlobalEvents;

        /// <summary>
        /// Button command when we open picaster panel 
        /// </summary>
        public RelayCommand WifiButtonCommand => new(async () =>
        {
            CurrentEssid = Network.GetCurrentWifiNetworkName();
            await UpdatePicasterListAsync().ConfigureAwait(false);
        });

        public RelayCommand BackButtonCommand => new RelayCommand(() =>
        {
            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                ShowDelay = TimeSpan.FromSeconds(0),
                PendingDelay = TimeSpan.FromSeconds(1),
            });
        });

        /// <summary>
        /// The command when we push OK button to patch the picaster device
        /// </summary>
        public RelayCommand<PasswordBox> PatchPanelButtonCommand => new(async pwd =>
        {
            await OnPicasterPatchAsync(pwd).ConfigureAwait(false);
        });

        /// <summary>
        /// Constructor
        /// </summary>
        public PicasterViewModel()
        {
            this.picasterPatcher = DependencyInjection.Get<IPicasterPatcher>();
            this.appGlobalEvents = DependencyInjection.Get<IAppGlobalEvents>();
        }

        /// <summary>
        /// Set Picaster Status
        /// </summary>
        /// <param name="status"></param>
        private void SetStatus(PiPacherStatus status)
        {
            this.picasterStatus = status;
            var statusText = Strings.ById($"{status}_Str");
            RaisePropertyChanged(nameof(this.PatchCasterButtonIsEnabled));

            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                SpinnerGo = this.SpinnerGo,
                Text = IsErrorStatus ? null : statusText,
                Error = IsErrorStatus ? statusText : null
            });
        }

        /// <summary>
        /// Update Picaster List 
        /// </summary>
        private async Task UpdatePicasterListAsync()
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                try
                {
                    SetStatus(PiPacherStatus.Searching);

                    var wifiItems = await picasterPatcher.GetPicasterWifiItemsAsync();

                    if (wifiItems != null && wifiItems.Count() > 0)
                    {
                        SetStatus(PiPacherStatus.DeviceFound);
                        PicasterList.Clear();
                        PicasterList.AddRange(wifiItems);
                        RaisePropertyChanged(nameof(PicasterList));
                    }
                    else
                    {
                        SetStatus(PiPacherStatus.NotFound);
                    }
                }
                catch (Exception e)
                {
                    Logger.WriteException(e.Message);
                }
            }).ConfigureAwait(false);
        }

        void ShowError(string message)
        {
            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                Error = message
            });
        }

        /// <summary>
        /// This method connects to the selected access point  
        /// (at this time disconnecting from the current Wi-Fi network and the Internet connection is suspended) 
        /// then registers the configuration of the current Wi-Fi network, - ssid and password.
        /// the picaster disappears in the panel and appears in the list of devices .. and is ready for use
        /// </summary>
        /// <param name="passwordBox">password for the current Wi-Fi network ..</param>
        /// <returns></returns>
        private async Task<bool> OnPicasterPatchAsync(PasswordBox passwordBox)
        {          
                if (PicasterList.Count == 0)
                {
                    ShowError(Strings.NotFound);
                    return false;
                }
                else if (SelectedWifi == null || string.IsNullOrEmpty(SelectedWifi.Ssid))
                {
                    ShowError(Strings.SelectNetwork);
                    return false;
                }
                else if (string.IsNullOrEmpty(passwordBox.Password))
                {
                    ShowError(Strings.EnterPassword);
                    return false;
                }
                SetStatus(PiPacherStatus.Connecting);

                var connectionResult = await this.picasterPatcher
                    .ConnectToPicasterWifiNetworkAsync(_selectedWifi.Ssid)
                    .ConfigureAwait(false);             
                int timeout = 1000;
                await Task.Run(() =>
                {
                    Debug.WriteLine("start timeout");
                    Task.Delay(timeout, _cts.Token).Wait();
                    Debug.WriteLine("stop timeout");
                });

            return await this
                .WriteWifiSettingsToDeviceAsync(passwordBox.Password); 
}

    /// <summary>
    /// Send network name and password to device, that device should be reconnected to this wifi nettowork.
    /// </summary>
    /// <param name="password">user setup the password for wifi</param>
    /// <returns></returns>
    async Task<bool> WriteWifiSettingsToDeviceAsync(string password)
        {
            SetStatus(PiPacherStatus.Registration);

            var writeResult = await picasterPatcher.WriteWifiSettingsToDeviceAsync(CurrentEssid, password)
                .ConfigureAwait(false);


            if (writeResult)
            {
                // after registration is complete, start a search to make sure the device is registered
                await UpdatePicasterListAsync().ConfigureAwait(false);
            }
            else
            {
                SetStatus(PiPacherStatus.Finished);
                ShowError(Strings.ById("WriteWifiError_Str"));
            }
            return writeResult;
        }
    }
}
