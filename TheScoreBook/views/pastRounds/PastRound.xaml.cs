using System;
using System.Linq;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models.round;
using TheScoreBook.views.shoot;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.pastRounds
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PastRound : PopupPage
    {
        private Round Round { get; }
        
        public PastRound(Round round)
        {
            InitializeComponent();
            Round = round;

            HitsDisplay.Text = $"{LocalisationManager.Instance["Hits"]}: {round.Hits()}";
            GoldDisplay.Text = $"{LocalisationManager.Instance["Golds"]}: {round.Golds()}";
            TotalDisplay.Text = $"{LocalisationManager.Instance["Score"]}: {round.Score()}";;
            
            foreach (var distance in round.Distances)
                DistanceDisplay.Children.Add(new DistanceDisplay(distance) { HasShadow = false} );
        }
        
        private void DeleteButtonClicked(object sender, EventArgs e)
        {
            UserData.Instance.DeleteRound(Round);
            PopupNavigation.Instance.PopAsync(true);
        }
        
        private void BackButtonClicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}