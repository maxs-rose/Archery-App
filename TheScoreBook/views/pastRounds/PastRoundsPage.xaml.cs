using System;
using System.Linq;
using System.Threading.Tasks;
using TheScoreBook.acessors;
using TheScoreBook.models.round;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.pastRounds
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PastRoundsPage : Frame
    {
        private bool showingAll;
        private StackLayout allRounds = null;
        private StackLayout pbRounds = null;

        public PastRoundsPage()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => ScoreView.Content = await LoadRounds());

            GestureRecognizers.Add(SwipeRightLeft(SwipeDirection.Left, () => ShowAllPast_OnClicked(null, null)));
            GestureRecognizers.Add(SwipeRightLeft(SwipeDirection.Right, () => ShowPBs_OnClicked(null, null)));
            ScoreView.GestureRecognizers.Add(SwipeRightLeft(SwipeDirection.Left,
                () => ShowAllPast_OnClicked(null, null)));
            ScoreView.GestureRecognizers.Add(SwipeRightLeft(SwipeDirection.Right, () => ShowPBs_OnClicked(null, null)));

            ShowAllPast_OnClicked(null, null);
            UserData.RoundsUpdatedEvent += ReloadRounds;
        }

        ~PastRoundsPage()
        {
            UserData.RoundsUpdatedEvent -= ReloadRounds;
        }

        private SwipeGestureRecognizer SwipeRightLeft(SwipeDirection direction, Action function)
            => new()
            {
                Direction = direction,
                Command = new Command(function)
            };

        private async Task<StackLayout> LoadPBs()
        {
            var pbStack = new StackLayout();

            foreach (var r in UserData.Instance.GetPB().OrderByDescending(r => r.Date))
                AddRound(pbStack, r);

            pbStack.Spacing = 0;

            showingAll = false;
            return pbStack;
        }

        private async Task<StackLayout> LoadRounds()
        {
            var roundStack = new StackLayout();
            var rounds = UserData.Rounds
                .OrderByDescending(r => r.Date)
                .GroupBy(r => new {r.Date.Month, r.Date.Year})
                .Select(g =>
                    g.OrderByDescending(r => r.Date));

            foreach (var gRound in rounds)
                AddMonthGroup(gRound, roundStack);

            roundStack.Spacing = 0;

            showingAll = true;
            return roundStack;
        }

        private void AddMonthGroup(IOrderedEnumerable<Round> gRound, StackLayout roundStack)
        {
            var first = true;

            foreach (var r in gRound)
            {
                if (first)
                    AddMonthHeader(roundStack, r.Date);

                AddRound(roundStack, r);
                first = false;
            }
        }

        private void AddMonthHeader(StackLayout roundStack, DateTime date)
        {
            roundStack.Children.Add(new RoundCardMonthHeader(date) {HasShadow = true});
            roundStack.Children.Last().GestureRecognizers
                .Add(SwipeRightLeft(SwipeDirection.Left, () => ShowAllPast_OnClicked(null, null)));
            roundStack.Children.Last().GestureRecognizers
                .Add(SwipeRightLeft(SwipeDirection.Right, () => ShowPBs_OnClicked(null, null)));
        }

        private void AddRound(StackLayout stack, Round round)
        {
            stack.Children.Add(new RoundCard(round) {HasShadow = false});
            stack.Children.Last().GestureRecognizers
                .Add(SwipeRightLeft(SwipeDirection.Left, () => ShowAllPast_OnClicked(null, null)));
            stack.Children.Last().GestureRecognizers
                .Add(SwipeRightLeft(SwipeDirection.Right, () => ShowPBs_OnClicked(null, null)));
        }

        private void ShowAllPast_OnClicked(object sender, EventArgs e)
        {
            ShowPBs.BorderColor = Color.Transparent;
            ShowAllPast.BorderColor = Color.Black;

            if (!showingAll)
                Device.BeginInvokeOnMainThread(async () => ScoreView.Content = await LoadRounds());
        }

        private void ShowPBs_OnClicked(object sender, EventArgs e)
        {
            ShowPBs.BorderColor = Color.Black;
            ShowAllPast.BorderColor = Color.Transparent;

            if (showingAll)
                Device.BeginInvokeOnMainThread(async () => ScoreView.Content = await LoadPBs());
        }

        private void ReloadRounds()
        {
            if (showingAll)
                Device.BeginInvokeOnMainThread(async () => ScoreView.Content = await LoadRounds());
            else
                Device.BeginInvokeOnMainThread(async () => ScoreView.Content = await LoadPBs());
        }
    }
}