using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheScoreBook.models;
using TheScoreBook.models.round;

namespace TheScoreBook.DataStore.DataStores
{
    public class JSONDataStore : IAppStoreContext
    {
        private readonly IRoundFactory roundFactory;
        private string dataPath = null;
        private JObject userData = null;
        
        public IEnumerable<SightMark> SightMarks { get; private set; } = Enumerable.Empty<SightMark>();
        public IEnumerable<Round> Rounds { get; private set; } = Enumerable.Empty<Round>();
        
        public JSONDataStore(IRoundFactory roundFactory)
        {
            this.roundFactory = roundFactory;
        }

        public async Task<bool> LoadData(string dataPath, Stream? resourceStream)
        {
            StreamReader reader;
            if (File.Exists(dataPath))
                reader = new StreamReader(dataPath, true);
            else
                reader = new StreamReader(resourceStream ??
                                          throw new InvalidDataException( "Data file was not found! Please supply a stream!"));
            
            userData = JObject.Parse(await reader.ReadToEndAsync());
            reader.Dispose();

            var builders = new Func<JObject, Task<bool>>[] {BuildRounds, BuildSightMarks};
            var result = await Task.WhenAll(builders.Select(b => b(userData)));

            return result.All(r => r);
        }

        public async Task<bool> Remove<T>(T objectToRemove)
        {
            var totalLength = SightMarks.Count() + Rounds.Count();
            
            if (typeof(T) == typeof(SightMark))
                SightMarks = SightMarks.Where(sm => !JToken.DeepEquals(sm.ToJson(), (objectToRemove as SightMark)!.ToJson()));
            else if (typeof(T) == typeof(Round))
                Rounds = Rounds.Where(r => !JToken.DeepEquals(r.ToJson(), (objectToRemove as Round)!.ToJson()));

            return (totalLength > SightMarks.Count() + Rounds.Count()) && await SaveData();
        }

        public bool Exists<T>(T objectToFind)
        {
            if (typeof(T) == typeof(SightMark))
                return SightMarks.AsParallel().Any(sm => !JToken.DeepEquals(sm.ToJson(), (objectToFind as SightMark)!.ToJson()));
            else if (typeof(T) == typeof(Round))
                return Rounds.AsParallel().Any(r => !JToken.DeepEquals(r.ToJson(), (objectToFind as Round)!.ToJson()));

            return false;
        }

        private async Task<bool> BuildRounds(JObject data)
        {
            Rounds = data["pastRounds"]!.Value<JArray>()!.AsParallel().Select(r => roundFactory.Create(r.Value<JObject>()));
            return true;
        }

        private async Task<bool> BuildSightMarks(JObject data)
        {
            SightMarks = data["sightMarks"]!.Value<JArray>()!
                .AsParallel()
                .Select(r => new SightMark(r.Value<JObject>()));
            return true;
        }

        private async Task<bool> SaveData()
        {
            try
            {
                await using var sw = new StreamWriter(dataPath, false);
                await sw.WriteLineAsync(userData.ToString().Replace("\n", ""));

                return true;
            }
            catch (SerializationException e)
            {   
                return false;
            }
        }
    }
}