﻿using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;
using TheScoreBook.models.round.structs;

namespace TheScoreBook.models.round
{
    public class Distance : IToJson, INotifyPropertyChanged
    {
        private DistanceData DistanceData { get; }
        public int DistanceLength => DistanceData.DistanceLength;
        public MeasurementUnit DistanceUnit => DistanceData.DistanceUnit;
        public End[] Ends { get; }
        public int MaxEnds => DistanceData.MaxEnds;
        public int MaxScore => DistanceData.MaxScore;
        public int MaxShots => DistanceData.MaxShots;

        public string TargetSize { get; }

        public ScoringType ScoringType { get; }

        public int Hits => Ends.Sum(e => e.Hits);
        public int Golds => CountScore(enums.Score.X) + CountScore(enums.Score.TEN) + CountScore(enums.Score.NINE);
        public int Score => Ends.Sum(e => e.Score);

        public Distance(DistanceData distanceData)
        {
            DistanceData = distanceData;

            TargetSize = $"{distanceData.TargetSize}{distanceData.TargetUnit}";

            Ends = new End[distanceData.MaxEnds];
            for (var i = 0; i < distanceData.MaxEnds; i++)
                Ends[i] = new End(distanceData.ArrowsPerEnd);
        }

        public Distance(DistanceData distanceData, JObject json) : this(distanceData)
        {
            Ends = json["scores"].AsJEnumerable().Select(e => new End(e.Value<JObject>())).ToArray();

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
                {"x's", CountScore(enums.Score.X)},
                {"10's", CountScore(enums.Score.TEN)},
                {"9's", CountScore(enums.Score.NINE)},
                {"maxEnds", MaxEnds},
                {"endComplete", AllEndsComplete()},
                {"distance", DistanceLength},
                {"unit", DistanceUnit.Id}
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