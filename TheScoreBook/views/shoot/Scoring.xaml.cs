using System;
using FormsControls.Base;
using TheScoreBook.game;
using TheScoreBook.localisation;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TheScoreBook.views.shoot
{
    public partial class Scoring : AnimationPage
    {
        public int Hits => GameManager.Hits();
        public int Golds => GameManager.Golds();
        public int Score => GameManager.Score();

        private int NextDistanceIndex => GameManager.NextDistanceIndex();
        private int NextEndIndex => GameManager.NextEndIndex(NextDistanceIndex);
        
        public delegate void UpdateScoringUI();

        public static UpdateScoringUI UpdateScoringUiEvent;
        
        public Scoring()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;

            UpdateScoringUiEvent += UpdateUI;
            UpdateScoringUiEvent += OnDistanceFinished;
            
            UpdateUI();
            
            DistanceDisplay.Children.Add(new DistanceDisplay(NextDistanceIndex));
        }

        ~Scoring()
        {
            UpdateScoringUiEvent -= UpdateUI;
            UpdateScoringUiEvent -= OnDistanceFinished;
        }

        private void OnFinishButtonClicked(object sender, EventArgs e)
        {
            GameManager.FinishRound();
            Navigation.RemovePage(Navigation.NavigationStack[^2]);
            ((GeneralContainer) Navigation.NavigationStack[^2]).AddFinishedPage();
            Navigation.PopAsync(true);
        }

        protected override bool OnBackButtonPressed()
        {
            GameManager.FinishRound(false);
            return base.OnBackButtonPressed();
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            GameManager.FinishRound(false);
            Navigation.PopAsync(true);
        }

        private void UpdateUI()
        {
            HitsDisplay.Text = $"{LocalisationManager.Instance["Hits"]}: {Hits.ToString()}";
            GoldDisplay.Text = $"{LocalisationManager.Instance["Golds"]}: {Golds.ToString()}";
            TotalDisplay.Text = $"{LocalisationManager.Instance["Score"]}: {Score.ToString()}";;
        }

        private int previousDistance = 0;
        private void OnDistanceFinished()
        {
            if (GameManager.AllDistancesComplete)
                return;
            
            if (NextDistanceIndex > previousDistance)
            {
                previousDistance = NextDistanceIndex;
                DistanceDisplay.Children.Add(new DistanceDisplay(NextDistanceIndex));
            }
        }
    }
}