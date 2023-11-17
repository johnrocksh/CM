namespace CastManager.CoreImpl
{
    using CastManager.Logger;
    using System.Net;
    using CastManager.Templates;
    using CastManager.Core;
    using Nancy;

    /// <summary>
    /// Http server distribute images that were prepared (combined) by template configs: grid, horizontal, vertical.
    /// They represent combinations of other images that come (downloaded) from TurnametExpert service.
    /// For the downloading other images coresponds the TatamiImagesDownloaderService.
    /// </summary>
    public class TemplateHttpServer : ITemplateHttpServer, IDisposable
    {
        private readonly ITemplatesService _templatesService;
        private readonly IAppConfiguration _appConfiguration;

        private ITemplateConfigs TemplateConfigs => _templatesService.AllTemplateConfigs;

        private Thread serverThread;

        private CancellationTokenSource _cts;

        private string UriPrefix => _appConfiguration.HttpServerPrefix;

        public TemplateHttpServer(ITemplatesService templatesService, IAppConfiguration appConfiguration)
        {
            _templatesService = templatesService;
            _appConfiguration = appConfiguration;
            templatesService.OnConfigLoaded += Start;
        }

        ~TemplateHttpServer()
        {
            Stop();
        }

        /// <summary>
        /// Stop the http server 
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
            serverThread?.Join();
            _cts = null;
            serverThread = null;
        }

        public void Start()
        {
            if (serverThread != null)
            {
                Logger.WriteLine($"Server {UriPrefix}, - already runing ...");
                return;
            }

            serverThread = new Thread(Run);
            serverThread.Start();
        }

        /// <summary>
        /// Start http server that will destribute images of prepared templates
        /// </summary>
        async void Run()
        {
            Logger.WriteLine($"Server {UriPrefix}, - run ...");

            var listener = new HttpListener();

            listener.Prefixes.Add(UriPrefix);
            listener.IgnoreWriteExceptions = true;

            _cts = new CancellationTokenSource();

            try
            {
                listener.Start();

                Logger.WriteLine($"Server {UriPrefix}, - started ...");

                while (_cts.IsCancellationRequested == false)
                {
                    var context = await listener.GetContextAsync().WaitAsync(_cts.Token).ConfigureAwait(false);
                    var request = context.Request;

                    if (request.HttpMethod != "HEAD")
                    {
                        try
                        {
                            var templateName = request.RawUrl.Split('/').Last();
                            var config = TemplateConfigs.GetConfig(templateName);
                            if (config != null)
                            {
                                var rawData = config.GetResultRawData();
                                if (rawData != null)
                                {
                                    var response = context.Response;
                                    response.SendChunked = true;
                                    response.ContentType = "image/png";
                                    //response.AddHeader("ETag", config.Checksum);
                                    //response.AddHeader("Checksum", defaultImage.Checksum);
                                    //response.AddHeader("Cache-Control", "public, max-age=3600");
                                    response.ContentLength64 = rawData.Length;
                                    await response.OutputStream.WriteAsync(rawData, 0, rawData.Length, _cts.Token).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteException(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
            Logger.WriteLine($"Server {UriPrefix}, - stopped ...");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}


