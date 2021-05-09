using System;
using System.Collections.ObjectModel;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.localisation;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.user
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPopup : PopupPage
    {
        private ChangeLanguage languageChanger = new();
        public ObservableCollection<Language> Languages => languageChanger.Languages;

        public Language SelectedLanguage
        {
            get => languageChanger.SelectedLanguage;
            set => languageChanger.SelectedLanguage = value;
        }

        public SettingsPopup()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void CloseSettings(object o, EventArgs e)
        {
            if (LocalisationManager.Instance.CurrentCulture.TwoLetterISOLanguageName !=
                languageChanger.SelectedLanguage.CI)
                languageChanger.ChangeLanguageCommand.Execute(this);

            PopupNavigation.Instance.PopAsync();
        }
    }
}