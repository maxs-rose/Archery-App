using Newtonsoft.Json.Linq;

namespace TheScoreBook.models
{
    public interface IToJson
    {
        public JObject ToJson();
    }
}