namespace CastManager.Templates
{
    using System.Windows.Media;
    using CastManager.Models.Desktops;
    
    public interface ITemplateConfig
    {
        TemplateType Type { get; }
        ImageSlot SelectedImageSlot { get; set; }
        
        TatamiTabloInfo SelectedTablo { get; }
        bool Invalidated { get; }
        bool IsUpdated { get; }
        bool IsSaved { get; }

        string Name { get; set; }
        object UrlA { get; }
        object UrlB { get; }
        object UrlC { get; }
        object UrlD { get; }

        string GetUrl(ImageSlot slot);
        void SetTatamiTablo(TatamiTabloInfo val);
        void SetTamplateType(TemplateType templateType);
        void SetUrl(string url);
        void SetUrl(ImageSlot slot, string url);
        void SetConfigSaved();

        Action OnTemplateTypeChanged { get; set; }
        Action<ImageSlot, IImageSlotData> OnImageSlotChanged { get; set; }        
        Action OnTatamiTabloChanged { get; set; }

        bool IsCompositeTemplate { get; }
    }
}
