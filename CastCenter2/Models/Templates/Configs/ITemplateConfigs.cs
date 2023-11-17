namespace CastManager.Templates
{
    public interface ITemplateConfigs
    {
        IEnumerable<TemplateConfig> Configs { get; }

        IDictionary<string, IImageSlotData> AllSlotsData { get; }

        TemplateConfig GetConfig(string name);
    }
}
