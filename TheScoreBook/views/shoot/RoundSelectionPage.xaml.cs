using System;
using FormsControls.Base;
using Xamarin.Forms;

namespace TheScoreBook.views.shoot
{
    public partial class RoundSelectionPage : AnimationPage
    {
        public RoundSelectionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void OnScoresButtonOnClicked(object sender, EventArgs e)
        {
            ((GeneralContainer)Navigation.NavigationStack[^2]).ScoreButtonOnClicked(null, null);
            Navigation.PopAsync(true);
        }

        private void OnProfileButtonOnClicked(object sender, EventArgs e)
        {
            ((GeneralContainer)Navigation.NavigationStack[^2]).ProfileButtonOnClicked(null, null);
            Navigation.PopAsync(true);
        }
    }
}