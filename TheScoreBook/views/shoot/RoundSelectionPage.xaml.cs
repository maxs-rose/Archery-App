using System;
using System.Collections.Generic;
using System.Linq;
using FormsControls.Base;
using TheScoreBook.acessors;
using TheScoreBook.game;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
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

            BindingContext = this;
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

        private void OnStartButtonOnClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedRound))
                return;
            
            GameManager.Instance.StartRound(SelectedRound.ToLower(), PossibleStyles[SelectedStyle].ToEStyle(), SelectedDate);
            Navigation.PushAsync(new Scoring());
        }
    }
}