namespace CastManager.CoreImpl
{
    using CastManager.Worker;
    using CastManager.Templates;
    using CastManager.Templates.Worker;
    using CastManager.Core;
    using CastManager.StringsResources;
    using CastManager.Models.Core;
    using CastManager.Logger;

    public class TemplatesService : ITemplatesService, IDisposable
    {
        public ITemplateConfigs AllTemplateConfigs
        {
            get
            {
                configsLoadedEvent.WaitOne();
                return _templateConfigs;
            }
        }

        public Action OnConfigLoaded { get; set; }

        public Action<ITemplateConfig> OnCurrentConfigChanged { get; set; }

        public Action<ITemplateConfig> OnAssemblyTemplateDone { get; set; }

        public ITemplateConfig CurrentConfig
        {
            get => _templateConfig;
            private set
            {
                if (value != null)
                {
                    _templateConfig = value as TemplateConfig;
                    OnCurrentConfigChanged?.Invoke(_templateConfig);
                }
            }
        }

        private readonly TemplateConfigs _templateConfigs = new TemplateConfigs(new FileStorageTemplateImpl());

        private readonly IWorker _templatesWorkers;

        private TemplateConfig _templateConfig;

        private ManualResetEvent configsLoadedEvent = new ManualResetEvent(false);

        private readonly IAppGlobalEvents globalEvents;
        private readonly IDevicesService devicesService;

        public TemplatesService(IAppGlobalEvents globalEvents, IDevicesService devicesService)
        {
            _templatesWorkers = new TemplatesWorkers(this);

            globalEvents.OnMainWindowLoaded += () =>
            {
                Notify_CurrentConfigChanged();

                if (configsLoadedEvent.WaitOne(0))
                {
                    _templatesWorkers.Start();
                }
                else
                {
                    OnConfigLoaded += () => _templatesWorkers.Start();
                }
            };

            this.globalEvents = globalEvents;
            this.devicesService = devicesService;

            Initialzie();
        }

        ~TemplatesService()
        {
            Shutdown();
        }

        public async void Initialzie()
        {
            //await Task.Delay(3000); // test delay simulate loading config will take time
            await _templateConfigs.LoadTemplateConfigsAsync();

            var config = _templateConfigs.LastConfig;

            if (config == null)
            {
                config = CreateNewConfig();
            }

            CurrentConfig = config;

            configsLoadedEvent.Set();
            OnConfigLoaded?.Invoke();

            Logger.WriteLine("Template Configuration - loaded.");
        }

        public void Shutdown()
        {
            _templateConfigs.Dispose();
            _templatesWorkers.Stop();
        }

        public async void SaveConfigToDevice()
        {
            globalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                SpinnerGo = true,
                Text = Strings.ById("SavingConfig_Str")
            });

            var (isSaved, resultStr) = await devicesService.SaveConfigToDevicesAsync(CurrentConfig);

            if (isSaved)
            {
                CurrentConfig.SetConfigSaved();

                if (CurrentConfig.IsCompositeTemplate)
                {
                    await _templateConfigs.SaveTemplateConfigsAsync();
                    CurrentConfig = CreateNewConfig();
                }


            }
            globalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                Error = !isSaved ? resultStr : null,
                Text = isSaved ? resultStr : null,
                ShowDelay = TimeSpan.FromSeconds(4)
            });
        }

        private TemplateConfig CreateNewConfig()
        {
            var config = _templateConfigs.CreateNewConfig();
            _templateConfigs.AddConfig(config);
            return config;
        }

        private void Notify_CurrentConfigChanged()
        {
            if (configsLoadedEvent.WaitOne(0))
            {
                OnCurrentConfigChanged?.Invoke(_templateConfig);
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

    }
}
