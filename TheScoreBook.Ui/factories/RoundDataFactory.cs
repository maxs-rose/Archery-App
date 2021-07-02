using TheScoreBook.acessors;
using TheScoreBook.models.round;
using TheScoreBook.models.round.Structs;

namespace TheScoreBook.Ui.factories
{
    public class RoundDataFactory : IRoundDataFactory
    {
        public RoundData Create(string roundName) => Rounds.Instance.Create(roundName);
    }
}