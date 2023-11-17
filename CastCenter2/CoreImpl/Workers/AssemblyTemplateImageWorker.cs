namespace CastManager.Templates.Worker
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using CastManager.Logger;
    using System.Collections.Generic;

    using CastManager.Views;
    using CastManager.Extentions;
    using CastManager.Templates;
    using CastManager.Worker;
    using CastManager.Core;
    using CastManager.Models.Templates.Assembly;

    /// <summary>
    /// The worker has monitor config invalidation that the template config was changed, and reassembly template to image raw data
    /// </summary>
    public class AssemblyTemplateImageWorker : WorkerBase
    {
        ITemplatesService _templatesService;

        public AssemblyTemplateImageWorker(ITemplatesService templatesService)
        {
            _templatesService = templatesService;
        }

        private readonly Dictionary<TemplateType, Lazy<FrameworkElement>> _templates = new()
        {
            { TemplateType.Horizontal, new Lazy<FrameworkElement>( () => new HorizontalTemplate() ) },
            { TemplateType.Vertical, new Lazy<FrameworkElement>( () => new VerticalTemplate() ) },
            { TemplateType.Grid, new Lazy<FrameworkElement>( () => new GridWndTemplate() ) },
            { TemplateType.ImageAdv, new Lazy<FrameworkElement>( () => new OneImageTemplate() ) },
        };

        protected override async Task DoWorkAsync()
        {
            await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
            await UpdateDataTemplatesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Create Template data from Wpf and put it to configs raw data
        /// </summary>
        private async Task UpdateDataTemplatesAsync()
        {
            var configs = _templatesService.AllTemplateConfigs.Configs;
            var currentConfig = _templatesService.CurrentConfig;

            var invalidated = configs
                .Where(x => x.IsCompositeTemplate)
                .Where(x => x.Invalidated || x.IsUpdated);

            var tasks = invalidated.Select(x => CreateTemplateDataFromWpfAsync(x));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private Task CreateTemplateDataFromWpfAsync(TemplateConfig config)
        {
            return Task.Run(() => CreateTemplateDataFromWpf(config));
        }       

        private void CreateTemplateDataFromWpf(TemplateConfig config)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (_templates.TryGetValue(config.Type, out var surfaceLazy))
                    {
                        var surface = surfaceLazy.Value;
                        surface.BeginInit();

                        surface.Width = 1920;
                        surface.Height = 1080;
                        surface.DataContext = new DataContextAssembly()
                        {
                            CurrentConfig = config
                        };                        
                        surface.EndInit();

                        var imageData = surface.ToPngImage();
                        config.SetResultRawData(imageData);
                        _templatesService.OnAssemblyTemplateDone?.Invoke(config);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteException(ex.Message);
                }
            });
        }
    }
}
