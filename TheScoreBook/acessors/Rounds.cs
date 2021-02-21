﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace TheScoreBook.acessors
{
    public sealed class Rounds
    {
        private readonly JObject roundData;
        public readonly Dictionary<string, JArray> data;
        public string[] Keys => data.Keys.ToArray();
        public JArray[] values => data.Values.ToArray();

        private Rounds()
        {
            var assembly = typeof(Rounds).GetTypeInfo().Assembly;
            using (var fStream =
                new StreamReader(assembly.GetManifestResourceStream($"{assembly.GetName().Name}.data.static.rounds.json")))
            {
                roundData = JObject.Parse(fStream.ReadToEnd());
            }

            data = roundData.Properties().ToDictionary(x => x.Name, x => roundData[x.Name]!.Value<JArray>());
        }

        private static readonly Lazy<Rounds> instance = new(() => new Rounds());
        public static Rounds Instance => instance.Value;
    }
}