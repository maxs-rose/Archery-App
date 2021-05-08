using System;
using System.Collections.Generic;
using System.Linq;
using FormsControls.Base;
using Newtonsoft.Json.Bson;
using TheScoreBook.acessors;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using Xamarin.Forms;

namespace TheScoreBook.views.shoot
{
    public partial class RoundSelectionPage : AnimationPage
    {
        public string[] PossibleRounds => Rounds.Instance.Keys.Select(LocalisationManager.ToTitleCase).ToArray();
        public string SelectedRound { get; set; }

        public string[] PossibleStyles => Enum.GetValues(typeof(EStyle)).Cast<EStyle>().Select(s => s.ToDisplayString()).ToArray();
        public int SelectedStyle { get; set; }
        
        public DateTime SelectedDate { get; set; } = DateTime.Now;
        
        public RoundSelectionPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            GameManager.LoadPartlyFinishedRound();
            
            BindingContext = this;
            
            
            if (GameManager.PreviousRoundNotFinished)
            {
                LoadUnfinishedRound();
            }
        }

        private async void LoadUnfinishedRound()
        {
            var cont = await Application.Current.MainPage.DisplayAlert(LocalisationManager.Instance["PrevGameNotFin"], LocalisationManager.Instance["Continue"],
                LocalisationManager.Instance["Yes"], LocalisationManager.Instance["No"]);

            if (cont)
            {
                GameManager.ContinuePreviousGame();
                await Navigation.PushAsync(new Scoring());
                Navigation.RemovePage(this);
            }
        }
        
        private void OnScoresButtonOnClicked(object sender, EventArgs e)
        {
            // we need to do this before the pop since its async
            // if we did it after the pop their is no way of guaranteeing if the previous page is ^1 or ^2 without awaiting 
            ((GeneralContainer)Navigation.NavigationStack[^2]).ScoreButtonOnClicked(null, null);
            Navigation.PopAsync(true);
        }

        private void OnProfileButtonOnClicked(object sender, EventArgs e)
        {
            // we need to do this before the pop since its async
            // if we did it after the pop their is no way of guaranteeing if the previous page is ^1 or ^2 without awaiting
            ((GeneralContainer)Navigation.NavigationStack[^2]).ProfileButtonOnClicked(null, null);
            Navigation.PopAsync(true);
        }

        private async void OnStartButtonOnClicked(object sender, EventArgs e)
        {
            if (!CanStartRound())
                return;
            
            GameManager.StartRound(SelectedRound.ToLower(), PossibleStyles[SelectedStyle].ToEStyle(), SelectedDate);
            await Navigation.PushAsync(new Scoring());
            Navigation.RemovePage(this);
        }

        private bool CanStartRound()
            => RoundPicker.SelectedIndex != -1 && !GameManager.GameInProgress;
        
        protected override bool OnBackButtonPressed()
        {
            GameManager.FinishRound(false);
            Navigation.PopAsync(true);
            return true;
        }

        private void AddBorderToStart()
        {
            StartButton.Padding = new Thickness(10);
            StartButton.BorderColor = Color.Goldenrod;
        }

        private void OnRoundSelected(object sender, EventArgs e)
        {
            if (!CanStartRound())
                return;
            
            AddBorderToStart();
            DisplayRoundData();
        }

        private void DisplayRoundData()
        {
            var round = new Round(SelectedRound.ToLower());
            TotalArrows.Text = $"{LocalisationManager.Instance["TotalArrows"]}: {round.MaxShots}";
            TotalScore.Text = $"{LocalisationManager.Instance["MaxScore"]}: {round.MaxScore}";
            Location.Text = $"{LocalisationManager.Instance["Location"]}: {LocalisationManager.Instance[round.Location.ToString()[0] + round.Location.ToString().ToLower()[1..]]}";
            ScoringType.Text = $"{LocalisationManager.Instance["ScoringType"]}: {round.ScoringType}";
            
            RoundInformation.Children.Clear();

            for (var i = 0; i < round.Distances.Length; i++)
                RoundInformation.Children.Add(new DistanceDataDisplay(round.Distances[i], i));
        }
    }
}