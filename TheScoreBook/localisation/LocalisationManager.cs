using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Threading;
using TheScoreBook.acessors;
using TheScoreBook.data.lang;
using Xamarin.Essentials;

namespace TheScoreBook.localisation
{
    public class LocalisationManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public int LanguageChangedNotification => 0;

        private static readonly Lazy<LocalisationManager> instance = new(() => new LocalisationManager());
        public static LocalisationManager Instance => instance.Value;

        public CultureInfo CurrentCulture => AppResources.Culture ?? Thread.CurrentThread.CurrentUICulture;

        private LocalisationManager()
        {
            SetCulture(new CultureInfo(Settings.Language));
        }

        public void SetCulture(CultureInfo language)
        {
            Thread.CurrentThread.CurrentUICulture = language;
            AppResources.Culture = language;
            Settings.Language = language.TwoLetterISOLanguageName;

            Invalidate();
        }

        public string GetValue(string text, string ResourceId)
        {
            var manager = new ResourceManager(ResourceId, typeof(LocalisationManager).GetTypeInfo().Assembly);
            return manager.GetString(text, CultureInfo.CurrentCulture);
        }

        public void Invalidate()
        {
            ForceUIReload();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LanguageChangedNotification"));
        }

        public void ForceUIReload()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public static string ToTitleCase(string input)
            => Instance.CurrentCulture.TextInfo.ToTitleCase(input);

        public static string ToRoundTitleCase(string input)
        {
            var result = ToTitleCase(input);

            // capitalise FITA and GNAS
            result = Regex.Replace(result, @"((F|f)ita)|((G|g)nas)",
                Regex.Match(result, @"((F|f)ita)|((G|g)nas)").Value.ToUpper());
            
            // capitalise roman numerals (we only need to go up to V)
            var pattern = @"(?i) (IV|V?I{0,3})$(?-i)";
            result = Regex.Replace(result, pattern,
                Regex.Match(result, pattern).Value.ToUpper());

            return result;
        }

        public static string LocalisedShortDate(DateTime date)
            => date.ToString("dd/MM/yyyy");

        public string this[string text] => AppResources.ResourceManager.GetString(text, AppResources.Culture);
    }
}