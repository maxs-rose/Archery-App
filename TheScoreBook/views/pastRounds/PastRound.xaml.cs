using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using TheScoreBook.acessors;
using TheScoreBook.models.round;
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
        }


        private void DeleteButtonClicked(object sender, EventArgs e)
        {
            UserData.Instance.DeleteRound(Round);
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}