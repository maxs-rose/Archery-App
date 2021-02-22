using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Internals;

namespace TheScoreBook.serialisation
{
    public static class SaveUserData
    {
        public static async Task<bool> SaveData(JObject data)
        {
            try
            {
                var path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                using (var sw = new StreamWriter(Path.Combine(path, "user.json"), false))
                {
                    // remove the newline characters to save a small amount of disk space
                    sw.WriteLine(data.ToString().Replace("\n" , ""));
                }
            }
            catch (SerializationException e)
            {
                Log.Warning("Serialisation Exception", $"Error saving user data, ${e.Message}: {e.StackTrace}");
                return false;
            }

            return true;
        }
    }
}