using System.Linq;
using TheScoreBook.DataStore.Test.infrastruct;
using TheScoreBook.models.round;
using Xunit;

namespace TheScoreBook.DataStore.Test
{
    public class AppDbContextText
    {
        [Fact]
        public void SavesRoundToDatabase()
        {
            var round = new Round(RoundDataFactory.Create("test"));

            using (var ctx = AppDbContextFactory.Create(nameof(SavesRoundToDatabase)))
            {
                ctx.Rounds.Add(round);
                ctx.SaveChanges();
            }
            
            using (var ctx = AppDbContextFactory.Create(nameof(SavesRoundToDatabase)))
            {
                var savedRound = ctx.Rounds.Single();
                Assert.Equal(round.RoundName, savedRound.RoundName);
            }
        }
    }
}