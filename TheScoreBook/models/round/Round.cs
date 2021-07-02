using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;
using TheScoreBook.models.round.Structs;

namespace TheScoreBook.models.round
{
    public class Round : IToJson
    {
        private RoundData RoundData { get; }
        public Distance[] Distances { get; }
        public int DistanceCount { get; }
        public DateTime Date { get; }
        public string RoundName => RoundData.Name;
        public Location Location { get; }
        public Style Style { get; }

        public int MaxScore => Distances.Sum(d => d.MaxScore);
        public int MaxShots { get; }

        public ScoringType ScoringType => RoundData.ScoringType;
        
        public int Score => Distances.Sum(d => d.Score);
        public int Hits => Distances.Sum(d => d.Hits);
        public int Golds => CountScore(enums.Score.X) + CountScore(enums.Score.TEN) + CountScore(enums.Score.NINE);
        
        public Round(RoundData round) : this(round, Style.RECURVE) { }
        public Round(RoundData round, Style style) : this(round, style, DateTime.Now) { }
        public Round(RoundData round, Style style, DateTime date)
        {
            RoundData = round;
            
            Distances = RoundData.Distances.Select(d => new Distance(d, PreCPX11Style(date, style)) ).ToArray();
            // Distances = RoundData.Distances.Select(d => new Distance(d, style) ).ToArray();

            MaxShots = RoundData.MaxShots;
            Location = RoundData.Location;
            DistanceCount = RoundData.DistanceCount;
            Style = style;
            Date = date;
        }

        public Round(RoundData staticRoundData, JObject roundData) : this(staticRoundData)
        {
            Date = DateTime.FromBinary(roundData["date"].Value<long>());
            Style = (Style)roundData["rStyle"]?.Value<int>() ?? Style.RECURVE;

            var dist = roundData["distances"]!.Value<JArray>();

            Distances = RoundData.Distances.Select( (d, i) => new Distance(d, PreCPX11Style(Date, Style), dist[i].Value<JObject>()))
                .ToArray();
        }

        private Style PreCPX11Style(DateTime roundDate, Style roundStyle) => roundStyle == Style.COMPOUND && roundDate < DateTime.Today ? roundStyle : roundStyle; // TODO: Check when rules changes as may not be needed

        public bool AddScore(int distanceIndex, int endIndex, Score score)
        {
            if (distanceIndex < 0 || distanceIndex >= DistanceCount) return false;
            if (distanceIndex > 0 && !Distances[distanceIndex - 1].AllEndsComplete) return false;

            return Distances[distanceIndex].AddScore(endIndex, score);
        }

        public bool AllDistancesComplete => Distances.All(d => d.AllEndsComplete);

        public bool DistanceComplete(int distanceIndex)
        {
            if (distanceIndex < 0 || distanceIndex >= DistanceCount) return false;
            return Distances[distanceIndex].AllEndsComplete;
        }

        public int NextDistanceIndex()
        {
            for (var i = 0; i < DistanceCount; i++)
                if (!DistanceComplete(i))
                    return i;

            return -1;
        }

        public int NextEndIndex(int distanceIndex)
        {
            if (Distances.Length > distanceIndex && distanceIndex >= 0)
                return Distances[distanceIndex].NextEndIndex();
            return -1;
        }

        public int CountScore(Score score)
            => Distances.Sum(d => d.CountScore(score));

        public void Finish()
        {
            foreach (var dist in Distances)
                dist.Finish();
        }

        public void Finish(int distanceIndex, int endIndex)
        {
            Distances[distanceIndex].Finish(endIndex);
        }

        public JObject ToJson()
        {
            var json = new JObject
            {
                {"distances", new JArray(Distances.Select(d => d.ToJson()))},
                {"date", Date.ToBinary()},
                {"rName", RoundData.Name},
                {"rStyle", Style.Id}
            };

            return json;
        }

        public void ClearEnd(int distanceIndex, int endIndex)
        {
            Distances[distanceIndex].ClearEnd(endIndex);
        }
    }
}