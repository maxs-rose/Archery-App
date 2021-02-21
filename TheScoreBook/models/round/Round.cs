using System.Linq;
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
                Distances[i] = new Distance(dist["distance"]!.Value<int>(), dist["unit"]!.Value<string>().ToEDistanceUnit(), dist["ends"]!.Value<int>(), dist["arrowsPerEnd"]!.Value<int>());
            }
        }

        public Round(JObject roundData)
        {
            DistanceCount = roundData["nDistances"].Value<int>();
            Distances = new Distance[DistanceCount];

            var dist = roundData["distances"]!.Value<JArray>();

            for (var i = 0; i < DistanceCount; i++)
                Distances[i] = new Distance(dist[i].Value<JObject>());
        }

        public JObject ToJson()
        {
            var json = new JObject()
            {
                {"distances", new JArray(Distances.Select(d => d.ToJson()))},
                {"nDistances", DistanceCount}
            };           
            
            return json;
        }
    }
}