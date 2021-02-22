using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TheScoreBook.models;
using TheScoreBook.models.round;
using static TheScoreBook.serialisation.SaveUserData;

namespace TheScoreBook.acessors
{
    // using lazy initialised singelton model since we only ever need one of these at a time and we might not need it for some pathways
    
    public sealed class UserData
    {
        // Object getter
        private static Lazy<UserData> instance = new(() => new UserData());
        public static UserData Instance => instance.Value;

        private readonly JObject userData;

        // Sight mark getter
        private static Lazy<List<SightMark>> sightMarks = new (() => Instance.GetSightMarks());
        public static ReadOnlyCollection<SightMark> SightMarks => sightMarks.Value.AsReadOnly();

        // Past round getter
        private static Lazy<List<Round>> rounds = new(() => Instance.GetRounds());
        public static ReadOnlyCollection<Round> Rounds => rounds.Value.AsReadOnly();

        private UserData()
        {
            // this needs to be done synchronously unlike saving as we need to the data to continue
            // another good reason for this to be a singelton
            var dataFile = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "user.json");

            if (!File.Exists(dataFile))
            {
                var assembly = typeof(UserData).GetTypeInfo().Assembly;
                using (var fStream =
                    new StreamReader(assembly.GetManifestResourceStream($"{assembly.GetName().Name}.data.user.user.json")))
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
            => userData["sightMarks"]!.Value<JArray>()!.Select(sm => new SightMark(sm.Value<JObject>())).ToList();

        private List<Round> GetRounds()
            => userData["pastRounds"]!.Value<JArray>()!.Select(r => new Round(r.Value<JObject>())).ToList();

        public void AddSightMark(SightMark mark)
        {
            if (sightMarks.Value.Contains(mark))
                return; // dont duplicate
            
            sightMarks.Value.Add(mark);
            userData["sightMarks"]!.Value<JArray>()!.Add(mark.ToJson());
            SaveData(userData);
        }

        public void SaveRound(Round round)
        {
            userData["pastRounds"]!.Value<JArray>()!.Add(round.ToJson());
            
            // we dont want to await this since we want the app to continue whilst this is saving
            SaveData(userData);
        }
    }
}