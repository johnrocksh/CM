namespace CastManager.Templates
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class TemplateConfigs : ITemplateConfigs, IDisposable
    {
        public IEnumerable<TemplateConfig> Configs => _configs;

        public TemplateConfig LastConfig => _configs.Count > 0 ? _configs.Last() : null;

        public int Count => _configs.Count;

        public IDictionary<string, IImageSlotData> AllSlotsData => _slotsDataPool;

        private List<TemplateConfig> _configs;

        private readonly ConcurrentDictionary<string, IImageSlotData> _slotsDataPool = new();

        private readonly ITemplatesConfigStorage Storage;

        private object _lockObj = new();

        public TemplateConfigs(ITemplatesConfigStorage storage)
        {
            this.Storage = storage;
        }

        public async Task LoadTemplateConfigsAsync()
        {
            var results = await Storage.LoadAsync().ConfigureAwait(false);
            lock (_lockObj)
            {
                _configs = results?.ToList() ?? new List<TemplateConfig>();
                _configs.ForEach(x => x.Init(_slotsDataPool));
            }
        }

        public async Task SaveTemplateConfigsAsync()
        {
            await Storage.SaveAsync(_configs).ConfigureAwait(false);
        }

        ~TemplateConfigs()
        {
            Dispose();
        }

        public bool RemoveConfig(string name)
        {
            lock (_lockObj)
            {
                var item = GetConfig(name);
                return _configs.Remove(item);
            }
        }

        public TemplateConfig GetConfig(string name)
        {
            lock (_lockObj)
            {
                return _configs.FirstOrDefault(x => x.Name == name);
            }
        }

        public TemplateConfig CreateNewConfig()
        {
            var nextId = 0;
            lock (_lockObj)
            {
                nextId = _configs?.Count + 1 ?? 0;
            }
            var config = new TemplateConfig(_slotsDataPool)
            {
                Name = $"Template_{nextId}"
            };

            return config;
        }

        public bool AddConfig(TemplateConfig config)
        {
            lock (_lockObj)
            {
                if (_configs.Count(i => i.Name == config.Name) > 0)
                {
                    return false;
                }
                _configs.Add(config);
                return true;
            }
        }

        public async void Dispose()
        {
            await SaveTemplateConfigsAsync().ConfigureAwait(false);
            _configs = null;
        }
    }
}
