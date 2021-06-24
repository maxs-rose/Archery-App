using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.acessors;
using TheScoreBook.exceptions;
using TheScoreBook.localisation;
using TheScoreBook.models.enums;
using TheScoreBook.models.enums.enumclass;
using TheScoreBook.models.round.structs;

namespace TheScoreBook.models.round
{
    public class Round : IToJson
    {
        private RoundData roundData;
        public Distance[] Distances { get; }
        public int DistanceCount => roundData.DistanceCount;
        public DateTime Date { get; }
        public string RoundName => LocalisationManager.ToRoundTitleCase(roundData.Name);
        public ELocation Location => roundData.Location;
        public Style Style { get; }

        public int MaxScore => roundData.MaxScore;
        public int MaxShots => roundData.MaxShots;

        public ScoringType ScoringType => roundData.ScoringType;

        public Round(string round, Style style, DateTime date) : this(Rounds.Instance.GetRound(round))
        {
            Distances = roundData.Distances.Select(d => new Distance(d)).ToArray();

            Style = style;
            Date = date;
        }

        public Round(string round) : this(round, Style.RECURVE, DateTime.Now) { }

        public Round(JObject roundData) : this(Rounds.Instance.GetRound(roundData["rName"].Value<string>()))
        {
            Date = DateTime.FromBinary(roundData["date"].Value<long>());
            Style = (Style)roundData["rStyle"]?.Value<int>() ?? Style.RECURVE;

            var dist = roundData["distances"]!.Value<JArray>();

            Distances = this.roundData.Distances.Select(
                (d, i) => new Distance(d, dist[i].Value<JObject>()))
                .ToArray();
        }

        private Round(RoundData data)
        {
            roundData = data;
        }

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
            => CountScore(enums.enumclass.Score.X) + CountScore(enums.enumclass.Score.TEN) + CountScore(enums.enumclass.Score.NINE);

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
                {"rName", roundData.Name},
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