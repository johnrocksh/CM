namespace CastManager.ViewModels
{
    using CastManager.Core;
    using CastManager.CoreImpl;
    using CastManager.Templates;
    using GalaSoft.MvvmLight;
    using System.ComponentModel;

    public class OneImageTemplateViewModel : ViewModelBase, IPage
    {
        readonly ITemplatesService _templatesService;
        public ITemplateConfig CurrentConfig => _templatesService.CurrentConfig;

        public OneImageTemplateViewModel(ITemplatesService templatesService)
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
