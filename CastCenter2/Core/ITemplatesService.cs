namespace CastManager.Core
{
    using CastManager.Templates;
    public interface ITemplatesService
    {
        ITemplateConfigs AllTemplateConfigs { get; }
        ITemplateConfig CurrentConfig { get; }

        Action OnConfigLoaded { get; set; }
        Action<ITemplateConfig> OnCurrentConfigChanged { get; set; }
        Action<ITemplateConfig> OnAssemblyTemplateDone { get; set; }

        void Initialzie();
        void Shutdown();
        void SaveConfigToDevice();
    }
}
