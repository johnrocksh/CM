namespace CastManager.Core
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using CastManager.Models;
    using CastManager.CoreImpl;
    using CastManager.ViewModels;
    using CastManager.Client;

    public static class DependencyInjection
    {
        public static IServiceCollection Services => builder.Services;

        private static HostApplicationBuilder builder;
        private static ServiceProvider serviceProvider;

        static public void Init()
        {
            builder = Host.CreateApplicationBuilder();

            builder.Services.AddSingleton<IAppGlobalEvents, AppGlobalEvents>();
            builder.Services.AddSingleton<IAppConfiguration, AppConfiguration>();

#if MOCK
            builder.Services.AddSingleton<IClientsFactory, ClientsFactoryMock>();
            builder.Services.AddSingleton<IPicasterPatcher, PicasterPatcherMock>();
#else
            builder.Services.AddSingleton<IClientsFactory, ClientsFactory>();
            builder.Services.AddSingleton<IPicasterPatcher, PicasterSimpleWifi>();
#endif

            builder.Services.AddSingleton<ILookupService, LookupService>();
            builder.Services.AddSingleton<IMonitorService, MonitorService>();
            builder.Services.AddSingleton<IDevicesService, DevicesService>();
            builder.Services.AddSingleton<IDesktopsService, DesktopsService>();
            builder.Services.AddSingleton<ITemplatesService, TemplatesService>();
            builder.Services.AddSingleton<ITemplateHttpServer, TemplateHttpServer>();

            builder.Services.AddTransient<TemplateViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            
            builder.Services.AddTransient<OneImageTemplateViewModel>();
            builder.Services.AddTransient<HorizontalTemplateViewModel>();
            builder.Services.AddTransient<VerticalTemplateViewModel>();
            builder.Services.AddTransient<GridWndTemplateViewModel>();
            builder.Services.AddTransient<OriginHomeTemplateMakerViewModel>();
            builder.Services.AddTransient<OriginMakerTatamiSelectionViewModel>();
            builder.Services.AddTransient<OriginMakerLinkToTabloViewModel>();
            builder.Services.AddTransient<TemplateMakerViewModel>();
            builder.Services.AddTransient<OneImageTemplateMakerViewModel>();


            serviceProvider = Services.BuildServiceProvider();
        }

        static public T Get<T>() => serviceProvider.GetService<T>();

        static public void Instantiate<T>() => Get<T>();

        static public void Exit()
        {
            serviceProvider.Dispose();
        }
    }
}
