namespace CastManager.ViewModels
{
    using GalaSoft.MvvmLight;
    using CastManager.Models.Device;
    using GalaSoft.MvvmLight.Command;
    using CastManager.Core;
    using Newtonsoft.Json;
    using CastManager.StringsResources;
    using CastManager.Logger;
    using CastManager.Models.Core;

    public class SettingsViewModel : ViewModelBase, IPage
    {
        /// <summary>
        /// Current settings status
        /// </summary>
        private SettingsStatus settingsStatus;

        enum SettingsStatus
        {
            None,
            SavingConfig,
            SavedConfigDone,
            SavedConfigFailed,
            SetDeviceToAccessPointMode,
            Finished,
            NoneDeviceSelected
        }

        /// <summary>
        /// Is in progress indicator 
        /// </summary>
        public bool SpinnerGo =>
            settingsStatus == SettingsStatus.SetDeviceToAccessPointMode ||
            settingsStatus == SettingsStatus.SavingConfig;

        private bool IsErrorStatus =>
            settingsStatus == SettingsStatus.SavedConfigFailed ||
            settingsStatus == SettingsStatus.NoneDeviceSelected;

        private bool isUrlTextBoxEnabled;
        public bool IsUrlTextBoxEnabled
        {
            get => isUrlTextBoxEnabled;
            set => Set(ref isUrlTextBoxEnabled, value);
        }

        //This property Binded to Left Panel (ListView)
        private DeviceData _selectedDevice;
        public DeviceData SelectedDevice
        {
            get => _selectedDevice;
            set => Set(ref _selectedDevice, value);
        }

        public RelayCommand AddNewConfigBtnCommand => new(AddNewConfigBtnCmd);
        public RelayCommand EditUrlButtonCommand => new(EditUrlButtonCmd);
        public RelayCommand ResetPicasterCommand => new(DeviceResetToAccessPointModeCmd);
        public RelayCommand DeleteConfigCommand => new(DeleteConfigCmd);
        public RelayCommand SaveConfigCommand => new(SaveConfigCmd);

        IAppGlobalEvents appGlobalEvents;

        public SettingsViewModel(ITemplatesService templatesService, IAppGlobalEvents appGlobalEvents)
        {
            templatesService.OnAssemblyTemplateDone += config =>
            {
                SelectedDevice?.Config.Update();
                RaisePropertyChanged(nameof(SelectedDevice));
            };

            this.appGlobalEvents = appGlobalEvents;
        }

        /// <summary>
        /// Add new empty Device config in Settings mode by pressing "+" button 
        /// </summary>        
        private void AddNewConfigBtnCmd()
        {
            if (SelectedDevice == null)
            {
                SetStatus(SettingsStatus.NoneDeviceSelected);
                return;
            }

            SelectedDevice.Config.AddNewDefaultConfig("");
            RaisePropertyChanged(nameof(SelectedDevice.Config.HostConfigs));
        }

        /// <summary>
        /// Enable url editing mode 
        /// </summary>
        private void EditUrlButtonCmd()
        {
            IsUrlTextBoxEnabled ^= true;
        }

        /// <summary>
        /// Reset device to Access Point
        /// </summary>
        private async void DeviceResetToAccessPointModeCmd()
        {

            if (SelectedDevice == null)
            {
                SetStatus(SettingsStatus.NoneDeviceSelected);
                return;
            }

            SetStatus(SettingsStatus.SetDeviceToAccessPointMode);
            await SelectedDevice.Client.WifiAccessPointMode(null);
            SetStatus(SettingsStatus.Finished);
        }

        /// <summary>
        /// Delete selected Config
        /// </summary>
        private void DeleteConfigCmd()
        {
            if (SelectedDevice == null)
            {
                SetStatus(SettingsStatus.NoneDeviceSelected);
                return;
            }

            SelectedDevice.Config.RemoveMarkedConfigs();
            RaisePropertyChanged(nameof(SelectedDevice));
        }

        /// <summary>
        /// SaveConfigCmd : Save current configuration on Device (RusberyPi or Windows)
        /// </summary>
        private async void SaveConfigCmd()
        {
            if (SelectedDevice == null)
            {
                SetStatus(SettingsStatus.NoneDeviceSelected);
                return;
            }

            SetStatus(SettingsStatus.SavingConfig);

            try
            {
                await SelectedDevice.UploadConfigAsync().ConfigureAwait(false);
                SetStatus(SettingsStatus.SavedConfigDone);
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
                SetStatus(SettingsStatus.SavedConfigFailed);
            }
        }

        /// <summary>
        /// Set Settings Status
        /// </summary>
        /// <param name="status"></param>
        private void SetStatus(SettingsStatus status)
        {
            settingsStatus = status;

            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                SpinnerGo = this.SpinnerGo,
                Error = IsErrorStatus ? Strings.ById($"{status}_Str") : null,
                Text = !IsErrorStatus ? Strings.ById($"{status}_Str") : null,
                ShowDelay = TimeSpan.FromSeconds(5)
            });
        }

        void IPage.OnActive()
        {
        }
    }
}
