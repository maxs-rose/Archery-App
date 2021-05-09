using System;
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

        public string Hits { get; }
        public string Golds { get; }
        public string Total { get; }

        public PastRound(Round round)
        {
            InitializeComponent();
            Round = round;

            Hits = $"{LocalisationManager.Instance["Hits"]}: {round.Hits()}";
            Golds = $"{LocalisationManager.Instance["Golds"]}: {round.Golds()}";
            Total = $"{LocalisationManager.Instance["Score"]}: {round.Score()}";
            ;

            foreach (var distance in round.Distances)
                DistanceDisplay.Children.Add(new DistanceDisplay(distance) {HasShadow = false});

            BindingContext = this;
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