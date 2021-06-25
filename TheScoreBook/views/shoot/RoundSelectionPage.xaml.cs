using System;
using System.Linq;
using FormsControls.Base;
using Rg.Plugins.Popup.Services;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.round;
using Xamarin.Forms;
using Style = TheScoreBook.models.enums.Style;

namespace TheScoreBook.views.shoot
{
    public partial class RoundSelectionPage : AnimationPage
    {
        private string selectedRound;
        public string SelectedRound
        {
            get => selectedRound;
            set
            {
                selectedRound = value;
                OnRoundSelected();
            }
        }

        private bool hasPickedRound = false;

        public Style[] PossibleStyles => models.enums.Style.GetAll<Style>().ToArray();

        public int SelectedStyle { get; set; }

        public DateTime SelectedDate { get; set; } = DateTime.Now;

        public RoundSelectionPage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
            GameManager.LoadPartlyFinishedRound();
            
            RoundPicker.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => OpenPicker())});

            BindingContext = this;

            if (GameManager.PreviousRoundNotFinished)
                LoadUnfinishedRound();
        }

        private async void LoadUnfinishedRound()
        {
            var cont = await Application.Current.MainPage.DisplayAlert(LocalisationManager.Instance["PrevGameNotFin"],
                LocalisationManager.Instance["Continue"],
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
            ((GeneralContainer) Navigation.NavigationStack[^2]).ScoreButtonOnClicked(null, null);
            Navigation.PopAsync(true);
        }

        private void OnProfileButtonOnClicked(object sender, EventArgs e)
        {
            // we need to do this before the pop since its async
            // if we did it after the pop their is no way of guaranteeing if the previous page is ^1 or ^2 without awaiting
            ((GeneralContainer) Navigation.NavigationStack[^2]).ProfileButtonOnClicked(null, null);
            Navigation.PopAsync(true);
        }

        private async void OnStartButtonOnClicked(object sender, EventArgs e)
        {
            if (!CanStartRound())
                return;

            GameManager.StartRound(SelectedRound.ToLower(), PossibleStyles[SelectedStyle], SelectedDate);
            await Navigation.PushAsync(new Scoring());
            Navigation.RemovePage(this);
        }

        private bool CanStartRound()
            => hasPickedRound && !GameManager.GameInProgress;

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

        private void OnRoundSelected()
        {
            hasPickedRound = true;
            RoundPicker.Text = SelectedRound;
            
            if (!CanStartRound())
                return;
            
            AddBorderToStart();
            DisplayRoundData();
        }
        private void OnBowStyleChanged(object sender, EventArgs e)
        {
            if(CanStartRound())
                DisplayRoundData();
        }

        private void DisplayRoundData()
        {
            var round = new Round(SelectedRound.ToLower(), PossibleStyles[SelectedStyle], SelectedDate);
            TotalArrows.Text = $"{LocalisationManager.Instance["TotalArrows"]}: {round.MaxShots}";
            TotalScore.Text = $"{LocalisationManager.Instance["MaxScore"]}: {round.MaxScore}";
            Location.Text =
                $"{LocalisationManager.Instance["Location"]}: {LocalisationManager.Instance[round.Location.ToString()[0] + round.Location.ToString().ToLower()[1..]]}";
            ScoringType.Text = $"{LocalisationManager.Instance["ScoringType"]}: {round.ScoringType}";

            RoundInformation.Children.Clear();

            for (var i = 0; i < round.Distances.Length; i++)
                RoundInformation.Children.Add(new DistanceDataDisplay(round.Distances[i], i));
        }

        private void OpenPicker()
        {
            if(PopupNavigation.Instance.PopupStack.Any(p => p.GetType() == typeof(RoundSelectionPopup)))
                return;

            PopupNavigation.Instance.PushAsync(new RoundSelectionPopup(this));
        }
    }
}