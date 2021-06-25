using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using Xamarin.Forms;

namespace TheScoreBook.views.user
{
    public partial class UserPage : Frame
    {
        public UserPage()
        {
            InitializeComponent();

            Task.Run(GenerateSightMarks);
            Task.Run(GeneratePreferedRounds);
            Task.Run(GenerateBestRounds);

            SetupPopups();

            UserData.SightMarksUpdatedEvent += GenerateSightMarks;
        }

        private void SetupPopups()
        {
            OpenAddNewSightMark.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnAddNewSightMarkTapped)
            });
        }

        ~UserPage()
        {
            UserData.SightMarksUpdatedEvent -= GenerateSightMarks;
        }

        private void OnAddNewSightMarkTapped()
        {
            PopupNavigation.Instance.PushAsync(new CreateSightMarkPopup());
        }

        private void SettingsButtonClicked(object o, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new SettingsPopup(this));
        }

        private void GenerateSightMarks()
        {
            SightMarks.Children.Clear();

            foreach (var mark in UserData.SightMarks)
            {
                var label = new Label
                {
                    Text = $"{mark.Position} - {mark.Notch} | {mark.Distance}{mark.DistanceUnit.ToString()}",
                    Margin = 0,
                    Padding = 0,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = new Command(() =>
                            {
                                if (PopupNavigation.Instance.PopupStack.Count < 1)
                                    DeleteSightMark(mark);
                            })
                        }
                    }
                };

                SightMarks.Children.Add(label);
            }
        }

        private async void DeleteSightMark(SightMark mark)
        {
            var delete = await Application.Current.MainPage
                .DisplayAlert(LocalisationManager.Instance["DeleteMark"], mark.ToString(),
                    LocalisationManager.Instance["Ok"], LocalisationManager.Instance["Cancel"]);
            if (delete)
                UserData.Instance.DeleteSightMark(mark);
        }

        private void GeneratePreferedRounds()
        {
            // group rounds name -> count number of rounds in each group -> order by count -> round
            var prefR = UserData.Rounds
                .GroupBy(r => r.RoundName)
                .Select(g => (roundName: g.Key, round: g.First(), count: g.Count()))
                .OrderByDescending(t => t.count)
                .Select(rt => rt.round);

            // ordered rounds -> take first round of correct location
            var prefIndoor = prefR.FirstOrDefault(r => r.Location == Location.INDOOR);
            var prefOutdoor = prefR.FirstOrDefault(r => r.Location == Location.OUTDOOR);

            OutdoorRound.BindingContext = IndoorRound.BindingContext = LocalisationManager.Instance;
            IndoorRound.SetBinding(Label.TextProperty, "LanguageChangedNotification",
                converter: new RoundConvertor {Location = "Indoor", Pref = prefIndoor});
            OutdoorRound.SetBinding(Label.TextProperty, "LanguageChangedNotification",
                converter: new RoundConvertor {Location = "Outdoor", Pref = prefOutdoor});
        }

        private void GenerateBestRounds()
        {
            // PB -> Order by score -> round
            var bestRounds = UserData.Instance
                .GetPB()
                .OrderByDescending(r => r.Score);

            // Ordered PB's -> take first of correct location
            var bestIndoor = bestRounds.FirstOrDefault(r => r.Location == Location.INDOOR);
            var bestOutdoor = bestRounds.FirstOrDefault(r => r.Location == Location.OUTDOOR);

            BestOutdoorRound.BindingContext = BestIndoorRound.BindingContext = LocalisationManager.Instance;
            BestIndoorRound.SetBinding(Label.TextProperty, "LanguageChangedNotification",
                converter: new RoundConvertor {Location = "Indoor", Pref = bestIndoor});
            BestOutdoorRound.SetBinding(Label.TextProperty, "LanguageChangedNotification",
                converter: new RoundConvertor {Location = "Outdoor", Pref = bestOutdoor});
        }

        private class RoundConvertor : IValueConverter
        {
            public string Location { get; set; }
            public Round Pref { get; set; }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return
                    $"{LocalisationManager.ToTitleCase(Pref?.RoundName ?? "N/A")} - {LocalisationManager.ToTitleCase(LocalisationManager.Instance[Location])}";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}