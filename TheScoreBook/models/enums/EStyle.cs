using TheScoreBook.localisation;

namespace TheScoreBook.models.enums
{
    public enum EStyle
    {
        RECURVE,
        COMPOUND,
        LONGBOW,
        AFB
    }

    public static class EStyleHelpers
    {
        public static EStyle ToEStyle(this string s)
            => s.ToUpper() switch
            {
                "RECURVE" => EStyle.RECURVE,
                "COMPOUND" => EStyle.COMPOUND,
                "LONGBOW" => EStyle.LONGBOW,
                "AFB" => EStyle.AFB
            };

        public static string ToDisplayString(this EStyle s)
            => s switch
            {
                EStyle.AFB => "AFB",
                _ => LocalisationManager.ToTitleCase(s.ToString().ToLower())
            };
    }
}