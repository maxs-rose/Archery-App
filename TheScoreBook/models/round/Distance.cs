﻿using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;
using TheScoreBook.models.enums.enumclass;

namespace TheScoreBook.models.round
{
    public class Distance : IToJson, INotifyPropertyChanged
    {
        public int DistanceLength { get; }
        public EDistanceUnit DistanceUnit { get; }
        public End[] Ends { get; }
        public int MaxEnds { get; }
        public int MaxScore { get; }
        public int MaxShots { get; }

        public string TargetSize { get; }

        public ScoringType ScoringType { get; }

        public int Hits => Ends.Sum(e => e.Hits);
        public int Golds => CountScore(enums.enumclass.Score.X) + CountScore(enums.enumclass.Score.TEN) + CountScore(enums.enumclass.Score.NINE);
        public int Score => Ends.Sum(e => e.Score);

        public Distance(int distanceLength, EDistanceUnit distanceUnit, int ends, int arrowsPerEnd, int targetSize,
            EDistanceUnit targetUnit, ScoringType scoringType)
        {
            DistanceLength = distanceLength;
            DistanceUnit = distanceUnit;
            Ends = new End[ends];
            MaxEnds = ends;
            ScoringType = scoringType;

            MaxShots = MaxEnds * arrowsPerEnd;
            MaxScore = MaxShots * scoringType.MaxScore().Value;

            TargetSize = $"{targetSize}{targetUnit}";

            for (var i = 0; i < ends; i++)
                Ends[i] = new End(arrowsPerEnd);
        }

        public Distance(JObject json)
        {
            DistanceLength = json["distance"].Value<int>();
            DistanceUnit = (EDistanceUnit) json["unit"].Value<int>();
            MaxEnds = json["maxEnds"].Value<int>();
            Ends = new End[MaxEnds];

            var ends = json["scores"].AsJEnumerable();
            for (var i = 0; i < MaxEnds; i++)
                Ends[i] = new End(ends[i].Value<JObject>());

            RunningTotal();
            PropertyHasChanged();
        }

        public bool AddScore(int endIndex, Score score)
        {
            if (endIndex < 0 || endIndex >= MaxEnds) return false;
            if (endIndex > 0 && !Ends[endIndex - 1].EndComplete()) return false;

            var res = Ends[endIndex].AddScore(score);
            RunningTotal();
            PropertyHasChanged();
            return res;
        }

        public int NextEndIndex()
        {
            for (var i = 0; i < MaxEnds; i++)
                if (!EndComplete(i))
                    return i;

            return -1;
        }

        public bool AllEndsComplete()
            => Ends.All(e => e.EndComplete());

        public bool EndComplete(int endIndex)
            => endIndex >= 0 && endIndex < MaxEnds && Ends[endIndex].EndComplete();

        public int CountScore(Score score)
            => Ends.Sum(e => e.CountScore(score));

        public void RunningTotal()
        {
            var sum = 0;

            for (var i = 0; i < Ends.Length && Ends[i].EndHasScores(); i++)
            {
                sum += Ends[i].Score;
                Ends[i].RunningTotal = sum;
                Ends[i].IsNextEnd = false;
                Ends[i].PropertyHasChanged();
            }

            PropertyHasChanged();
        }

        public string DistanceEndString()
            => $"[{string.Join(",", Ends.Select(e => e.EndString()))}]";

        public override string ToString()
            => $"ends: {MaxEnds}, scores: {DistanceEndString()}, endScore: {Score}]";

        public void Finish()
        {
            foreach (var end in Ends)
                end.Finish();

            PropertyHasChanged();
        }

        public void Finish(int endIndex)
        {
            Ends[endIndex].Finish();
            RunningTotal();
            PropertyHasChanged();
        }

        public JObject ToJson()
        {
            var json = new JObject
            {
                {"scores", new JArray(Ends.Select(e => e.ToJson()))},
                {"score", Score},
                {"hits", Hits},
                {"golds", Golds},
                {"x's", CountScore(enums.enumclass.Score.X)},
                {"10's", CountScore(enums.enumclass.Score.TEN)},
                {"9's", CountScore(enums.enumclass.Score.NINE)},
                {"maxEnds", MaxEnds},
                {"endComplete", AllEndsComplete()},
                {"distance", DistanceLength},
                {"unit", (int) DistanceUnit}
            };

            return json;
        }

        public void ClearEnd(int endIndex)
        {
            Ends[endIndex].ClearEnd();
            PropertyHasChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PropertyHasChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Score"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Golds"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Hits"));
        }
    }
}