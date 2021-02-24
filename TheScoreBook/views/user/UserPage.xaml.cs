using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models;
using TheScoreBook.models.enums;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

            OpenAddNewSightMark.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(OnAddNewSightMarkTapped)
            });

            UserData.SightMarksUpdatedEvent += GenerateSightMarks;
        }

        ~UserPage()
        {
            UserData.SightMarksUpdatedEvent -= GenerateSightMarks;
        }

        private void OnAddNewSightMarkTapped()
        {
            PopupNavigation.Instance.PushAsync(new CreateSightMarkPopup());
        }

        private void GenerateSightMarks()
        {
            SightMarks.Children.Clear();

            foreach (var mark in UserData.SightMarks)
            {
                var label = new Label
                {
                    Text  = $"{mark.Position} - {mark.Notch} | {mark.Distance}{mark.DistanceUnit.ToString()}",
                    Margin= 0,
                    Padding= 0,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor= Color.Chartreuse
                };
                
                SightMarks.Children.Add(label);
            }
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
            var prefIndoor = prefR.FirstOrDefault(r => r.Location == ELocation.INDOOR);
            var prefOutdoor = prefR.FirstOrDefault(r => r.Location == ELocation.OUTDOOR);
            
            IndoorRound.Text = $"{LocalisationManager.ToTitleCase(prefIndoor?.RoundName ?? "N/A")} - {LocalisationManager.ToTitleCase(LocalisationManager.Instance["Indoor"])}";
            OutdoorRound.Text = $"{LocalisationManager.ToTitleCase(prefOutdoor?.RoundName ?? "N/A")} - {LocalisationManager.ToTitleCase(LocalisationManager.Instance["Outdoor"])}";
        }

        private void GenerateBestRounds()
        {
            // PB -> Order by score -> round
            var bestRounds = UserData.Instance
                .GetPB()
                .OrderByDescending(r => r.Score());

            // Ordered PB's -> take first of correct location
            var bestIndoor = bestRounds.FirstOrDefault(r => r.Location == ELocation.INDOOR);
            var bestOutdoor = bestRounds.FirstOrDefault(r => r.Location == ELocation.OUTDOOR);
            
            BestIndoorRound.Text = $"{LocalisationManager.ToTitleCase(bestIndoor?.RoundName ?? "N/A")} - {LocalisationManager.ToTitleCase(LocalisationManager.Instance["Indoor"])}";
            BestOutdoorRound.Text = $"{LocalisationManager.ToTitleCase(bestOutdoor?.RoundName ?? "N/A")} - {LocalisationManager.ToTitleCase(LocalisationManager.Instance["Outdoor"])}";
        }
    }
}
