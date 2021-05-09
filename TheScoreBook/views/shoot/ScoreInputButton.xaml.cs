using TheScoreBook.localisation;
using TheScoreBook.models.enums;
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
            if (Score != null)
            {
                if (Score == Score.X || Score == Score.TEN || Score == Score.NINE)
                    ColourFrame.BackgroundColor = Color.Yellow;
                else if (Score == Score.EIGHT || Score == Score.SEVEN)
                    ColourFrame.BackgroundColor = Color.Red;
                else if (Score == Score.SIX || Score == Score.FIVE)
                    ColourFrame.BackgroundColor = Color.Blue;
                else if (Score == Score.FOUR || Score == Score.THREE || Score == Score.MISS)
                    ColourFrame.BackgroundColor = Color.White;
                else if (Score == Score.TWO || Score == Score.ONE)
                    ColourFrame.BackgroundColor = Color.Black;
                else
                    ColourFrame.BackgroundColor = Color.Gray;
            }
            else
                ColourFrame.BackgroundColor = Color.Gray;

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