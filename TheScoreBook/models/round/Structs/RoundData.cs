using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.models.round.Structs
{
    public readonly struct RoundData
    {
        public string Name { get; }
        public RoundGrouping group { get; }
        public ScoringType ScoringType { get; }
        public Location Location { get; }
        
        public int DistanceCount { get; }
        public DistanceData[] Distances { get; }
        
        public int MaxScore { get; }
        public int MaxShots { get; }

        public RoundData(JProperty roundJson)
        {
            Name = roundJson.Name;
            var roundJObject = roundJson.Value.Value<JObject>()!;
            Location = roundJObject["distances"]![0]!["location"]!.Value<string>() == "in" ? Location.INDOOR : Location.OUTDOOR;

            if (roundJObject!.TryGetValue("sorting", out var groupName))
                group = (RoundGrouping) groupName.Value<string>();
            else
                group = RoundGrouping.Other;

            ScoringType = (ScoringType)roundJObject["scoringType"]!.Value<string>();

            var distances = roundJObject["distances"]!.Value<JArray>();
            DistanceCount = distances!.Count;
            Distances = new DistanceData[DistanceCount];

            for (var i = 0; i < DistanceCount; i++)
            {
                if(!(distances[i].Value<JObject>()!.ContainsKey("scoringType")))
                    distances[i].Value<JObject>()!.Add("scoringType", roundJObject["scoringType"]!);
                
                Distances[i] = new DistanceData(distances[i].Value<JObject>());
            }
            
            MaxScore = Distances.Sum(d => d.MaxScore);
            MaxShots = Distances.Sum(d => d.MaxShots);
        }
    }
}