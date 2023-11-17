namespace CastManager.ViewModels
{
    using System.ComponentModel;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight;

    using CastManager.Models;
    using CastManager.Models.Desktops;
    using CastManager.Models.Device;
    using CastManager.Models.Extension;
    using CastManager.Templates;
    using CastManager.Core;
    using System.Diagnostics;

    public class TemplateViewModel : ViewModelBase, IPage
    {
        public IPage TemplateLeftPanel => CurrentPage.Left.Value;

        public IPage TemplateRightPanel => CurrentPage.Right.Value;

        public DesktopInfo SelectedDesktop
        {
            get => _selectedDesktop;
            set
            {
                if (value != null)
                {
                    Set(ref _selectedDesktop, value);
                    ForwardSwitchOnOrignPage();
                }
            }
        }
        public IList<DeviceData> Devices => _devicesService.Devices;

        public IList<DesktopInfo> Desktops => _desktopsService.Desktops;

        public TatamiTabloInfo SelectedTablo_VM1
        {
            get => CurrentConfig?.SelectedTablo;
            set
            {
                CurrentConfig?.SetTatamiTablo(value);
                if (value != null)
                {
                    CurrentConfig?.SetUrl(ImageSlot.A, value.Url);

                    _devicesService.UpdateCheckboxesForDevices(CurrentConfig);
                    ForwardSwitchOnOrignPage();
                }
            }
        }

        public TatamiTabloInfo SelectedTablo_MV2
        {
            get => null;
            set
            {
                if (value != null)
                {
                    CurrentConfig?.SetUrl(value.Url);
                    RaisePropertyChanged(nameof(SelectedDesktop));
                }
            }
        }

        public ITemplateConfig CurrentConfig => _templatesService.CurrentConfig;

        public TemplatePageNum CurentTemplatePageNum
        {
            get => _currentPage.PageNum;
            set
            {
                SetTemplatePage(value);
            }
        }

        public RelayCommand NavigateBackCommand => new(BackwardSwitchOnOrignPage);

        public RelayCommand SaveTemplateCommand => new RelayCommand(_templatesService.SaveConfigToDevice);

        private readonly TemplatePagesContainer templatePages = new();

        private TemplatePageItem _currentPage;

        private DesktopInfo _selectedDesktop;

        private TemplatePageItem CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                RaisePropertyChanged(nameof(TemplateLeftPanel));
                RaisePropertyChanged(nameof(TemplateRightPanel));
            }
        }

        private readonly ITemplatesService _templatesService;
        private readonly IDevicesService _devicesService;
        private readonly IDesktopsService _desktopsService;

        public TemplateViewModel(ITemplatesService templatesService, IDevicesService devicesService, IDesktopsService desktopsService)
        {
            this._templatesService = templatesService;
            this._devicesService = devicesService;
            this._desktopsService = desktopsService;

            _currentPage = templatePages.GetPage(TemplatePageNum.Orign);

            if (!IsInDesignMode)
            {
                templatesService.OnCurrentConfigChanged += curConfig =>
                {
                    var pageNum = curConfig.Type.ToEnum(_currentPage.PageNum);
                    SetTemplatePage(pageNum);

                    _templatesService.OnAssemblyTemplateDone += curConfig =>
                    {
                        SelectedDesktop?.DesktopConfig.Update();
                        RaisePropertyChanged(nameof(SelectedDesktop));
                        RaisePropertyChanged(nameof(CurrentConfig));
                    };

                    curConfig.OnTatamiTabloChanged += () =>
                    {
                        RaisePropertyChanged(nameof(SelectedTablo_VM1));
                        RaisePropertyChanged(nameof(SelectedTablo_MV2));
                    };
                };
            }
        }

        void IPage.OnActive()
        {
        }
        
        private void SetTemplatePage(TemplatePageNum pageNum)
        {
            if (templatePages.TryGetPageItem(pageNum, out TemplatePageItem pageItem))
            {
                var previousePage = CurrentPage;
                CurrentPage = pageItem;
                CurrentPage.OnActive(previousePage);

            }
            if (CurrentConfig != null)
            {
                var templateType = pageNum.ToEnum(CurrentConfig.Type);
                CurrentConfig.SetTamplateType(templateType);
                _devicesService.UpdateCheckboxesForDevices(CurrentConfig);
            }

            switch(pageNum)
            {
                case TemplatePageNum.Orign:
                    _selectedDesktop = null;
                    RaisePropertyChanged(nameof(SelectedDesktop));
                    break;
                case TemplatePageNum.Orign_SelectedDesktop:
                    SelectedTablo_VM1 = null;
                    break;
            }

            RaisePropertyChanged(nameof(CurentTemplatePageNum));
        }

        private void ForwardSwitchOnOrignPage()
        {
            /// minor FSM for origin view switch pages farward
            switch (CurrentPage.PageNum)
            {
                case TemplatePageNum.Orign:
                    SetTemplatePage(TemplatePageNum.Orign_SelectedDesktop);
                    break;
                case TemplatePageNum.Orign_SelectedDesktop:
                    SetTemplatePage(TemplatePageNum.Orign_SelectedTablo);
                    break;
            }
        }

        private void BackwardSwitchOnOrignPage()
        {
            /// minor FSM for origin view switch pages backward
            switch (CurrentPage.PageNum)
            {
                case TemplatePageNum.Orign_SelectedTablo:
                    SetTemplatePage(TemplatePageNum.Orign_SelectedDesktop);
                    break;
                case TemplatePageNum.Orign_SelectedDesktop:
                    SetTemplatePage(TemplatePageNum.Orign);
                    break;
            }
        }
    }
}
