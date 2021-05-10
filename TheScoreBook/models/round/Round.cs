using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.acessors;
using TheScoreBook.exceptions;
using TheScoreBook.models.enums;
using TheScoreBook.models.enums.enumclass;

namespace TheScoreBook.models.round
{
    public class Round : IToJson
    {
        public Distance[] Distances { get; }
        public int DistanceCount { get; }
        public DateTime Date { get; }
        public string RoundName { get; }
        public ELocation Location { get; }
        public Style Style { get; }

        public int MaxScore { get; }
        public int MaxShots { get; }

        public ScoringType ScoringType { get; }

        public Round(string round, Style style, DateTime date)
        {
            if (!Rounds.Instance.Keys.Contains(round.ToLower()))
                throw new InvalidRoundException($"{round} is not a valid round");

            var roundConstructor = Rounds.Instance.data[round.ToLower()];
            var distances = roundConstructor["distances"]!.Value<JArray>();
            DistanceCount = distances!.Count;
            Distances = new Distance[DistanceCount];
            ScoringType = (ScoringType) roundConstructor["scoringType"]!.Value<string>();

            for (var i = 0; i < DistanceCount; i++)
            {
                var dist = distances[i]!.Value<JObject>();
                Distances[i] = new Distance(dist!["distance"]!.Value<int>(),
                    dist["unit"]!.Value<string>().ToEDistanceUnit(), dist["ends"]!.Value<int>(),
                    dist["arrowsPerEnd"]!.Value<int>(), dist["targetSize"]!.Value<int>(),
                    dist["targetUnit"]!.Value<string>().ToEDistanceUnit(), ScoringType);
            }

            Style = style;
            Date = date;
            RoundName = round;
            Location = Rounds.Instance.roundLocation(round);
            MaxScore = Distances.Sum(d => d.MaxScore);
            MaxShots = Distances.Sum(d => d.MaxShots);
        }

        public Round(string round) : this(round, Style.RECURVE, DateTime.Now) { }

        public Round(JObject roundData)
        {
            DistanceCount = roundData["nDistances"].Value<int>();
            Distances = new Distance[DistanceCount];

            Date = DateTime.FromBinary(roundData["date"].Value<long>());
            RoundName = roundData["rName"].Value<string>();

            var dist = roundData["distances"]!.Value<JArray>();

            for (var i = 0; i < DistanceCount; i++)
                Distances[i] = new Distance(dist[i].Value<JObject>());

            Location = Rounds.Instance.roundLocation(RoundName);
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
                {"rName", RoundName}
            };

            return json;
        }

        public void ClearEnd(int distanceIndex, int endIndex)
        {
            Distances[distanceIndex].ClearEnd(endIndex);
        }
    }
}