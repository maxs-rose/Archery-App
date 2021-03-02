using TheScoreBook.models.enums;
using Xamarin.Forms;

namespace TheScoreBook.views.shoot
{
    public partial class ScoreInputButton : Frame
    {
        private EScore score;
        public EScore Score
        {
            get => score;
            set
            {
                score = value;
                UpdateRingColour();
                UpdateLabelText();
            }
        }

        private string wordString = "";
        public string WordString
        {
            get => wordString;
            set
            {
                wordString = value;
                UpdateLabelText();
            }   
        }

        public ScoreInputButton()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void UpdateRingColour()
        {
            switch (Score)
            {
                case EScore.X:
                case EScore.TEN:
                case EScore.NINE:
                    ColourFrame.BackgroundColor = Color.Yellow;
                    break;
                case EScore.EIGHT:
                case EScore.SEVEN:
                    ColourFrame.BackgroundColor = Color.Red;
                    break;
                case EScore.SIX:
                case EScore.FIVE:
                    ColourFrame.BackgroundColor = Color.Blue;
                    break;
                case EScore.TWO:
                case EScore.ONE:
                    ColourFrame.BackgroundColor = Color.White;
                    break;
                default:
                    ColourFrame.BackgroundColor = Color.Black;
                    break;
            }
        }

        private void UpdateLabelText()
        {
            if(WordString == "")
                ButtonText.Text = score.ToUserString();
            else
                ButtonText.Text = WordString;
        }
    }
}