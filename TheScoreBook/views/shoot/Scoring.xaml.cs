using System;
using FormsControls.Base;
using TheScoreBook.game;
using TheScoreBook.localisation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Scoring : AnimationPage
    {
        public int Hits => GameManager.Hits();
        public int Golds => GameManager.Golds();
        public int Score => GameManager.Score();

        private int NextDistanceIndex => GameManager.NextDistanceIndex();
        private int NextEndIndex => GameManager.NextEndIndex(NextDistanceIndex);

        public delegate void UpdateScoringUI(int distance, int end);

        public static UpdateScoringUI UpdateScoringUiEvent;

        private int previousDistance = 0;

        public Scoring()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;

            RoundTitle.Text = GameManager.RoundName();

            UpdateScoringUiEvent += UpdateUI;
            UpdateScoringUiEvent += OnDistanceFinished;

            UpdateUI(-1, -1);

            for (var i = 0; i < NextDistanceIndex && GameManager.GetDistance(i).AllEndsComplete(); i++)
            {
                AddNewDistance(i);
                ((DistanceDisplay) DistanceDisplay.Children[i]).AddDistanceTotalsUI();
            }

            AddNewDistance();
        }

        ~Scoring()
        {
            GameManager.SavePartlyFinishedRound();
            UpdateScoringUiEvent -= UpdateUI;
            UpdateScoringUiEvent -= OnDistanceFinished;
        }

        private void OnFinishButtonClicked(object sender, EventArgs e)
        {
            GameManager.FinishRound();
            ((GeneralContainer) Navigation.NavigationStack[^2]).AddFinishedPage();
            Navigation.PopAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            // os nav back button
            if (GameManager.GameInProgress && !GameManager.AllDistancesComplete)
                GameManager.SavePartlyFinishedRound();

            GameManager.ClearPartialRound();
            Navigation.PopAsync(true);

            return true;
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            GameManager.FinishRound(false);
            Navigation.PopAsync(true);
            // back button in app
        }

        private void UpdateUI(int distance, int end)
        {
            HitsDisplay.Text = $"{LocalisationManager.Instance["Hits"]}: {Hits}";
            GoldDisplay.Text = $"{LocalisationManager.Instance["Golds"]}: {Golds}";
            TotalDisplay.Text = $"{LocalisationManager.Instance["Score"]}: {Score}";
            ;

            if (GameManager.AllDistancesComplete)
                FinishedButton.BorderColor = FinishedButton.TextColor = Color.DarkGreen;
        }

        private void OnDistanceFinished(int distance, int end)
        {
            if (distance >= 0 && distance <= previousDistance && GameManager.GetDistance(distance).AllEndsComplete())
                ((DistanceDisplay) DistanceDisplay.Children[distance]).AddDistanceTotalsUI();

            if (GameManager.AllDistancesComplete)
                return;

            if (NextDistanceIndex != previousDistance)
                return;

            previousDistance = NextDistanceIndex;
            AddNewDistance();
        }

        private void AddNewDistance()
        {
            AddNewDistance(NextDistanceIndex);
        }

        private void AddNewDistance(int distanceIndex)
        {
            previousDistance++;
            DistanceDisplay.Children.Add(new DistanceDisplay(distanceIndex, ScoreInputScroll) {HasShadow = false});
        }
    }
}