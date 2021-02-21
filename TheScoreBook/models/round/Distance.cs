using System.Linq;
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

        public Distance(int distanceLength, EDistanceUnit distanceUnit, int ends, int arrowsPerEnd)
        {
            DistanceLength = distanceLength;
            DistanceUnit = distanceUnit;
            Ends = new End[ends];
            MaxEnds = ends;

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

        public bool AllEndsComplete()
            => Ends.All(e => e.EndComplete());
        
        public bool EndComplete(int endIndex)
            => Ends[endIndex].EndComplete();

        public int Hits()
            => Ends.Sum(e => e.Hits());

        public int EndHits(int endIndex)
            => Ends[endIndex].Hits();

        public int CountScore(EScore score)
            => Ends.Sum(e => e.CountScore(score));

        public int CountEndScore(int index, EScore score)
            => Ends[index].CountScore(score);

        public int Golds()
            => CountScore(EScore.X) + CountScore(EScore.TEN) + CountScore(EScore.NINE);
        
        public int EndGolds(int endIndex)
            => CountEndScore(endIndex, EScore.X) + CountEndScore(endIndex, EScore.TEN) + CountEndScore(endIndex, EScore.NINE);

        public int DistanceScore()
            => Ends.Sum(e => e.EndScore());
        
        public int EndScore(int endIndex)
            => Ends[endIndex].EndScore();

        public string DistanceEndString()
            => $"[{string.Join(",", Ends.Select(e => e.EndString()))}]";
        
        public override string ToString()
            => $"ends: {MaxEnds}, scores: {DistanceEndString()}, endScore: {DistanceScore()}]";

        public JObject ToJson()
        {
            var json = new JObject
            {
                {"scores", new JArray(Ends.Select(e => e.ToJson()))},
                {"score", DistanceScore()},
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
    }
}