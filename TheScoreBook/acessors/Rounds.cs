using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TheScoreBook.models.enums;

namespace TheScoreBook.acessors
{
    public sealed class Rounds
    {
        private readonly JObject roundData;
        public readonly Dictionary<string, JObject> data;
        public string[] Keys => data.Keys.ToArray();
        public JObject[] values => data.Values.ToArray();

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

            data = roundData.Properties().ToDictionary(x => x.Name, x => x.Value.Value<JObject>());
        }

        public ELocation roundLocation(string roundName)
        {
            return data[roundName]!["distances"]![0]!["location"]!.Value<string>() == "in"
                ? ELocation.INDOOR
                : ELocation.OUTDOOR;
        }

        private static readonly Lazy<Rounds> instance = new(() => new Rounds());
        public static Rounds Instance => instance.Value;
    }
}