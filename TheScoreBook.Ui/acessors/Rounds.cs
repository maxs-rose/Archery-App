using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TheScoreBook.exceptions;
using TheScoreBook.models.enums;
using TheScoreBook.models.round.Structs;

namespace TheScoreBook.acessors
{
    public sealed class Rounds
    {
        private readonly JObject roundData;
        private readonly Dictionary<string, RoundData> rounds;
        public string[] Keys { get; }

        private Rounds()
        {
            var assembly = typeof(Rounds).GetTypeInfo().Assembly;

#if DEBUG
            var filePath = $"{assembly.GetName().Name}.data.static.rounds.debug.json";
#else
            var filePath = $"{assembly.GetName().Name}.data.static.rounds.json";
#endif

            using (var fStream = new StreamReader(assembly.GetManifestResourceStream(filePath)))
            {
                roundData = JObject.Parse(fStream.ReadToEnd());
            }

            rounds = roundData.Properties().ToDictionary(x => x.Name, x => new RoundData(x));
            Keys = rounds.Keys.ToArray();
        }

        private static readonly Lazy<Rounds> instance = new(() => new Rounds());
        public static Rounds Instance => instance.Value;

        public RoundData Create(string roundName)
        {
            if (!Keys.Contains(roundName.ToLower()))
                throw new InvalidRoundException($"{roundName} is not a valid round");

            return rounds[roundName.ToLower()];
        }

        public IEnumerable<(RoundGrouping group, IEnumerable<string> roundNames)> GetGroupedRounds()
        {
            return rounds.Values
                .OrderBy(r => r.Name)
                .ThenBy(r => r.group.DisplayName)
                .GroupBy(r => r.group)
                .Select(g => (g.Key, g.Select(r => r.Name)));
        }
    }
}