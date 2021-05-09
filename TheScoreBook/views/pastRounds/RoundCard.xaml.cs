using Rg.Plugins.Popup.Services;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;
using Xamarin.Forms;

namespace TheScoreBook.views.pastRounds
{
    public partial class RoundCard : Frame
    {
        private readonly Round round;
        
        public RoundCard(Round round)
        {
            InitializeComponent();

            this.round = round;

            BindingContext = this;
            
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (PopupNavigation.Instance.PopupStack.Count < 1)
                        PopupNavigation.Instance.PushAsync(new PastRound(round));
                })
            });
        }

        public string RoundName => LocalisationManager.ToTitleCase(round.RoundName);
        public string Date => LocalisationManager.LocalisedShortDate(round.Date);

        public int Hits => round.Hits();
        public int Tens => round.CountScore(models.enums.Score.TEN) + round.CountScore(models.enums.Score.X);
        public int Score => round.Score();
    }
}