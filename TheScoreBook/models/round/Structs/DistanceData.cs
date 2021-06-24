using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.models.round.structs
{
    public readonly struct DistanceData
    {
        public int DistanceLength { get; }
        public MeasurementUnit DistanceUnit { get; }
        
        public int MaxScore { get; }
        public int MaxShots { get; }
        
        public int TargetSize { get; }
        public MeasurementUnit TargetUnit { get; }
        
        public ScoringType ScoringType { get; }

        public int MaxEnds { get; }
        public int ArrowsPerEnd { get; }

        public DistanceData(JObject distanceJObject)
        {
            DistanceLength = distanceJObject["distance"]!.Value<int>();
            DistanceUnit = (MeasurementUnit)distanceJObject["unit"]!.Value<string>();

            TargetSize = distanceJObject["targetSize"]!.Value<int>();
            TargetUnit = (MeasurementUnit)distanceJObject["targetUnit"]!.Value<string>();

            ScoringType = (ScoringType) distanceJObject["scoringType"]?.Value<string>() ?? ScoringType.TenZone;
            
            MaxEnds = distanceJObject["ends"].Value<int>();
            ArrowsPerEnd = distanceJObject["arrowsPerEnd"]!.Value<int>();

            MaxShots = MaxEnds * ArrowsPerEnd;
            MaxScore = MaxShots * ScoringType.MaxScore().Value;
        }
    }
}