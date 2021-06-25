using TheScoreBook.acessors;
using TheScoreBook.localisation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinishedRound : Frame
    {
        public string RoundName { get; }
        public string Date { get; }
        public string Bow { get; }
        public int Score { get; }

        public FinishedRound()
        {
            InitializeComponent();

            RoundName = LocalisationManager.ToTitleCase(UserData.LatestRound.RoundName);
            Date = LocalisationManager.LocalisedShortDate(UserData.LatestRound.Date);
            Bow = UserData.LatestRound.Style.ToString();
            Score = UserData.LatestRound.Score;

            BindingContext = this;
        }
    }
}