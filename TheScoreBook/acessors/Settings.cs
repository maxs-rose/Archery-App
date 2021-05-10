using System.Threading;
using TheScoreBook.data.lang;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TheScoreBook.acessors
{
    public static class Settings
    {
        // Language Setting
        private const string LanguageKey = nameof(LanguageKey);
        public static string Language
        {
            get => Preferences.Get(LanguageKey, AppResources.Culture?.TwoLetterISOLanguageName ?? Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
            set => Preferences.Set(LanguageKey, value);
        }
        
        // Colourful Arrows Settings
        private const string ColorfulArrowsKey = nameof(ColorfulArrowsKey);
        public static bool ColorfulArrows
        {
            get => Preferences.Get(ColorfulArrowsKey, true);
            set => Preferences.Set(ColorfulArrowsKey, value);
        }

        // Dark Mode
        private const string DarkModeKey = nameof(DarkModeKey);
        public static bool DarkMode
        {
            get => Preferences.Get(DarkModeKey, false);
            set => Preferences.Set(DarkModeKey, value);
        }

        public static Color BackgroundColor => DarkMode ? Color.DimGray : Color.White;
    }
}