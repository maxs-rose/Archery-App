using System;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;

namespace TheScoreBook
{
    public class RoundFactory : IRoundFactory
    {
        private readonly IRoundDataFactory _dataFactory;

        public RoundFactory(IRoundDataFactory dataFactory)
        {
            _dataFactory = dataFactory;
        }

        public Round Create(string name) => Create(name, Style.RECURVE);

        public Round Create(string name, Style style) => Create(name, style, DateTime.Now);

        public Round Create(string name, Style style, DateTime date)
            => new Round(_dataFactory.Create(name), style, date);

        public Round Create(JObject roundData)
            => new Round(_dataFactory.Create(roundData["rName"].Value<string>()), roundData);
    }
}