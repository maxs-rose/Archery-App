using System.Linq;
using Moq;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.round.Structs;

namespace TheScoreBook.DataStore.Test.infrastruct
{
    public static class RoundDataFactory
    {
        public static RoundData Create(string roundName)
        {
            var property = JObject.Parse("{\"" + roundName + "\": { \"scoringType\": \"TenZone\", \"distances\":[{\"location\": \"out\",\"distance\": 18,\"unit\": \"m\",\"arrowsPerEnd\": 3,\"ends\": 1,\"targetSize\": 40, \"targetUnit\": \"cm\"}]}}")
                .Properties()
                .Single();
            var roundData = new RoundData(property);
            
            return roundData;
        }
    }
}