namespace CastManager.ViewModels
{
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using CastManager.Core;
    using CastManager.Models.Core;
    using CastManager.StringsResources;
    using System.Net;

    public class AddHostDialogViewModel : ViewModelBase
    {
        public string HostText { get; set; }

        public RelayCommand ConfirmCommand => new(OnConfirmCommand);

        private readonly IAppGlobalEvents appEvents;
        private readonly ILookupService lookupService;

        private enum AddHostStatus
        {
            HostAddedToLookupList,
            HostAlreadyPresentAtLookupList,
            HostFormatException
        }

        private AddHostStatus _hostStaus;
        private bool IsErrorStatus => _hostStaus != AddHostStatus.HostAddedToLookupList;

        public AddHostDialogViewModel()
        {
            appEvents = DependencyInjection.Get<IAppGlobalEvents>();
            lookupService = DependencyInjection.Get<ILookupService>();
        }

        public void OnConfirmCommand()
        {
            var isHostValidFormat = IsValidHostFormat(HostText);
            var isIpValidFormat = IsValidIpFormat(HostText);

            if (isHostValidFormat || isIpValidFormat)
            {
                if (lookupService.AddIpAddreess(HostText))
                {
                    SetStatus(AddHostStatus.HostAddedToLookupList);
                }
                else
                {
                    SetStatus(AddHostStatus.HostAlreadyPresentAtLookupList);
                }
            }
            else
            {
                SetStatus(AddHostStatus.HostFormatException);
            }
        }

        private static bool IsValidHostFormat(string host)
        {
            try
            {
                var _ = new Uri(host);
                return true;
            }
            catch 
            {
            }
            return false;
        }

        private static bool IsValidIpFormat(string ip)
        {
            return IPAddress.TryParse(ip, out var _) && ip.Split('.').Length == 4;
        }

        private void SetStatus(AddHostStatus status)
        {
            _hostStaus = status;

            appEvents.OnStatusChanged(new AppStatus()
            {
                Error = IsErrorStatus ? Strings.ById($"{status}_Str") : null,
                Text = !IsErrorStatus ? Strings.ById($"{status}_Str") : null,
                ShowDelay = TimeSpan.FromSeconds(5)
            });
        }
    }
}

