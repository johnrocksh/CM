namespace CastManager.CoreImpl
{
    using CastManager.Client;
    using CastManager.Models.Desktops;
    using CastManager.Models.Device;
    using CastManager.CoreImpl.Lookup;
    using CastManager.CoreImpl.Lookup.Addresses;
    using CastManager.Core;
    using CastManager.StringsResources;
    using CastManager.Models.Core;

    public class LookupService : ILookupService
    {
        DesktopDiscoveryWorker desktopDiscoveryWorker;
        DevicesDiscoveryWorker devicesDiscoveryWorker;

        Task lookupActiveAddressesTask = Task.CompletedTask;

        List<string> activeLocalAddresses = new();
        List<string> externalAddresses = new();

        public Action<DeviceData> OnDeviceFound { get; set; }

        public Action<DesktopInfo> OnDesktopFound { get; set; }

        ManualResetEvent serviceStarted = new ManualResetEvent(false);

        CancellationTokenSource _cts;

        private readonly IClientsFactory clientsFactory;
        private readonly IAppGlobalEvents appGlobalEvents;

        public LookupService(IClientsFactory clientsFactory, IAppGlobalEvents appGlobalEvents)
        {
            this.clientsFactory = clientsFactory;
            this.appGlobalEvents = appGlobalEvents;
        }

        public void Start()
        {
            serviceStarted.Reset();

            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                Text = Strings.ById("SearchingServices_Str"),
                SpinnerGo = true
            });

            LookupActiveAddresses();
            LookupDevicesAsync();
            LookupDesktopsAsync();
        }

        public void Stop()
        {
            desktopDiscoveryWorker.Stop();
            devicesDiscoveryWorker.Stop();
            _cts.Cancel();
            Wait();
        }

        public void Wait()
        {
            serviceStarted.WaitOne();
            List<Task> tasks = new List<Task>();
            tasks.Add(lookupActiveAddressesTask);
            tasks.AddRange(desktopDiscoveryWorker.AllTasks);
            tasks.AddRange(devicesDiscoveryWorker.AllTasks);
            Task.WaitAll(tasks.ToArray());
            serviceStarted.Reset();
        }

        public bool AddIpAddreess(string ip)
        {
            var s = externalAddresses.Find(x => x == ip);

            if(s == null) 
            {
                externalAddresses.Add(ip);
            }

            return s == null;
        }

        public void StartLookupDesktops()
        {
            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                Text = Strings.ById("SearchingServices_Desktops_Str"),
                SpinnerGo = true
            });
            if (desktopDiscoveryWorker?.IsRunning() ?? false)
            {
                return;
            }
            LookupActiveAddresses();
            LookupDesktopsAsync();
        }

        public void StartLookupDevices()
        {
            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
            {
                Text = Strings.ById("SearchingServices_Devices_Str"),
                SpinnerGo = true
            });
            if (devicesDiscoveryWorker?.IsRunning() ?? false)
            {
                return;
            }
            LookupActiveAddresses();
            LookupDevicesAsync();
        }

        /// <summary>
        /// Lookup Active Ip Addresses in local network
        /// </summary>
        void LookupActiveAddresses()
        {

            if (lookupActiveAddressesTask.IsCompleted)
            {
                _cts = new CancellationTokenSource();
                activeLocalAddresses.Clear();
                lookupActiveAddressesTask = Task.Run(() => clientsFactory.LookupActiveLocalsAsync(ip => activeLocalAddresses.Add(ip)));
            }
        }

        /// <summary>
        /// LookupDeviceAsync
        /// </summary>
        void LookupDevicesAsync()
        {
            Task.Run(() =>
            {
                var loopAddress = CreateLookupActiveAddresses();
                if (loopAddress != null)
                {
                    devicesDiscoveryWorker = new DevicesDiscoveryWorker(clientsFactory, loopAddress)
                    {
                        OnDeviceDiscovered = OnDeviceFound,
                        OnLookupRoundDone = n =>
                        {
                            devicesDiscoveryWorker?.Stop();
                            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
                            {
                                Text = Strings.ById("LookupsFinished_Devices_Str"),
                                ShowDelay = TimeSpan.FromSeconds(7),
                            });
                        }

                    };
                    devicesDiscoveryWorker.Start();
                }
            });
        }

        /// <summary>
        /// LookupDesktopsAsync
        /// </summary>
        void LookupDesktopsAsync()
        {
            Task.Run(() =>
            {
                var loopAddress = CreateLookupActiveAddresses();
                if (loopAddress != null)
                {
                    // find where our desktop applications are running
                    desktopDiscoveryWorker = new DesktopDiscoveryWorker(clientsFactory, loopAddress)
                    {
                        OnDesktopDiscovered = OnDesktopFound,
                        OnLookupRoundDone = n =>
                        {
                            desktopDiscoveryWorker?.Stop();
                            appGlobalEvents.OnStatusChanged?.Invoke(new AppStatus()
                            {
                                Text = Strings.ById("LookupsFinished_Desktop_Str"),
                                ShowDelay = TimeSpan.FromSeconds(7),
                            });
                        }
                    };
                    desktopDiscoveryWorker?.Start();
                }
                serviceStarted.Set();
            });
        }

        IAddressesLoop CreateLookupActiveAddresses()
        {
            // We find active ip addresses all available ip addresses in the local network 
            if (lookupActiveAddressesTask.IsCompleted == false)
            {
                // We are waiting for the completion of the task
                // of finding active ip addresses
                lookupActiveAddressesTask.Wait();
            }

            if (!_cts.IsCancellationRequested)
            {
                return new ActiveAddresses(activeLocalAddresses.Union(externalAddresses));
            }
            return null;
        }
    }
}
