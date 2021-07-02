using System;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;

namespace TheScoreBook
{
    public interface IRoundFactory
    {
        public Round Create(string name);
        public Round Create(string name, Style style);
        public Round Create(string name, Style style, DateTime date);

        public Round Create(JObject roundData);
    }
}