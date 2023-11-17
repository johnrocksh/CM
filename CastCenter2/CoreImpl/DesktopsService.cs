namespace CastManager.CoreImpl
{
    using CastManager.Models.Desktops;
    using CastManager.Core;
    using System.Collections.ObjectModel;
    using CastManager.Models.Core;

    public class DesktopsService : IDesktopsService
    {
        public IList<DesktopInfo> Desktops { get; private set; } = new ObservableCollection<DesktopInfo>();

        public Action<ItemAction, DesktopInfo> OnDesktopAction { get; set; }

        private object lockObj = new();

        public DesktopsService(ILookupService lookups, IMonitorService monitorService)
        {
            lookups.OnDesktopFound += OnDesktopFound;
            monitorService.OnDesktopLost += OnDesktopNotFound;
            monitorService.OnDesktopChanged += OnDesktopUpdated;
            monitorService.Desktops += () => Desktops;
        }

        /// <summary>
        /// Add found desktop in to collection that show at UI
        /// </summary>
        void OnDesktopFound(DesktopInfo desktopInfo)
        {
            if (desktopInfo == null)
                return;

            lock (lockObj)
            {
                var item = Desktops.FirstOrDefault(i => i.IsSameIp(desktopInfo));

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (item == null)
                    {
                        Desktops.Add(desktopInfo);
                    }
                    else
                    {
                        Desktops.Remove(item);
                        Desktops.Add(desktopInfo);
                    }
                });
            }
            SendCallback(ItemAction.Added, desktopInfo);
        }


        /// <summary>
        /// Call`s when desktop info was changed, update the desktop item at collection, that show at UI
        /// </summary>
        void OnDesktopUpdated(DesktopInfo desktopInfo)
        {
            if (desktopInfo != null)
            {
                lock (lockObj)
                {
                    var item = Desktops.FirstOrDefault(i => i.IsSameIp(desktopInfo));
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (item != null)
                        {
                            Desktops.Remove(item);
                            Desktops.Add(desktopInfo);
                        }
                    });
                }
                SendCallback(ItemAction.Updated, desktopInfo);
            }
        }

        /// <summary>
        /// Call`s when desktop service was lost, remove the desktop item from the collection, that show at UI
        /// </summary>
        void OnDesktopNotFound(DesktopInfo desktopInfo)
        {
            if (desktopInfo != null)
            {
                lock (lockObj)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Desktops.Remove(desktopInfo);
                    });
                }
                SendCallback(ItemAction.Removed, desktopInfo);
            }
        }

        void SendCallback(ItemAction action, DesktopInfo desktopInfo)
        {
            if (OnDesktopAction != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    OnDesktopAction.Invoke(action, desktopInfo);
                });
            }
        }
    }
}
