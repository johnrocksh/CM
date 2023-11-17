namespace CastManager.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
    using CastManager.Logger;
    using System.Threading;
    using NativeWifi;

    public static class Network
    {

        /// <summary>
        /// Represents a client to the Zeroconf (Native Wifi) service.
        /// </summary>
        static readonly Lazy<WlanClient> wlanClient = new( () => new() );

        /// <summary>
        ///We check if there is an Internet connection or not..
        /// </summary>
        public static bool IsInternetConection()
        {
            string host = "www.google.com";
            bool result = false;
            Ping p = new Ping();

            try
            {
                Logger.WriteLine("LOG: inside IsInternetConection()");
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    Logger.WriteLine($"LOG: internet connection status  = Success");
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.WriteException(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Get the current wifi network name
        /// </summary>
        static public string GetCurrentWifiNetworkName()
        {
            try
            {
                var @interface = wlanClient.Value.Interfaces.FirstOrDefault();
                var ssid = @interface?.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                return ssid.ToStringV2();
            }
            catch (Exception e)
            {
                Logger.WriteException(e.Message);
            }
            return string.Empty;
        }

        public static IEnumerable<IPAddress> GetIPAddress()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Select(i => i.GetIPProperties().UnicastAddresses)
                .SelectMany(u => u)
                .Where(u => u.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(i => i.Address);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static IEnumerable<IPAddress> AllInterNetworksAddresses => Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.Where(u => u.AddressFamily == AddressFamily.InterNetwork);

        public static IEnumerable<string> AllLoaclIPs
        {
            get
            {
                foreach (var ipAddress in AllInterNetworksAddresses)
                {
                    var b = ipAddress.GetAddressBytes();

                    for (int i = 1; i < 255; ++i)
                    {
                        var b0 = b[0];
                        var b1 = b[1];
                        var b2 = b[2];
                        var b3 = i;
                        yield return $"{b0}.{b1}.{b2}.{b3}";
                    }
                }
            }
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        //ping locals address in network
        public static void LookupActiveLocals(Action<string> onActiveFound) 
        {
            const int timeOut = 250;
            const int ttl = 25;
            byte[] buffer =
            {
                0xab, 0xab, 0xab, 0xab, 0xab, 0xab, 0xab, 0xab
            };

            int instances = 0;
            object @lock = new object();

            SpinWait wait = new SpinWait();

            List<Ping> pingers = new();

            PingOptions po = new PingOptions(ttl, true);

            foreach (var ip in AllLoaclIPs)
            {
                lock (@lock)
                {
                    ++instances;
                }

                Ping p = new Ping();
                p.PingCompleted += (object s, PingCompletedEventArgs e) =>
                {
                    if (e.Reply?.Status == IPStatus.Success)
                    {
                        var activeIp = e.Reply.Address.ToString();
                        Logger.WriteLine(string.Concat("Active IP: ", activeIp));
                        onActiveFound(activeIp);
                    }
                    lock (@lock)
                    {
                        --instances;
                    }
                };
                p.SendAsync(ip, timeOut, buffer, po);
                pingers.Add(p);
            }
            while (instances > 0)
            {
                wait.SpinOnce();
            }
            foreach (Ping p in pingers)
            {
                p.Dispose();
            }
            pingers.Clear();
        }

        public static Task LookupActiveLocalsAsync(Action<string> onActiveLocalIPFound)
        {
            return Task.Run(() => LookupActiveLocals(onActiveLocalIPFound));
        }

        static List<string> ActiveLocalAddresses = new List<string>();
        public static async Task<List<string>> LookupActiveLocalsAsync()
        {
            ActiveLocalAddresses.Clear();
            await LookupActiveLocalsAsync(ip => ActiveLocalAddresses.Add(ip));
            return ActiveLocalAddresses;
        }
    }

}
