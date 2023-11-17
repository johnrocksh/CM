namespace CastManager.Models
{
    using CastManager.ViewModels;
    using CastManager.Templates;
    using CastManager.Core;

    public class TemplatePagesContainer
    {
        /// <summary>
        /// All lazy models for `Templates` views(UserControls)
        /// </summary>
        readonly TemplateViewModelsFactory models = new();

        /// <summary>
        /// Serves to connect the TemplatePageNum and the ViewModel...
        /// while UserControls(views) are connected with ViewModel at DataTemplate in TemplatePage.xaml
        /// </summary>
        readonly Dictionary<TemplatePageNum, TemplatePageItem> pages = new();

        public TemplatePagesContainer()
        {           
            AddPage(new TemplatePageItem()
            {
                PageNum = TemplatePageNum.Orign,
                Left = models.OneImage,
                Right = models.OriginMaker
            });
            AddPage(new TemplatePageItem()
            {
                PageNum = TemplatePageNum.Horizontal,
                Left = models.Horizontal,
                Right = models.TemplateMaker
            });
            AddPage(new TemplatePageItem()
            {
                PageNum = TemplatePageNum.Vertical,
                Left = models.Vertical,
                Right = models.TemplateMaker
            });
            AddPage(new TemplatePageItem()
            {
                PageNum = TemplatePageNum.Grid,
                Left = models.GridWnd,
                Right = models.TemplateMaker
            });
            AddPage(new TemplatePageItem()
            {
                PageNum = TemplatePageNum.ImageAdv,
                Left = models.OneImage,
                Right = models.OneImageMaker
            });
        }

        public bool TryGetPageItem(TemplatePageNum key, out TemplatePageItem value) => pages.TryGetValue(key, out value);
        public TemplatePageItem GetPage(TemplatePageNum key) => pages[key];

        private void AddPage(TemplatePageItem p) => pages.Add(p.PageNum, p);            
    }
}
