using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.user
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPopup : PopupPage
    {
        private Frame parent;
        
        private ChangeLanguage languageChanger = new();
        
        private bool colorSetting = Settings.ColorfulArrows;
        private bool darkSetting = Settings.DarkMode;
        
        public ObservableCollection<Language> Languages => languageChanger.Languages;
        
        public Language SelectedLanguage
        {
            get => languageChanger.SelectedLanguage;
            set => languageChanger.SelectedLanguage = value;
        }

        public SettingsPopup(Frame parent)
        {
            this.parent = parent;
            InitializeComponent();
            BindingContext = this;
        }

        public void CloseSettings(object o, EventArgs e)
        {
            if (LocalisationManager.Instance.CurrentCulture.TwoLetterISOLanguageName !=
                languageChanger.SelectedLanguage.CI)
                languageChanger.ChangeLanguageCommand.Execute(this);

            Settings.ColorfulArrows = colorSetting;
            
            if (Settings.DarkMode != darkSetting)
            {
                Settings.DarkMode = darkSetting;
                parent.BackgroundColor = Settings.BackgroundColor;
            }
            
            PopupNavigation.Instance.PopAsync();
        }

        private void ColorfulChanged(object sender, CheckedChangedEventArgs e)
        {
            colorSetting = e.Value;
        }

        private void DarkMode(object sender, CheckedChangedEventArgs e)
        {
            darkSetting = e.Value;
        }
    }
}