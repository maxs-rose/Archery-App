using TheScoreBook.acessors;
using TheScoreBook.models.enums.enumclass;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EndScoreDisplay : Frame
    {
        private Score score = null;

        public Score Score
        {
            get => score;
            private set
            {
                score = value;
                ScoreLabel.Text = score?.ToString();
                SetBackgroundColor();
            }
        }

        public static readonly BindableProperty ScoreTextProperty = BindableProperty.Create(
            propertyName: "Score",
            returnType: typeof(string),
            declaringType: typeof(EndScoreDisplay),
            defaultValue: "",
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: ScoreTextPropertyChanged);

        private static void ScoreTextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var control = (EndScoreDisplay) bindable;
            control.Score = (Score) newvalue.ToString();
        }

        public EndScoreDisplay()
        {
            InitializeComponent();
            BindingContext = this;
            
            if(!Settings.ColorfulArrows)
                BorderColor = Color.Transparent;
        }

        private void SetBackgroundColor()
        {
            if (!Settings.ColorfulArrows)
                return;
            
            BackgroundColor = (Color) Score;
            if (BackgroundColor == Color.Black || BackgroundColor == Color.Blue)
                ScoreLabel.TextColor = Color.White;
            else
                ScoreLabel.TextColor = Color.Black;
        }
    }
}