using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinishedRound : Frame
    {
        public FinishedRound()
        {
            InitializeComponent();
            RoundName.Text = UserData.LatestRound.RoundName;
            Date.Text = LocalisationManager.LocalisedShortDate(UserData.LatestRound.Date);
            Bow.Text = UserData.LatestRound.Style.ToDisplayString();
            Score.Text = UserData.LatestRound.Score().ToString();
        }
    }
}