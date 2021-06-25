using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.round.structs;

namespace TheScoreBook.models.round
{
    public class Round : IToJson
    {
        private RoundData RoundData { get; }
        public Distance[] Distances { get; }
        public int DistanceCount => RoundData.DistanceCount;
        public DateTime Date { get; }
        public string RoundName => LocalisationManager.ToRoundTitleCase(RoundData.Name);
        public Location Location => RoundData.Location;
        public Style Style { get; }

        public int MaxScore => Distances.Sum(d => d.MaxScore);
        public int MaxShots => RoundData.MaxShots;

        public ScoringType ScoringType => RoundData.ScoringType;

        public Round(string round) : this(round, Style.RECURVE, DateTime.Now) { }
        public Round(string round, Style style) : this(round, style, DateTime.Now) { }
        public Round(string round, Style style, DateTime date) : this(Rounds.Instance.GetRound(round))
        {
            Distances = RoundData.Distances.Select(d => new Distance(d, PreCPX11Style(date, style)) ).ToArray();
            // Distances = RoundData.Distances.Select(d => new Distance(d, style) ).ToArray();

            Style = style;
            Date = date;
        }

        public Round(JObject roundData) : this(Rounds.Instance.GetRound(roundData["rName"].Value<string>()))
        {
            Date = DateTime.FromBinary(roundData["date"].Value<long>());
            Style = (Style)roundData["rStyle"]?.Value<int>() ?? Style.RECURVE;

            var dist = roundData["distances"]!.Value<JArray>();

            Distances = RoundData.Distances.Select( (d, i) => new Distance(d, PreCPX11Style(Date, Style), dist[i].Value<JObject>()))
                .ToArray();
        }

        private Round(RoundData data)
        {
            RoundData = data;
        }

        private Style PreCPX11Style(DateTime roundDate, Style roundStyle) => roundStyle == Style.COMPOUND && roundDate < DateTime.Today ? roundStyle : roundStyle; // TODO: Check when rules changes as may not be needed

        public bool AddScore(int distanceIndex, int endIndex, Score score)
        {
            if (distanceIndex < 0 || distanceIndex >= DistanceCount) return false;
            if (distanceIndex > 0 && !Distances[distanceIndex - 1].AllEndsComplete()) return false;

            return Distances[distanceIndex].AddScore(endIndex, score);
        }

        public bool AllDistancesComplete()
            => Distances.All(d => d.AllEndsComplete());

        public bool DistanceComplete(int distanceIndex)
        {
            if (distanceIndex < 0 || distanceIndex >= DistanceCount) return false;
            return Distances[distanceIndex].AllEndsComplete();
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

        public int Hits()
            => Distances.Sum(d => d.Hits);

        public int CountScore(Score score)
            => Distances.Sum(d => d.CountScore(score));

        public int Score()
            => Distances.Sum(d => d.Score);

        public int Golds()
            => CountScore(enums.Score.X) + CountScore(enums.Score.TEN) + CountScore(enums.Score.NINE);

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
                {"nDistances", DistanceCount},
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