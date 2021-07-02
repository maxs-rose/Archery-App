using TheScoreBook.models.round;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheScoreBook.views.shoot
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DistanceDataDisplay : Frame
    {
        private Distance Distance { get; }
        public string RoundIndex { get; }
        public string TargetDistance { get; }
        public string TargetSize { get; }
        public string TotalArrows { get; }
        public string MaxScore { get; }
        public string MaxEnds { get; }
        public string ArrowsPerEnd { get; }

        public DistanceDataDisplay(Distance distance, int index)
        {
            Distance = distance;
            RoundIndex = $"Target {index + 1}";
            TargetDistance = $"{distance.DistanceLength}{distance.DistanceUnit}";
            TargetSize = $"{distance.TargetSize}";
            TotalArrows = $"{distance.MaxShots}";
            MaxScore = $"{distance.MaxScore}";
            MaxEnds = $"{distance.MaxEnds}";
            ArrowsPerEnd = $"{distance.Ends[0].ArrowsPerEnd}";

            InitializeComponent();
            BindingContext = this;
        }
    }
}