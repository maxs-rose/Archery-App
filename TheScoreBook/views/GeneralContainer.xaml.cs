using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheScoreBook.views.pastRounds;
using TheScoreBook.views.shoot;
using TheScoreBook.views.user;
using Xamarin.Forms;

namespace TheScoreBook.views
{
    public partial class GeneralContainer : ContentPage
    {
        private int currentPage = 0;
        public GeneralContainer()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Layout.Children.Add(new PastRoundsPage());
            // Layout.Children.Add(new UserPage());
            
            BottomBar.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Up,
                Command = new Command(() => ShootButtonOnClicked(null, null))
            });
            
            BottomBar.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Left,
                Command = new Command(() => ScoreButtonOnClicked(null, null))
            });
            
            BottomBar.GestureRecognizers.Add(new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Right,
                Command = new Command(() => ProfileButtonOnClicked(null, null))
            });
        }

        public async void ScoreButtonOnClicked(object sender, EventArgs e)
        {
            if (currentPage == 0)
                return;

            Layout.Children.Insert(0, new PastRoundsPage());
            await TransitionInOut(Layout.Children[1], Width, Layout.Children[0], -Width, 0);
            Layout.Children.RemoveAt(1);
            currentPage = 0;
        }

        public async void ProfileButtonOnClicked(object sender, EventArgs e)
        {
            if (currentPage == 1)
                return;

            Layout.Children.Insert(0, new UserPage());
            await TransitionInOut(Layout.Children[1], -Width, Layout.Children[0], Width, 0);
            Layout.Children.RemoveAt(1);
            currentPage = 1;
        }

        private async Task TransitionInOut(VisualElement leaving, double lTo, VisualElement entering, double eFrom, double eTo, uint animationLength = 375)
        {
            leaving.IsEnabled = false;
            entering.TranslationX = eFrom;
            entering.Opacity = 0;

            // having them all the same length means all 4 should finish at the same time as the awaited operation
            leaving.TranslateTo(lTo, 0, animationLength, Easing.SinIn);
            leaving.FadeTo(0, animationLength, Easing.SinIn);
            entering.FadeTo(1, animationLength, Easing.SinOut);
            await entering.TranslateTo(eTo, 0, animationLength + 3, Easing.SinOut); // add a little extra time to the awaited operation to account for any overhead
        }

        public void AddFinishedPage()
        {
            Layout.Children.RemoveAt(0);
            Layout.Children.Add(new FinishedRound());
            currentPage = 2;
        }

        private async void ShootButtonOnClicked(object sender, EventArgs e)
        {
            if (Navigation.NavigationStack.OfType<RoundSelectionPage>().Any())
                return;
            await Navigation.PushAsync(new RoundSelectionPage());
        }

        protected override bool OnBackButtonPressed()
        {
            if(currentPage == 1)
                ScoreButtonOnClicked(null, null);
            else
                return base.OnBackButtonPressed();

            return true;
        }
    }
}