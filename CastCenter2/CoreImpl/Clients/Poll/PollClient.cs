namespace CastManager.Client.Poll
{
    using System;
    using System.Threading;

    using System.Diagnostics;
    using System.Net.Http;
    using CastManager.Logger;

    public class PollClient : IPollClient
    {
        CancellationTokenSource _cts = new CancellationTokenSource();

        TimeSpan timeout = TimeSpan.FromSeconds(2);

        public PollClient(TimeSpan timeout, CancellationTokenSource cts)
        {
            this.timeout = timeout;
            _cts = cts;
        }

        public async Task<string> GetAsync(string endpointUrl)
        {
            try
            {
                using var client = new HttpClient();

                client.Timeout = timeout;

                var httpResult = await client.GetAsync(endpointUrl, _cts.Token).ConfigureAwait(false);

                if (httpResult.IsSuccessStatusCode)
                {
                    return await httpResult.Content.ReadAsStringAsync(_cts.Token).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException($"PollClient reject from ip: {endpointUrl}: " + ex.Message);
            }
            return null;
        }
    }
}
