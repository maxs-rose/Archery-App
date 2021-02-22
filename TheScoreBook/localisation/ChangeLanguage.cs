using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using TheScoreBook.data.lang;
using Xamarin.Forms;

namespace TheScoreBook.localisation
{
    public class ChangeLanguage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Attatch picker to this to select languages
        public ObservableCollection<Language> Languages { get; set; }
        public Language SelectedLanguage { get; set; }

        // Attach page button to this to change the language
        public ICommand ChangeLanguageCommand { get; }

        public ChangeLanguage()
        {
            LoadLanguage();
            ChangeLanguageCommand = new Command(async () =>
            {
                LocalisationManager.Instance.SetCulture(CultureInfo.GetCultureInfo(SelectedLanguage.CI));
                LoadLanguage();
                await App.Current.MainPage.DisplayAlert(AppResources.LanguageChanged, "", AppResources.Done);
            });
        }

        void LoadLanguage()
        {
            Languages = new ObservableCollection<Language>()
            {
                {new Language("English", "en") },
                {new Language("French", "fr") }
            };
            SelectedLanguage = Languages.FirstOrDefault(pro => pro.CI == LocalisationManager.Instance.CurrentCulture.TwoLetterISOLanguageName);
        }
    }
}