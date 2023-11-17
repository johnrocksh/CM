namespace CastManager.ViewModels
{
    using CastManager.Core;

    class TemplateViewModelsFactory
    {
        public Lazy<IPage> OneImage { get; private set; } 
        public Lazy<IPage> Horizontal { get; private set; }
        public Lazy<IPage> Vertical { get; private set; } 
        public Lazy<IPage> GridWnd { get; private set; }
        public Lazy<IPage> OriginMaker { get; private set; }
        public Lazy<IPage> OriginMakerTatamiSelection { get; private set; } 
        public Lazy<IPage> OriginMakerLinkToTablo { get; private set; } 
        public Lazy<IPage> TemplateMaker { get; private set; }
        public Lazy<IPage> OneImageMaker { get; private set; }

        public TemplateViewModelsFactory()
        {
            OneImage = new Lazy<IPage>(DependencyInjection.Get<OneImageTemplateViewModel>());
            Horizontal = new Lazy<IPage>(DependencyInjection.Get<HorizontalTemplateViewModel>());
            Vertical = new Lazy<IPage>(DependencyInjection.Get<VerticalTemplateViewModel>());
            GridWnd = new Lazy<IPage>(DependencyInjection.Get<GridWndTemplateViewModel>());
            OriginMaker = new Lazy<IPage>(DependencyInjection.Get<OriginHomeTemplateMakerViewModel>());
            OriginMakerTatamiSelection = new Lazy<IPage>(DependencyInjection.Get<OriginMakerTatamiSelectionViewModel>());
            OriginMakerLinkToTablo = new Lazy<IPage>(DependencyInjection.Get<OriginMakerLinkToTabloViewModel>());
            TemplateMaker = new Lazy<IPage>(DependencyInjection.Get<TemplateMakerViewModel>());
            OneImageMaker = new Lazy<IPage>(DependencyInjection.Get<OneImageTemplateMakerViewModel>());
        }
    }
}
