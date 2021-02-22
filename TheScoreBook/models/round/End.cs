using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.models.round
{
    public class End : IToJson
    {
        private List<EScore> scores = new();
        public int ArrowsPerEnd { get; }

        public End(int arrowsPerEnd)
        {
            ArrowsPerEnd = arrowsPerEnd;
        }

        public End(JObject json)
        {
            scores = json["scores"].Value<JArray>()!.Select(s => s.Value<EScore>()).ToList();
            ArrowsPerEnd = json["scoresPerEnd"].Value<int>();
        }

        public bool AddScore(EScore score)
        {
            if (scores.Count >= ArrowsPerEnd) return false;
            
            scores.Add(score);
            SortList();
            return true;
        }
        
        public bool RemoveScore(int index)
        {
            if (index < 0 || index >= scores.Count) return false;
            
            scores.RemoveAt(index);
            SortList();
            return true;
        }

        public bool ChangeScore(int index, EScore score)
        {
            if (index < 0 || index >= scores.Count) return false;

            scores[index] = score;
            SortList();
            return true;
        }
        
        private void SortList()
        {
            scores.Sort((v1, v2) => v1.CompareTo(v2) * -1);
        }

        public bool EndComplete()
            => scores.Count() == ArrowsPerEnd;
        
        public int Hits()
            => scores.Count(s => s != EScore.M);

        public int CountScore(EScore score)
            => scores.Count(s => s == score);

        public int Golds()
            => CountScore(EScore.X) + CountScore(EScore.TEN) + CountScore(EScore.NINE);
        
        public int Score()
            => scores.Sum(e => e.GetRealValue());

        public string EndString()
            => $"[{string.Join(",", scores)}]";
        
        public override string ToString()
            => $"max: {ArrowsPerEnd}, complete: {EndComplete()}, scores: {EndString()}, endScore: {Score()}]";

        public JObject ToJson()
        {
            var json = new JObject
            {
                {"scores", new JArray(scores)},
                {"score", Score()},
                {"hits", Hits()},
                {"golds", Golds()},
                {"x's", CountScore(EScore.X)},
                {"10's", CountScore(EScore.TEN)},
                {"9's", CountScore(EScore.NINE)},
                {"scoresPerEnd", ArrowsPerEnd},
                {"endComplete", EndComplete()}
            };

            return json;
        }
    }
}