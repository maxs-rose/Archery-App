using System.Collections.Generic;
using System.Linq;
using TheScoreBook.models;
using TheScoreBook.models.round;

namespace TheScoreBook.DataStore
{
    public class PersistenceIteractor
    {
        private readonly IAppStoreContext _ctx;

        public PersistenceIteractor(IAppStoreContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<SightMark> SightMarks => _ctx.SightMarks;
        
        public SightMark GetSightMark(Distance distance) 
            => _ctx.SightMarks.FirstOrDefault(mark => mark.Distance == distance.DistanceLength && mark.DistanceUnit == distance.DistanceUnit);
        public bool RemoveSightMark(SightMark sightMark) 
            => _ctx.Exists(sightMark) && _ctx.Remove(sightMark).Result;

        public IEnumerable<Round> Rounds => _ctx.Rounds;
        public bool RemoveRound(Round round) 
            => _ctx.Exists(round) && _ctx.Remove(round).Result;
    }
}