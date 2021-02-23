using System;
using TheScoreBook.views.pastRounds;
using TheScoreBook.views.user;
using Xamarin.Forms;

namespace TheScoreBook.views
{
    public partial class GeneralContainer : ContentPage
    {
        private bool isCurrentRoundsPage = true;
        public GeneralContainer()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            Layout.Children.Add(new PastRoundsPage());
            // Layout.Children.Add(new UserPage());
        }

        private void ScoreButtonOnClicked(object sender, EventArgs e)
        {
            if (isCurrentRoundsPage)
                return;
            
            Layout.Children.RemoveAt(0);
            Layout.Children.Add(new PastRoundsPage());
            isCurrentRoundsPage = true;
        }

        private void ProfileButtonOnClicked(object sender, EventArgs e)
        {
            if (!isCurrentRoundsPage)
                return;
            
            Layout.Children.RemoveAt(0);
            Layout.Children.Add(new UserPage());
            isCurrentRoundsPage = false;
        }
    }
}