using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TheScoreBook.models;
using TheScoreBook.models.round;
using Xamarin.Forms;
using static TheScoreBook.serialisation.SaveUserData;

namespace TheScoreBook.acessors
{
    // using lazy initialised singelton model since we only ever need one of these at a time and we might not need it for some pathways
    
    public sealed class UserData
    {
        private readonly JObject userData;
        private static Mutex mut;
        
        // Object getter
        private static readonly Lazy<UserData> instance = new(() => new UserData());
        public static UserData Instance => instance.Value;
        
        // Sight mark stuff
        private static readonly Lazy<List<SightMark>> sightMarks = new (() => Instance.GetSightMarks());
        public static ReadOnlyCollection<SightMark> SightMarks => sightMarks.Value.AsReadOnly();

        public delegate void SightMarksUpdated();
        public static SightMarksUpdated SightMarksUpdatedEvent;

        // Round stuff
        private static readonly Lazy<List<Round>> rounds = new(() => Instance.GetRounds());
        public static ReadOnlyCollection<Round> Rounds => rounds.Value.AsReadOnly();
        public static Round LatestRound;

        private UserData()
        {
            mut = new Mutex();
            
            // this needs to be done synchronously unlike saving as we need to the data to continue
            // another good reason for this to be a singelton
#if DEBUG
            var fileName = "user.debug.json";
#else
            var fileName = $"user.json";
#endif
            var dataFile = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

            if (!File.Exists(dataFile))
            {
                var assembly = typeof(UserData).GetTypeInfo().Assembly;
                using (var fStream = new StreamReader(assembly.GetManifestResourceStream($"{assembly.GetName().Name}.data.user.{fileName}")))
                {
                    userData = JObject.Parse(fStream.ReadToEnd());
                }
            }
            else
            {
                using (var sr = new StreamReader(dataFile, true))
                {
                    userData = JObject.Parse(sr.ReadToEnd());
                }
            }
        }

        ~UserData()
        {
            try
            {
                var saveData = SaveData(userData).Result;
                Console.WriteLine("INFO - Saved user data");
            }
            catch (Exception e)
            {
                Console.WriteLine("Serialisation Exception - Failed to save user data in deconstruction");
            }
        }

        private List<SightMark> GetSightMarks()
        {
            var result = new List<SightMark>();
            
            mut.WaitOne();
            result = userData["sightMarks"]!.Value<JArray>()!.Select(sm => new SightMark(sm.Value<JObject>())).ToList();
            mut.ReleaseMutex();

            return result;
        }

        private List<Round> GetRounds()
        {
            var result = new List<Round>();
            
            mut.WaitOne();
            result = userData["pastRounds"]!.Value<JArray>()!.Select(r => new Round(r.Value<JObject>())).ToList();
            mut.ReleaseMutex();

            return result;
        }

        public List<Round> GetPB()
        {
            mut.WaitOne();

            var result = Rounds.GroupBy(r => r.RoundName)
                .Select(g => g.OrderByDescending(g => g.Score()).First());
            
            mut.ReleaseMutex();

            return result.ToList();
        }

        public void AddSightMark(SightMark mark)
        {
            if (sightMarks.Value.Contains(mark))
                return; // dont duplicate
            
            sightMarks.Value.Add(mark);

            mut.WaitOne();
            userData["sightMarks"]!.Value<JArray>()!.Add(mark.ToJson());
            mut.ReleaseMutex();
            
            Device.BeginInvokeOnMainThread(() => SightMarksUpdatedEvent?.Invoke());
            Task.Run(() => SaveData(userData));
        }

        public void DeleteSightMark(SightMark mark)
        {
            if (!sightMarks.Value.Contains(mark))
                return; // dont try and remove things that dont exist

            sightMarks.Value.Remove(mark);
            mut.WaitOne();
            
            userData["sightMarks"]!.Value<JArray>()!.Clear();
            
            foreach(var m in sightMarks.Value)
                userData["sightMarks"]!.Value<JArray>()!.Add(m.ToJson());
            
            mut.ReleaseMutex();
            
            Device.BeginInvokeOnMainThread(() => SightMarksUpdatedEvent?.Invoke());
            Task.Run(() => SaveData(userData));
        }

        public async void SaveRound(Round round)
        {
            rounds.Value.Add(round);
            
            mut.WaitOne();
            userData["pastRounds"]!.Value<JArray>()!.Add(round.ToJson());
            mut.ReleaseMutex();
            
            // we dont want to await this since we want the app to continue whilst this is saving
            Task.Run(() => SaveData(userData));
        }
    }
}