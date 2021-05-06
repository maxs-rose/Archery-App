using TheScoreBook.localisation;

namespace TheScoreBook.models.enums
{
    public enum EStyle
    {
        RECURVE,
        BAREBOW,
        COMPOUND,
        LONGBOW,
        AFB,
        OTHER
    }

    public static class EStyleHelpers
    {
        public static EStyle ToEStyle(this string s)
            => s.ToUpper() switch
            {
                "RECURVE" => EStyle.RECURVE,
                "BAREBOW" => EStyle.BAREBOW,
                "COMPOUND" => EStyle.COMPOUND,
                "LONGBOW" => EStyle.LONGBOW,
                "AFB" => EStyle.AFB,
                "OTHER" => EStyle.OTHER
            };

        public static string ToDisplayString(this EStyle s)
            => s switch
            {
                EStyle.AFB => "AFB",
                _ => LocalisationManager.ToTitleCase(s.ToString().ToLower())
            };
    }
}