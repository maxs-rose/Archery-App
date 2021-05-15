using System;
using System.Collections.ObjectModel;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models.enums.enumclass;
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
        private OSAppTheme darkSetting = Settings.AppTheme;
        
        public ObservableCollection<Language> Languages => languageChanger.Languages;
        
        public Language SelectedLanguage
        {
            get => languageChanger.SelectedLanguage;
            set => languageChanger.SelectedLanguage = value;
        }

        public ObservableCollection<RescoreType> RescoreTypes => new (EnumClass.GetAll<RescoreType>());

        private RescoreType selectedType = Settings.RescoreType;
        public RescoreType SelectedType
        {
            get => selectedType;
            set => selectedType = value;
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
            
            if (Settings.AppTheme != darkSetting)
            {
                Settings.AppTheme = darkSetting;
                // parent.BackgroundColor = Settings.BackgroundColor;
            }

            Settings.RescoreType = selectedType;
            
            PopupNavigation.Instance.PopAsync();
        }

        private void ColorfulChanged(object sender, CheckedChangedEventArgs e)
        {
            colorSetting = e.Value;
        }

        private void DarkMode(object sender, CheckedChangedEventArgs e)
        {
            darkSetting = e.Value ? OSAppTheme.Dark : OSAppTheme.Light;
        }
    }
}