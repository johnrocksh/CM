namespace CastManager.Models
{
    using CastManager.Templates;
    using CastManager.ViewModels;

    public class TemplatePageItem
    {
        public TemplatePageNum PageNum;
        public Lazy<IPage> Left;
        public Lazy<IPage> Right;
        public void OnActive(TemplatePageItem previousePageItem)
        {
            if (previousePageItem == null || previousePageItem.Left != Left)
            {
                Left?.Value?.OnActive();
            }
            if (previousePageItem == null || previousePageItem.Right != Right)
            {
                Right?.Value?.OnActive();
            }            
        }
    }
}
