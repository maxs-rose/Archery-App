using Microsoft.EntityFrameworkCore;

namespace TheScoreBook.DataStore.Test.infrastruct
{
    public static class AppDbContextFactory
    {
        public static AppDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new AppDbContext(options);
        }
    }
}