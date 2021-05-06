using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.models.round
{
    public class Distance : IToJson
    {
        public int DistanceLength { get; }
        public EDistanceUnit DistanceUnit { get; }
        public End[] Ends { get; }
        public int MaxEnds { get; }
        public int MaxScore { get; }
        public int MaxShots { get; }

        public string TargetSize { get; }
        
        public Distance(int distanceLength, EDistanceUnit distanceUnit, int ends, int arrowsPerEnd, int targetSize, EDistanceUnit targetUnit)
        {
            DistanceLength = distanceLength;
            DistanceUnit = distanceUnit;
            Ends = new End[ends];
            MaxEnds = ends;

            MaxShots = MaxEnds * arrowsPerEnd;
            MaxScore = MaxShots * 10;

            TargetSize = $"{targetSize}{targetUnit}";

            for (var i = 0; i < ends; i++)
                Ends[i] = new End(arrowsPerEnd);
        }

        public Distance(JObject json)
        {
            DistanceLength = json["distance"].Value<int>();
            DistanceUnit = (EDistanceUnit)json["unit"].Value<int>();
            MaxEnds = json["maxEnds"].Value<int>();
            Ends = new End[MaxEnds];

            var ends = json["scores"].AsJEnumerable();
            for (var i = 0; i < MaxEnds; i++)
                Ends[i] = new End(ends[i].Value<JObject>());
        }
    
        public bool AddScore(int endIndex, EScore score)
        {
            if (endIndex < 0 || endIndex >= MaxEnds) return false;
            if (endIndex > 0 && !Ends[endIndex - 1].EndComplete()) return false;
            
            return Ends[endIndex].AddScore(score);
        }
        
        public bool RemoveScore(int endIndex, int scoreIndex)
        {
            if (endIndex < 0 || endIndex >= MaxEnds) return false;
            return Ends[endIndex].RemoveScore(scoreIndex);
        }

        public bool ChangeScore(int endIndex, int scoreIndex, EScore score)
        {
            if (endIndex < 0 || endIndex >= MaxEnds) return false;
            return Ends[endIndex].ChangeScore(scoreIndex, score);
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

        public int Hits()
            => Ends.Sum(e => e.Hits());

        public int Hits(int endIndex)
            => Ends[endIndex].Hits();

        public int CountScore(EScore score)
            => Ends.Sum(e => e.CountScore(score));

        public int CountScore(int index, EScore score)
            => Ends[index].CountScore(score);

        public int Golds()
            => CountScore(EScore.X) + CountScore(EScore.TEN) + CountScore(EScore.NINE);
        
        public int Golds(int endIndex)
            => CountScore(endIndex, EScore.X) + CountScore(endIndex, EScore.TEN) + CountScore(endIndex, EScore.NINE);

        public int Score()
            => Ends.Sum(e => e.Score());

        public int Score(int index)
            => Ends[index].Score();

        public int RunningTotal(int toIndex)
        {
            var sum = 0;

            for (var i = 0; i <= toIndex && i < MaxEnds; i++)
                sum += Ends[i].Score();
            
            return sum;
        }

        public string DistanceEndString()
            => $"[{string.Join(",", Ends.Select(e => e.EndString()))}]";
        
        public override string ToString()
            => $"ends: {MaxEnds}, scores: {DistanceEndString()}, endScore: {Score()}]";

        public void Finish()
        {
            foreach (var end in Ends)
                end.Finish();
        }

        public void Finish(int endIndex)
        {
            Ends[endIndex].Finish();
        }
        
        public JObject ToJson()
        {
            var json = new JObject
            {
                {"scores", new JArray(Ends.Select(e => e.ToJson()))},
                {"score", Score()},
                {"hits", Hits()},
                {"golds", Golds()},
                {"x's", CountScore(EScore.X)},
                {"10's", CountScore(EScore.TEN)},
                {"9's", CountScore(EScore.NINE)},
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
        }
    }
}