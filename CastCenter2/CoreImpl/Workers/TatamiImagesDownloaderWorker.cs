namespace CastManager.Templates.Worker
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Drawing;
    using System.Threading.Tasks;
    using CastManager.Logger;

    using HttpStatusCode = System.Net.HttpStatusCode;

    using CastManager.Errors;
    using CastManager.Extentions;
    using CastManager.Templates;
    using CastManager.Worker;
    using CastManager.Core;
    using System.Collections.Concurrent;
    using System.Windows.Media.Imaging;
    using Microsoft.Windows.ApplicationModel.Resources;


    /// <summary>
    /// The worker downloading images from it endpoint url, every time when it "hash" None-Match
    /// </summary>
    class TatamiImagesDownloaderWorker : WorkerBase
    {

        private readonly ConcurrentDictionary<string, IPendingContext> poolOfPendingContent = new();
        private readonly ConcurrentBag<string> poolOfInvalidUri = new();

        private readonly ITemplatesService _templatesService;

        public TatamiImagesDownloaderWorker(ITemplatesService templatesService)
        {
            _templatesService = templatesService;
        }

        protected override async Task DoWorkAsync()
        {
            try
            {
                await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
                await UpdateImagesAsync().ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
        }
             
        private async Task UpdateImagesAsync()
        {
            var slots = _templatesService.AllTemplateConfigs.AllSlotsData;

            var updateSlotsFn = (IImageSlotData x) => Task.Run(() =>
            {
                UpdateImageSourceCache(x.Url);
                slots[x.Url].Update(x);
            });

            var tasks = slots.Where(x => ValidateImageUrl(x.Key)).Select(x => DownloadImageAsync(x.Value, updateSlotsFn));

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private void UpdateImageSourceCache(string url)
        {
            //Update cached image
            Task.Run(() => System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                var uri = new Uri(url);
                if (uri != null)
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    image.CacheOption = BitmapCacheOption.None;
                    image.UriSource = uri;
                    image.EndInit();
                }
            }));
        }

        private async Task DownloadImageAsync(IImageSlotData slot, Func<IImageSlotData, Task> updateSlotsFn)
        {
            var result = await DownloadImageAsync(slot).ConfigureAwait(false);

            if (result)
            {
                // call ImageSlotData.update for all slots with selected url
                await (updateSlotsFn?.Invoke(slot)).ConfigureAwait(false);
            }
        }

        private bool ValidateImageUrl(string url)
        {
            try
            {
                if(poolOfInvalidUri.Contains(url))
                {
                    return false;
                }

                var url_ = new Nancy.Url(url);

                if (url_.Scheme != "http")
                {
                    poolOfInvalidUri.Add(url);
                    return false;
                }

                if(poolOfPendingContent.TryGetValue(url_, out var content) )
                {
                    if (content.IsElapsed)
                    {
                        poolOfPendingContent.Remove(url_, out var _);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
            poolOfInvalidUri.Add(url);
            return false;
        }

        private async Task<bool> DownloadImageAsync(IImageSlotData slot)
        {
            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", slot.Hash);

                using var response = await client.GetAsync(slot.Url).ConfigureAwait(false);

                var code = (int)response.StatusCode;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotModified:
                        return false;
                    case HttpStatusCode.OK:
                        {
                            var checksum = response.Headers.GetValues("Checksum").FirstOrDefault();
                            slot.SetSourceChecksum(checksum);

                            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                            if (stream != null && stream.Length != 0)
                            {
                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var source = stream.ToImageSource();
                                    slot.SetSource(source);
                                });

                                Logger.WriteLine($"Image {slot.Url} was changed, and downloaded");
                                return true;
                            }
                            Logger.WriteLine($"Image {slot.Url} content empty.");
                            DonwloadResourceFaild(code, slot);
                        }
                        return false;
                    default:
                            DonwloadResourceFaild(code, slot);
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
            DonwloadResourceFaild(0, slot);
            return false;
        }

        private void DonwloadResourceFaild(int code, IImageSlotData slot)
        {
            var pendingPeriod = TimeSpan.FromMinutes(1);
            var pendingContent = new PendingContext<IImageSlotData>(slot, pendingPeriod);

            poolOfPendingContent.TryAdd(slot.Url, pendingContent);
            ErrorHandle?.Invoke(
                new ErrorData()
                {
                    error = ErrorNum.Error_DownloadImage,
                    code = code,
                    message = $"Resource {slot.Url} not downloaded.",
                    data = slot
                });
        }
    }
}
