namespace CastManager.ViewModels
{
    using CastManager.Models.Desktops;
    using CastManager.Models.Device;
    using CastManager.Core;
    using CastManager.CoreImpl;
    using CastManager.Templates;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel;
    using System.Windows;

    public class TemplateMakerViewModel : ViewModelBase, IPage
    {
        void IPage.OnActive()
        {
        }
    }
}
