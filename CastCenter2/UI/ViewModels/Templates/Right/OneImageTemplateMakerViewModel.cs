namespace CastManager.ViewModels
{
    using System.ComponentModel;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    using CastManager.Helper;
    using CastManager.Templates;
    using CastManager.CoreImpl;
    using CastManager.Core;

    public class OneImageTemplateMakerViewModel : ViewModelBase, IPage
    {
        readonly ITemplatesService _templatesService;
        public RelayCommand OpenImageCommand { get; set; }

        public OneImageTemplateMakerViewModel(ITemplatesService templatesService)
        {
            this._templatesService = templatesService;

            OpenImageCommand = new RelayCommand(() =>
            {
                var url = UIHelper.OpenImageFileDialog();
                _templatesService.CurrentConfig?.SetUrl(ImageSlot.A, url);
            });
        }
        void IPage.OnActive()
        {
        }
    }
}
