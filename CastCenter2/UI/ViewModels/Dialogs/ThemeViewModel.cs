namespace CastManager.ViewModels
{
    using System.Windows;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using GalaSoft.MvvmLight;

    using CastManager.Models.Theme;

    public class ThemeViewModel : ViewModelBase
    {
        public ObservableCollection<ColorTheme> ColorThemes { get; set; } = new();

        static ResourceDictionary Resources => Application.Current.Resources;

        ResourceDictionary CurrentSkinDictionary = null;

        ColorTheme selectedTheme;

        public ColorTheme SelectedTheme
        {
            get => selectedTheme; set
            {
                if (value != null)
                {
                    Set(ref selectedTheme, value);
                    ChangeSkin(selectedTheme.ThemeName);
                }
            }
        }

        ResourceDictionary FindResource(string resName) => Resources.MergedDictionaries.FirstOrDefault(d => d.Source.ToString().Contains(resName));

        ResourceDictionary CreateNewSkinResource(string skinName) => new()
        {
            Source = new Uri($"UI\\Styles\\Themes\\{skinName}.xaml", UriKind.Relative)
        };

        ColorTheme GetColorTheme(string name) => ColorThemes.Where(t => t.ThemeName == name).FirstOrDefault();

        public ThemeViewModel()
        {
            if (!IsInDesignMode)
            {
                InitColorThemes();
                SelectedTheme = GetColorTheme("ClassicSkin");
            }
        }

        void ChangeSkin(string skinName)
        {
            if (CurrentSkinDictionary is not null)
            {
                Resources.MergedDictionaries.Remove(CurrentSkinDictionary);
            }

            var skinRes = FindResource(skinName);
            if (skinRes == null)
            {
                skinRes = CreateNewSkinResource(skinName);
                Resources.MergedDictionaries.Add(skinRes);
            }
            CurrentSkinDictionary = skinRes;
        }

        void InitColorThemes()
        {
            AddColorThemeFromSkin("ClassicSkin");
            AddColorThemeFromSkin("DinerSkin");

            //TODO: Add other themes to skin style xaml new files
            //ColorThemes.Add(new ColorTheme("Carbon", "#292521", "#509452", "#f5ae2d"));
            //ColorThemes.Add(new ColorTheme("Earthsong", "#313131", "#f66e0d", "#509452"));
            //ColorThemes.Add(new ColorTheme("Camping", "#444", "#f44c7f", "#e3e3e3"));
            //ColorThemes.Add(new ColorTheme("Dark", "#444", "#222", "#eee"));
            //ColorThemes.Add(new ColorTheme("wavezs", "#1b3238", "#6bde3b", "#1f5e6b"));
            //ColorThemes.Add(new ColorTheme("Eadlin", "#221c35", "#f67599", "#5a3a7e"));
            //ColorThemes.Add(new ColorTheme("King Matrix", "#1e001e", "#972fff", "#c58aff"));
            //ColorThemes.Add(new ColorTheme("80s", "#3A4149", "#cf6bdd", "#eedaea"));
            //ColorThemes.Add(new ColorTheme("vscode", "#2c3333", "#95d5b2", "#f0d3c9"));
        }

        ResourceDictionary FindSkinOrCreateNew(string skinName)
        {
            var r = FindResource(skinName);
            return r is null ? CreateNewSkinResource(skinName) : r;
        }

        void AddColorThemeFromSkin(string skinName)
        {
            var r = FindSkinOrCreateNew(skinName);

            ColorThemes.Add(new ColorTheme()
            {
                ThemeName = skinName,
                ColorTop = r["ColorTop"].ToString(),
                ColorButton = r["ColorButton"].ToString(),
                ColorCenter = r["ColorCenter"].ToString(),
                ColorButtonPressed = r["ColorButtonPressed"].ToString(),
            });
        }
    }
}
