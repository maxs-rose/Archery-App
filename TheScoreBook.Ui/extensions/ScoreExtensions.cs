using TheScoreBook.models.enums;
using Xamarin.Forms;

namespace TheScoreBook.Ui.extensions
{
    public static class ScoreExtensions
    {
        public static Color ToColour(this Score s)
        {
            if (s is null)
                return Color.Gray;

            switch (s.Id)
            {
                case 11:
                case 10:
                case 9:
                    return Color.Yellow;
                case 8:
                case 7:
                    return Color.Red;
                case 6:
                case 5:
                    return Color.Blue;
                case 4:
                case 3:
                    return Color.Black;
                default:
                    return Color.Black;
            }
        }
    }
}