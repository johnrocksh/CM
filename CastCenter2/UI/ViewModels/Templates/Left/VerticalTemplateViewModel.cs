namespace CastManager.ViewModels
{
    using System.ComponentModel;
    using GalaSoft.MvvmLight;

    using CastManager.Templates;
    using CastManager.CoreImpl;
    using CastManager.Core;

    public class VerticalTemplateViewModel : ViewModelBase, IPage
    {
        readonly ITemplatesService _templatesService;

        public ITemplateConfig CurrentConfig => _templatesService.CurrentConfig;

        /// <summary>
        /// Contains the value of the button pressed when selecting a template type in Template menu
        /// </summary>    
        public ImageSlot ImageSlot
        {
            get => CurrentConfig.SelectedImageSlot;
            set
            {
                CurrentConfig.SelectedImageSlot = value;
                CurrentConfig?.SetTatamiTablo(null);
                RaisePropertyChanged(nameof(ImageSlot));
            }
        }

        public VerticalTemplateViewModel(ITemplatesService templatesService)
        {
            this._templatesService = templatesService;
            templatesService.OnCurrentConfigChanged += curConfig =>
            {
                curConfig.OnImageSlotChanged += (s, d) => RaisePropertyChanged(nameof(CurrentConfig));
            };
        }

        void IPage.OnActive()
        {
            CurrentConfig?.SetTatamiTablo(null);
        }
    }
}
