namespace CastManager.Views
{
    using System.Windows;
    using CastManager.Models;

    /// <summary>
    /// A static class responsible for creating and managing various dialogs used in the application.
    /// </summary>
    public static class DialogsFactory
    {
        static readonly LazyWrappers lazyWrappers = new();

        public static ThemeDialog ThemeDialog => lazyWrappers.Get<ThemeDialog>();
        public static AddHostDialog AddHostDialog => lazyWrappers.Get<AddHostDialog>();

        public static void ToggleVisible(this Window wnd)
        {
            if (wnd.IsVisible)
            {
                wnd.Hide();
            }
            else
            {
                wnd.Show();
            }
        }
    }
}
