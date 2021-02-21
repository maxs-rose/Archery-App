using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TheScoreBook.models
{
    public record End
    {
        private List<EScore> scores = new();
        public int MaxEndScores { get; }

        public End(int maxEndScores)
        {
            MaxEndScores = maxEndScores;
        }

        public End(JObject json)
        {
            scores = json["scores"].Value<JArray>()!.Select(s => s.Value<EScore>()).ToList();
            MaxEndScores = json["scoresPerEnd"].Value<int>();
        }

        public bool AddScore(EScore score)
        {
            if (scores.Count >= MaxEndScores) return false;
            
            scores.Add(score);
            return true;
        }
        
        public bool RemoveScore(int index)
        {
            if (index < 0 || index >= scores.Count) return false;
            
            scores.RemoveAt(index);
            return true;
        }

        public bool ChangeScore(int index, EScore score)
        {
            if (index < 0 || index >= scores.Count) return false;

            scores[index] = score;
            return true;
        }

        public bool EndComplete()
            => scores.Count() == MaxEndScores;
        
        public int Hits()
            => scores.Count(s => s != EScore.M);

        public int CountScore(EScore score)
            => scores.Count(s => s == score);

        public int Golds()
            => CountScore(EScore.X) + CountScore(EScore.TEN) + CountScore(EScore.NINE);
        
        public int EndScore()
            => scores.Sum(e => e.GetRealValue());

        public string EndString()
            => $"[{string.Join(",", scores)}]";
        
        public override string ToString()
            => $"max: {MaxEndScores}, complete: {EndComplete()}, scores: {EndString()}, endScore: {EndScore()}]";

        public JObject ToJson()
        {
            var json = new JObject
            {
                {"scores", new JArray(scores)},
                {"score", EndScore()},
                {"hits", Hits()},
                {"golds", Golds()},
                {"x's", CountScore(EScore.X)},
                {"10's", CountScore(EScore.TEN)},
                {"9's", CountScore(EScore.NINE)},
                {"scoresPerEnd", MaxEndScores},
                {"endComplete", EndComplete()}
            };

            return json;
        }
    }
}