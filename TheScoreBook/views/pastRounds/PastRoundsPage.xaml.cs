using System;
using System.Linq;
using System.Threading.Tasks;
using TheScoreBook.acessors;
using TheScoreBook.Views;
using Xamarin.Forms;

namespace TheScoreBook.views.pastRounds
{
    public partial class PastRoundsPage : Frame
    {
        private bool showingAll;

        public PastRoundsPage()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => scoreView.Content = await LoadRounds());
        }

        async Task<StackLayout> LoadPBs()
        {
            // TODO: Possibly make regeneration of this triggered by a change event in the user data instead of every time it is viewed for better performance?
            
            var pbStack = new StackLayout();
            
            foreach (var r in UserData.Instance.GetPB().OrderByDescending(r => r.Date))
                pbStack.Children.Add(new RoundCard(r));

            pbStack.Spacing = 0;

            
            showingAll = false;
            return pbStack;
        }

        

        async Task<StackLayout> LoadRounds()
        {
            var roundStack = new StackLayout();
            var rounds = UserData.Rounds
                .OrderByDescending(r => r.Date)
                .GroupBy(r => new {r.Date.Month, r.Date.Year})
                .Select(g => 
                    g.OrderByDescending(r => r.Date));

            foreach (var gRound in rounds)
            {
                roundStack.Children.Add(new RoundCardMonthHeader(gRound.First().Date));
                
                foreach(var r in gRound)
                    roundStack.Children.Add(new RoundCard(r));
            }
                
            
            roundStack.Spacing = 0;
            
            showingAll = true;
            return roundStack;
        }

        async void Lang_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangeLanguagePage());
        }

        private void ShowAllPast_OnClicked(object sender, EventArgs e)
        {
            if (!showingAll)
                Device.BeginInvokeOnMainThread(async () => scoreView.Content = await LoadRounds());
        }

        private void ShowPBs_OnClicked(object sender, EventArgs e)
        {
            if (showingAll)
                Device.BeginInvokeOnMainThread(async () => scoreView.Content = await LoadPBs());
        }
    }
}