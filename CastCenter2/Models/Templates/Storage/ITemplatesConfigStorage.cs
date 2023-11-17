namespace CastManager.Templates
{
   
    public interface ITemplatesConfigStorage
    {
        Task<IEnumerable<TemplateConfig>> LoadAsync();

        Task<bool> SaveAsync(IEnumerable<TemplateConfig> config);
    }
}
