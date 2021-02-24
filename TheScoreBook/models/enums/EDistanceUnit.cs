using System;
using System.Collections.Generic;

namespace TheScoreBook.models.enums
{
    public enum EDistanceUnit
    {
        yd, m
    }

    public static class EDistanceUnitHelpers
    {
        public static EDistanceUnit ToEDistanceUnit(this string s)
            => s.ToLower() switch
            {
                "m" => EDistanceUnit.m,
                "yd" => EDistanceUnit.yd,
                _ => throw new InvalidCastException()
            };
    }
}