using TheScoreBook.localisation;
using TheScoreBook.models.enums;

namespace TheScoreBook.Ui.extensions
{
    public static class StyleExtension
    {
        public static string LocalizedName(this Style style)
            => LocalisationManager.Instance[style.Name] ?? style.Name;
    }
}