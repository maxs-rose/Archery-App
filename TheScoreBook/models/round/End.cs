using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.models.round
{
    public class End : IToJson
    {
        private List<Score> scores = new();
        public int ArrowsPerEnd { get; }

        public End(int arrowsPerEnd)
        {
            ArrowsPerEnd = arrowsPerEnd;
        }

        public End(JObject json)
        {
            scores = json["scores"].Value<JArray>()!.Select(s => (Score)s.Value<int>()).ToList();
            ArrowsPerEnd = json["scoresPerEnd"].Value<int>();
        }

        public Score GetScore(int scoreIndex)
        {
            if (scores.Count > scoreIndex && scoreIndex >= 0)
                return scores[scoreIndex];

            return null;
        }

        public bool AddScore(Score score)
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

        public bool ChangeScore(int index, Score score)
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
            => scores.Count(s => s != enums.Score.MISS);

        public int CountScore(Score score)
            => scores.Count(s => s == score);

        public int Golds()
            => CountScore(enums.Score.X) + CountScore(enums.Score.TEN) + CountScore(enums.Score.NINE);
        
        public int Score()
            => scores.Sum(e => e.Value);

        public string EndString()
            => $"[{string.Join(",", scores)}]";
        
        public override string ToString()
            => $"max: {ArrowsPerEnd}, complete: {EndComplete()}, scores: {EndString()}, endScore: {Score()}]";

        public void Finish()
        {
            while(scores.Count != ArrowsPerEnd)
                scores.Add(enums.Score.MISS);
        }
        
        public JObject ToJson()
        {
            var json = new JObject
            {
                {"scores", new JArray(scores.Select(s => s.Id))},
                {"score", Score()},
                {"hits", Hits()},
                {"golds", Golds()},
                {"x's", CountScore(enums.Score.X)},
                {"10's", CountScore(enums.Score.TEN)},
                {"9's", CountScore(enums.Score.NINE)},
                {"scoresPerEnd", ArrowsPerEnd},
                {"endComplete", EndComplete()}
            };

            return json;
        }

        public void ClearEnd()
        {
            scores.Clear();
        }
    }
}