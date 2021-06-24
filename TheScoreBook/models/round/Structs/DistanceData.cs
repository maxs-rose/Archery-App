using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;
using TheScoreBook.models.enums.enumclass;

namespace TheScoreBook.models.round.structs
{
    public readonly struct DistanceData
    {
        public int DistanceLength { get; }
        public EDistanceUnit DistanceUnit { get; }
        
        public int MaxScore { get; }
        public int MaxShots { get; }
        
        public int TargetSize { get; }
        public EDistanceUnit TargetUnit { get; }
        
        public ScoringType ScoringType { get; }

        public int MaxEnds { get; }
        public int ArrowsPerEnd { get; }

        public DistanceData(JObject distanceJObject)
        {
            DistanceLength = distanceJObject["distance"]!.Value<int>();
            DistanceUnit = distanceJObject["unit"]!.Value<string>().ToEDistanceUnit();

            TargetSize = distanceJObject["targetSize"]!.Value<int>();
            TargetUnit = distanceJObject["targetUnit"]!.Value<string>().ToEDistanceUnit();

            ScoringType = (ScoringType) distanceJObject["scoringType"]?.Value<string>() ?? ScoringType.TenZone;
            
            MaxEnds = distanceJObject["ends"].Value<int>();
            ArrowsPerEnd = distanceJObject["arrowsPerEnd"]!.Value<int>();

            MaxShots = MaxEnds * ArrowsPerEnd;
            MaxScore = MaxShots * ScoringType.MaxScore().Value;
        }
    }
}