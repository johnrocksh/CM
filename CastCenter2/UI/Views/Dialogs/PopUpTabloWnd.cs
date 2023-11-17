namespace CastManager.Views.Dialogs
{
    using System.Collections.ObjectModel;
    using CastManager.Logger;
    using System.Windows.Interop;
    using System.Windows.Controls;

    using CastManager.Models.Desktops;
    using CastManager.Models.Device;
    using CastManager.View.Dialogs;

    public class PopUpInfo
    {
        public string DeviceName { get; set; }
        public string TatamiName { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
    }

    public class PopUpTabloWnd
    {
        public List<PopUpInfo> GetPopUpTabloInfo(DeviceData selectedDevice, ObservableCollection<DesktopInfo> Desktops)
        {

            List<PopUpInfo> pupUpInfos = new List<PopUpInfo>();

            if (selectedDevice.Config == null) return pupUpInfos;

            foreach (var item_device in selectedDevice.Config.HostConfigs)
            {
                foreach (var item_desktop in Desktops)
                {
                    foreach (var config in item_desktop.TatamiConfigUrls)
                    {
                        if (config.Url.Contains(item_device.ImageUrl))
                        {
                            pupUpInfos.Add(new PopUpInfo
                            {
                                TatamiName = item_desktop.TatamiText,
                                Title = config.Title,
                                DeviceName = selectedDevice.Info.Id,
                                Avatar = config.Url
                            });
                        }
                    }
                }
            }
            return pupUpInfos;
        }

        public void ShowTabloInfoWnd(UserControl control)
        {
            //Show Window
            try
            {
                var infownd = InfoWnd.getInstance();
                var helper = new WindowInteropHelper(infownd);
                var hwndSource = HwndSource.FromHwnd(helper.EnsureHandle());
                var transformFromDevice = hwndSource.CompositionTarget.TransformFromDevice;
                System.Windows.Point wpfMouseLocation = transformFromDevice.Transform(GetMousePosition(control));

                infownd.Show();
            }
            catch (Exception e)
            {
                Logger.WriteLine($"LOG AddNewConfigBtnCmd throw an Exception  :{e}");
            }
        }

        public System.Windows.Point GetMousePosition(UserControl control)
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X, point.Y);
        }
    }
}
