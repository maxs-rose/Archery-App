using TheScoreBook.models.round.Structs;

namespace TheScoreBook
{
    public interface IRoundDataFactory
    {
        public RoundData Create(string roundName);
    }
}