using TheScoreBook.localisation;
using TheScoreBook.models.round;

namespace TheScoreBook.Ui.extensions
{
    public static class RoundExtensions
    {
        public static string LocalisedName(this Round round) => LocalisationManager.ToRoundTitleCase(round.RoundName);
    }
}