using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheScoreBook.models;
using TheScoreBook.models.round;

namespace TheScoreBook.DataStore
{
    public interface IAppStoreContext
    {
        IEnumerable<SightMark> SightMarks { get; }
        IEnumerable<Round> Rounds { get; }

        Task<bool> LoadData(string dataPath, Stream? resourceStream);
        Task<bool> Remove<T>(T objectToRemove);
        
        bool Exists<T>(T objectToFind);
    }
}