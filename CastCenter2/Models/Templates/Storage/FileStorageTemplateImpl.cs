namespace CastManager.Templates
{
    using System;
    using System.Collections.Generic;
    using CastManager.Logger;
    using System.IO;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    internal class FileStorageTemplateImpl : ITemplatesConfigStorage
    {
        private readonly string FileName = @"TemplatesConfigs.json";

        public async Task<IEnumerable<TemplateConfig>> LoadAsync()
        {
            try
            {
                using StreamReader reader = File.OpenText(FileName);
                var data = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<TemplateConfig>>(data);
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
            return null;
        }

        public async Task<bool> SaveAsync(IEnumerable<TemplateConfig> config)
        {
            try
            {
                using StreamWriter writer = File.CreateText(FileName);                
                var data = JsonConvert.SerializeObject(config);
                await writer.WriteAsync(data);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
            return false;
        }
    }
}
