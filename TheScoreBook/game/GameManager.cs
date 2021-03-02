using System;
using TheScoreBook.acessors;
using TheScoreBook.models.enums;
using TheScoreBook.models.round;

namespace TheScoreBook.game
{
    public static class GameManager
    {
        public static bool GameInProgress { private set; get; } = false;
        private static Round CurrentGame { set; get; }
        
        public static bool AllDistancesComplete => GameInProgress ? CurrentGame.AllDistancesComplete() : false;

        public static void StartRound(string roundName, EStyle style, DateTime date)
        {
            GameInProgress = true;
            CurrentGame = new Round(roundName, style, date);
        }
        
        public static void FinishRound()
        {
            GameInProgress = false;
            CurrentGame.Finish();
            UserData.Instance.SaveRound(CurrentGame);
            CurrentGame = null;
        }

        public static bool AddScore(int distanceIndex, int endIndex, EScore score)
            => GameInProgress && CurrentGame.AddScore(distanceIndex, endIndex, score);

        public static Distance GetDistance(int distanceIndex)
            => GameInProgress ? CurrentGame.Distances[distanceIndex] : null;
        

        public static int Hits()
        {
            if (GameInProgress)
                return CurrentGame.Hits();

            return -1;
        }
        
        public static int Golds()
        {
            if (GameInProgress)
                return CurrentGame.Golds();

            return -1;
        }
        
        public static int Score()
        {
            if (GameInProgress)
                return CurrentGame.Score();

            return -1;
        }

        public static int NextDistanceIndex()
        {
            if (GameInProgress)
                return CurrentGame.NextDistanceIndex();

            return -1;
        }

        public static int NextEndIndex(int distIndex)
        {
            if (GameInProgress)
                return CurrentGame.NextEndIndex(distIndex);

            return -1;   
        }

        public static bool EndComplete(int distanceIndex, int end)
            => GameInProgress && CurrentGame.Distances[distanceIndex].EndComplete(end);

        public static void Finish(int distance, int end)
        {
            if(GameInProgress)
                CurrentGame.Finish(distance, end);
        }
    }
}