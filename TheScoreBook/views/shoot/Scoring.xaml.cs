using System;
using FormsControls.Base;
using TheScoreBook.game;
using TheScoreBook.models.enums;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TheScoreBook.views.shoot
{
    public partial class Scoring : AnimationPage
    {
        public int Hits => GameManager.Instance.CurrentGame.Hits();
        public int Golds => GameManager.Instance.CurrentGame.Golds();
        public int Score => GameManager.Instance.CurrentGame.Score();

        private int NextDistanceIndex => GameManager.Instance.CurrentGame.NextDistanceIndex();
        private int NextEndIndex => GameManager.Instance.CurrentGame.NextEndIndex(NextDistanceIndex);
        
        public delegate void UpdateScoringUI();

        public static UpdateScoringUI UpdateScoringUiEvent;
        
        public Scoring()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;

            UpdateScoringUiEvent += UpdateUI;
            
            DistanceDisplay.Children.Add(new DistanceDisplay(NextDistanceIndex));
        }

        ~Scoring()
        {
            UpdateScoringUiEvent -= UpdateUI;
        }

        private void OnFinishButtonClicked(object sender, EventArgs e)
        {
            GameManager.Instance.FinishRound();
            Navigation.RemovePage(Navigation.NavigationStack[^2]);

            Navigation.PopAsync(true);
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            GameManager.Instance.CurrentGame.AddScore(NextDistanceIndex, NextEndIndex, EScore.TEN);
            UpdateScoringUiEvent();
        }

        private void UpdateUI()
        {
            HitsDisplay.Text = Hits.ToString();
            GoldDisplay.Text = Golds.ToString();
            TotalDisplay.Text = Score.ToString();
        }
    }
}