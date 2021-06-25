using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.models.round
{
    public class End : IToJson, INotifyPropertyChanged
    {
        private List<Score> scores = new();
        private Style Style { get; }
        public int ArrowsPerEnd { get; }
        public int Score => scores.Sum(s => s.StyleScore(Style));

        public int Golds => CountScore(enums.Score.X) + CountScore(enums.Score.TEN) + CountScore(enums.Score.NINE);
        public int Hits => scores.Count(s => s != enums.Score.MISS);
        public int RunningTotal { get; set; }

        public bool IsNextEnd { get; set; } = true;
        public bool EndHasScores => scores.Count > 0;
        public bool EndComplete => scores.Count == ArrowsPerEnd;

        public End(int arrowsPerEnd) : this(arrowsPerEnd, Style.RECURVE) {}

        public End(int arrowsPerEnd, Style style) => (ArrowsPerEnd, Style) = (arrowsPerEnd, style);

        public End(JObject json, Style style)
        {
            scores = json["scores"].Value<JArray>()!.Select(s => (Score) s.Value<int>()).ToList();
            ArrowsPerEnd = json["scoresPerEnd"].Value<int>();
            Style = style;

            PropertyHasChanged();
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

            PropertyHasChanged();

            return true;
        }

        private void SortList()
        {
            scores.Sort((v1, v2) => v1.CompareTo(v2) * -1);
        }

        public int CountScore(Score score)
            => scores.Count(s => s == score);

        public string EndString()
            => $"[{string.Join(",", scores)}]";

        public override string ToString()
            => $"max: {ArrowsPerEnd}, complete: {EndComplete}, scores: {EndString()}, endScore: {Score}]";

        public void Finish()
        {
            while (scores.Count != ArrowsPerEnd)
                scores.Add(enums.Score.MISS);

            PropertyHasChanged();
        }

        public JObject ToJson()
        {
            var json = new JObject
            {
                {"scores", new JArray(scores.Select(s => s.Id))},
                {"score", Score},
                {"hits", Hits},
                {"golds", Golds},
                {"x's", CountScore(enums.Score.X)},
                {"10's", CountScore(enums.Score.TEN)},
                {"9's", CountScore(enums.Score.NINE)},
                {"scoresPerEnd", ArrowsPerEnd},
                {"endComplete", EndComplete}
            };

            return json;
        }

        public void ClearEnd()
        {
            scores.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PropertyHasChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RunningTotal"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Score"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Golds"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hits"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsNextEnd"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GetScore"));
        }
    }
}