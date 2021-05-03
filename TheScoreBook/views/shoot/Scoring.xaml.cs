using System;
using FormsControls.Base;
using TheScoreBook.game;
using TheScoreBook.localisation;
using Xamarin.Forms.Xaml;
using NavigationPage = Xamarin.Forms.NavigationPage;

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
        
        public Scoring()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;

            UpdateScoringUiEvent += UpdateUI;
            UpdateScoringUiEvent += OnDistanceFinished;
            
            UpdateUI(-1, -1);
            
            AddNewDistance();
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

        private void UpdateUI(int distance, int end)
        {
            HitsDisplay.Text = $"{LocalisationManager.Instance["Hits"]}: {Hits.ToString()}";
            GoldDisplay.Text = $"{LocalisationManager.Instance["Golds"]}: {Golds.ToString()}";
            TotalDisplay.Text = $"{LocalisationManager.Instance["Score"]}: {Score.ToString()}";;
        }

        private int previousDistance = 0;
        private void OnDistanceFinished(int distance, int end)
        {
            if (GameManager.AllDistancesComplete)
                return;

            if (NextDistanceIndex <= previousDistance)
                return;
            
            previousDistance = NextDistanceIndex;
            AddNewDistance();
        }
        
        private void AddNewDistance()
        {
            DistanceDisplay.Children.Add(new DistanceDisplay(NextDistanceIndex) { HasShadow = false} );
        }
    }
}