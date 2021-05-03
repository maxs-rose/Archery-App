using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.models
{
    public record SightMark : IToJson
    {
        public int Distance { get; }
        public EDistanceUnit DistanceUnit { get; }
        public float Position { get; }
        public float Notch { get; }

        public SightMark(int distance, EDistanceUnit distanceUnit, float position, float notch)
        {
            Distance = distance;
            DistanceUnit = distanceUnit;
            Position = position;
            Notch = notch;
        }

        public SightMark(JObject json)
        {
            Distance = json["distance"].Value<int>();
            DistanceUnit = json["distanceUnit"].Value<string>().ToEDistanceUnit();
            Position = json["pos"].Value<float>();
            Notch = json["notch"].Value<float>();
        }

        public string ToScoringString()
            => $"{Position} - {Notch}"; 
        
        public JObject ToJson()
        {
            return new JObject()
            {
                {"distance", Distance},
                {"distanceUnit", DistanceUnit.ToString()},
                {"pos", Position},
                {"notch", Notch}
            };
        }

        public override string ToString()
        {
            return $"{Position} - {Notch} @ {Distance}{DistanceUnit}";
        }
    }
}