using System;
using Newtonsoft.Json.Linq;

namespace TheScoreBook.acessors
{
    public sealed class UserData
    {
        // TODO: Implement
        
        private static Lazy<UserData> instance = new(() => new UserData());
        public static UserData Instance => instance.Value;

        private readonly JObject userData;
        
        private UserData()
        {
        }
    }
}