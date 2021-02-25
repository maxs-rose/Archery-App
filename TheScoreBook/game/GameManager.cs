using System;
using TheScoreBook.acessors;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;

namespace TheScoreBook.game
{
    public sealed class GameManager
    {
        private static Lazy<GameManager> instance = new(() => new GameManager());
        public static GameManager Instance => instance.Value;
        private GameManager() {}

        public bool GameInProgress { private set; get; } = false;
        public Round CurrentGame { private set; get; }

        public void StartRound(string roundName, EStyle style, DateTime date)
        {
            GameInProgress = true;
            CurrentGame = new Round(roundName, style, date);
        }
        
        public void FinishRound()
        {
            GameInProgress = false;
            CurrentGame.Finish();
            UserData.Instance.SaveRound(CurrentGame);
            CurrentGame = null;
        }
    }
}