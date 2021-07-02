using System.Threading;
using TheScoreBook.models.enums;
using TheScoreBook.Ui.data.lang;
using TheScoreBook.Ui.enums;
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

        // Theme
        private const string AppThemeKey = nameof(AppThemeKey);

        public static OSAppTheme AppTheme
        {
            get => (OSAppTheme)Preferences.Get(AppThemeKey, (int)Application.Current.RequestedTheme);
            set
            {
                Preferences.Set(AppThemeKey, (int) value);
                Application.Current.UserAppTheme = value;
            }
        }

        public static bool IsDarkMode => AppTheme == OSAppTheme.Dark;
        public static T GetStaticResource<T>(string key) => (T) Application.Current.Resources[key];
        
        // Rescore Button Type
        private const string RescoreTypeKey = nameof(RescoreTypeKey);

        public static RescoreType RescoreType
        {
            get => (RescoreType) Preferences.Get(RescoreTypeKey, (int)RescoreType.LONG);
            set => Preferences.Set(RescoreTypeKey, (int) value);
        }
    }
}