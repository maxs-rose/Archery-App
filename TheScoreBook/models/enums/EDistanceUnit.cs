using System;

namespace TheScoreBook.models.enums
{
    public enum EDistanceUnit
    {
        yd,
        m,
        cm
    }

    public static class EDistanceUnitHelpers
    {
        public static EDistanceUnit ToEDistanceUnit(this string s)
            => s.ToLower() switch
            {
                "m" => EDistanceUnit.m,
                "yd" => EDistanceUnit.yd,
                "cm" => EDistanceUnit.cm,
                _ => throw new InvalidCastException()
            };
    }
}