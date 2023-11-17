namespace CastManager.ViewModels
{
    using System.ComponentModel;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using CastManager.CoreImpl;
    using CastManager.Core;

    public class OriginHomeTemplateMakerViewModel : ViewModelBase, IPage
    {
        readonly ITemplatesService _templatesService;

        public OriginHomeTemplateMakerViewModel(ITemplatesService templatesService)
        {
            this._templatesService = templatesService;

        }

        void IPage.OnActive()
        {
            _templatesService.CurrentConfig?.SetTatamiTablo(null);
        }
    }
}
