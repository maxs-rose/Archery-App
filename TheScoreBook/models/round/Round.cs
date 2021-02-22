using System;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using TheScoreBook.acessors;
using TheScoreBook.exceptions;
using TheScoreBook.models.enums;

namespace TheScoreBook.models.round
{
    public class Round : IToJson
    {
        public Distance[] Distances { get; }
        public int DistanceCount { get; }
        public DateTime Date { get; }
        public string RoundName { get; }

        public Round(string round)
        {
            if (!Rounds.Instance.Keys.Contains(round.ToLower()))
                throw new InvalidRoundException($"{round} is not a valid round");

            var roundConstructor = Rounds.Instance.data[round.ToLower()];
            DistanceCount = roundConstructor.Count;
            Distances = new Distance[DistanceCount];

            for (var i = 0; i < DistanceCount; i++)
            {
                var dist = roundConstructor[i]!.Value<JObject>();
                Distances[i] = new Distance(dist!["distance"]!.Value<int>(), dist["unit"]!.Value<string>().ToEDistanceUnit(), dist["ends"]!.Value<int>(), dist["arrowsPerEnd"]!.Value<int>());
            }

            Date = DateTime.Now;
            RoundName = round;
        }

        public Round(JObject roundData)
        {
            DistanceCount = roundData["nDistances"].Value<int>();
            Distances = new Distance[DistanceCount];

            Date = DateTime.FromBinary(roundData["date"].Value<long>());
            RoundName = roundData["rName"].Value<string>();
            
            var dist = roundData["distances"]!.Value<JArray>();

            for (var i = 0; i < DistanceCount; i++)
                Distances[i] = new Distance(dist[i].Value<JObject>());
        }

        public bool AddScore(int distanceIndex, int endIndex, EScore score)
        {
            if (distanceIndex < 0 || distanceIndex >= DistanceCount) return false;
            if (distanceIndex > 0 && !Distances[distanceIndex - 1].AllEndsComplete()) return false;

            return Distances[distanceIndex].AddScore(endIndex, score);
        }

        public bool RemoveScore(int distanceIndex, int endIndex, int scoreIndex)
        {
            if (distanceIndex < 0 || distanceIndex >= DistanceCount) return false;

            return Distances[distanceIndex].RemoveScore(endIndex, scoreIndex);
        }

        public bool ChangeScore(int distanceIndex, int endIndex, int scoreIndex, EScore score)
        {
            if (distanceIndex < 0 || distanceIndex >= DistanceCount) return false;

            return Distances[distanceIndex].ChangeScore(endIndex, scoreIndex, score);
        }

        public bool AllDistancesComplete()
            => Distances.All(d => d.AllEndsComplete());

        public bool DistanceComplete(int distanceIndex)
            => Distances[distanceIndex].AllEndsComplete();

        public bool DistanceEndComplete(int distanceIndex, int endIndex)
            => Distances[distanceIndex].EndComplete(endIndex);

        public int NextDistanceIndex()
        {
            for (var i = 0; i < DistanceCount; i++)
                if (!DistanceComplete(i))
                    return i;
            
            return -1;
        }

        public int NextEndIndex(int distanceIndex)
            => Distances[distanceIndex].NextEndIndex();
        
        public int Hits()
            => Distances.Sum(d => d.Hits());

        public int Hits(int distanceIndex)
            => Distances[distanceIndex].Hits();

        public int Hits(int distanceIndex, int endIndex)
            => Distances[distanceIndex].Hits(endIndex);

        public int CountScore(EScore score)
            => Distances.Sum(d => d.CountScore(score));

        public int CountScore(int distanceIndex, EScore score)
            => Distances[distanceIndex].CountScore(score);

        public int CountScore(int distanceIndex, int endIndex, EScore score)
            => Distances[distanceIndex].CountScore(endIndex, score);

        public int Score()
            => Distances.Sum(d => d.Score());

        public int Score(int distanceIndex)
            => Distances[distanceIndex].Score();

        public int Score(int distanceIndex, int endIndex)
            => Distances[distanceIndex].Score(endIndex);
        
        public int Golds()
            => CountScore(EScore.X) + CountScore(EScore.TEN) + CountScore(EScore.NINE);

        public int Golds(int distanceIndex)
            => Distances[distanceIndex].Golds();

        public int Golds(int distanceIndex, int endIndex)
            => Distances[distanceIndex].Golds(endIndex);

        public JObject ToJson()
        {
            var json = new JObject()
            {
                {"distances", new JArray(Distances.Select(d => d.ToJson()))},
                {"nDistances", DistanceCount},
                {"date", Date.ToBinary()},
                {"rName", RoundName}
            };           
            
            return json;
        }
    }
}