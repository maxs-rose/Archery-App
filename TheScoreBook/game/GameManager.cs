using System;
using TheScoreBook.acessors;
using TheScoreBook.localisation;
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

        public static string RoundName()
            => GameInProgress ? LocalisationManager.ToTitleCase(CurrentGame.RoundName) : default;

        public static void FinishRound(bool saveResult = true)
        {
            GameInProgress = false;
            if(CurrentGame != null)
            {
                CurrentGame.Finish();
                UserData.LatestRound = CurrentGame;
            }
            
            if(saveResult)
                UserData.Instance.SaveRound(CurrentGame);
            CurrentGame = null;
        }

        public static bool AddScore(int distanceIndex, int endIndex, EScore score)
            => GameInProgress && CurrentGame.AddScore(distanceIndex, endIndex, score);

        public static EScore? GetScore(int distanceIndex, int endIndex, int scoreIndex)
            => GameInProgress ? CurrentGame.Distances[distanceIndex].Ends[endIndex].GetScore(scoreIndex) : null;

        public static void ClearEnd(int distanceIndex, int endIndex)
        {
            if (GameInProgress)
                CurrentGame.ClearEnd(distanceIndex, endIndex);
        }

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