namespace CastManager.CoreImpl.Lookup
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using System.Diagnostics;
    using CastManager.Worker;
    using CastManager.CoreImpl.Lookup.Addresses;
    using CastManager.Client.Poll;
    using CastManager.Client;
    using System.Net;
    using Windows.Services.Maps;

    /// <summary>
    /// Base implemenation logic to lookup http service over local network
    /// </summary>
    internal class LookupWorker : WorkerBase
    {
        private IPollClient pollClient;

        /// <summary>
        /// 
        /// </summary>
        public string Query { get; set; } = "/index";

        /// <summary>
        /// Milliseconds
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Http port to discovery
        /// </summary>
        public ushort Port { get; set; } = 80;

        /// <summary>
        /// User specific url for lookup
        /// </summary>
        public Func<string, string> OnMakeEndpoint { get; set; }

        /// <summary>
        /// Device discovered result, the callback. 
        /// The param is the success response result from host
        /// </summary>
        public Action<string, string> OnDiscovered { get; set; }

        /// <summary>
        /// Device discovered result, the callback. 
        /// The param is the success response result from host
        /// </summary>
        public Action<int> OnLookupRoundDone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IAddressesLoop addressesLoop;

        int count = 0;
        int roundNum = 0;

        protected IClientsFactory clientsFactory;

        /// <summary>
        /// Create instance to discovery devices
        /// </summary>
        /// <param name="totalWorkers">Count of job workers.</param>
        public LookupWorker(IClientsFactory clientsFactory, IAddressesLoop addressesLoop)
        {
            this.clientsFactory = clientsFactory;
            this.addressesLoop = addressesLoop;
            if (addressesLoop == null)
            {
                throw new Exception("Not all property was initialized:" + nameof(addressesLoop));
            }

            base.OnStart += OnStart;
        }

        new void OnStart()
        {
            pollClient = clientsFactory.GetPollClient(Timeout, _cts);
        }
        /// <summary>
        /// Add addresses to the address pool
        /// </summary>
        protected override async Task DoWorkAsync()
        {
            var tasks = PollClients();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            OnLookupRoundDone?.Invoke(++roundNum);
        }

        IEnumerable<Task> PollClients()
        {
            for(int i = 0; i < addressesLoop.Count;i++)
            {
                yield return Task.Run(async () =>
                {
                    var ip = addressesLoop.Next;
                    var url = OnMakeEndpoint?.Invoke(ip) ?? $"http://{ip}:{Port}{Query}";

                    var response = await pollClient.GetAsync(url).ConfigureAwait(false);

                    if (response != null)
                    {
                        OnDiscovered?.Invoke(ip, response);
                    }
                });
            }
        }

    }
}
