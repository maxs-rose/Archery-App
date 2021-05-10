using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.enums.enumclass;
using Xamarin.Forms;

namespace TheScoreBook.views.shoot
{
    public partial class ScoreInputButton : Frame
    {
        private Score score = null;

        public Score Score
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
                UpdateRingColour();
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
            ColourFrame.BackgroundColor = (Color)Score;

            switch (WordString)
            {
                case "X":
                    WordString = LocalisationManager.Instance["Delete"];
                    ColourFrame.BackgroundColor = Color.DarkRed;
                    break;
                case "^":
                    WordString = LocalisationManager.Instance["Tick"];
                    ColourFrame.BackgroundColor = Color.DarkGreen;
                    ButtonText.TextColor = Color.DarkGreen;
                    break;
            }
        }

        private void UpdateLabelText()
        {
            ButtonText.Text = WordString == "" ? score?.ToString() : WordString;
        }
    }
}