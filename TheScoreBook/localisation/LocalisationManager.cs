using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using TheScoreBook.data.lang;
using Xamarin.Essentials;

namespace TheScoreBook.localisation
{
    public class LocalisationManager : INotifyPropertyChanged
    {
        private const string LanguageKey = nameof(LanguageKey);

        public event PropertyChangedEventHandler PropertyChanged;
        public int LanguageChangedNotification => 0;

        private static readonly Lazy<LocalisationManager> instance = new(() => new LocalisationManager());
        public static LocalisationManager Instance => instance.Value;

        public CultureInfo CurrentCulture => AppResources.Culture ?? Thread.CurrentThread.CurrentUICulture;

        private LocalisationManager()
        {
            SetCulture(new CultureInfo(Preferences.Get(LanguageKey, CurrentCulture.TwoLetterISOLanguageName)));
        }

        public void SetCulture(CultureInfo language)
        {
            Thread.CurrentThread.CurrentUICulture = language;
            AppResources.Culture = language;
            Preferences.Set(LanguageKey, language.TwoLetterISOLanguageName);

            Invalidate();
        }

        public string GetValue(string text, string ResourceId)
        {
            var manager = new ResourceManager(ResourceId, typeof(LocalisationManager).GetTypeInfo().Assembly);
            return manager.GetString(text, CultureInfo.CurrentCulture);
        }

        public void Invalidate()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LanguageChangedNotification"));
        }

        public static string ToTitleCase(string input)
            => Instance.CurrentCulture.TextInfo.ToTitleCase(input);

        public static string LocalisedShortDate(DateTime date)
            => date.ToString("dd/MM/yyyy");

        public string this[string text] => AppResources.ResourceManager.GetString(text, AppResources.Culture);
    }
}