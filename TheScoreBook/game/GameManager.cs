using System;
using System.Linq;
using Newtonsoft.Json.Linq;
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
        private static Round PreviousGame { set; get; }
        public static bool PreviousRoundNotFinished { get; private set; } = false;

        public static bool AllDistancesComplete => GameInProgress ? CurrentGame.AllDistancesComplete() : false;

        public static void StartRound(string roundName, EStyle style, DateTime date)
        {
            PreviousRoundNotFinished = false;
            PreviousGame = null;
            GameInProgress = true;
            CurrentGame = new Round(roundName, style, date);
        }

        public static void ContinuePreviousGame()
        {
            CurrentGame = PreviousGame;
            GameInProgress = true;
            PreviousRoundNotFinished = false;
        }

        public static string RoundName()
            => GameInProgress ? LocalisationManager.ToTitleCase(CurrentGame.RoundName) : default;

        public static void FinishRound(bool saveResult = true)
        {
            ClearPartialRound();
            PreviousRoundNotFinished = false;
            GameInProgress = false;
            if (CurrentGame != null)
            {
                CurrentGame.Finish();
                UserData.LatestRound = CurrentGame;
            }

            if (saveResult)
                UserData.Instance.SaveRound(CurrentGame);
            CurrentGame = null;
        }

        public static bool AddScore(int distanceIndex, int endIndex, Score score)
            => GameInProgress && CurrentGame.AddScore(distanceIndex, endIndex, score);

        public static Score GetScore(int distanceIndex, int endIndex, int scoreIndex)
            => GameInProgress ? CurrentGame.Distances[distanceIndex].Ends[endIndex].GetScore(scoreIndex) : null;

        public static void ClearEnd(int distanceIndex, int endIndex)
        {
            if (GameInProgress)
                CurrentGame.ClearEnd(distanceIndex, endIndex);
        }

        public static Distance GetDistance(int distanceIndex)
            => GameInProgress ? CurrentGame.Distances[distanceIndex] : null;


        public static bool IsFiveZone() => GameInProgress && CurrentGame.ScoringType == ScoringType.FiveZone;

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
            if (GameInProgress)
                CurrentGame.Finish(distance, end);
        }

        public static void SavePartlyFinishedRound()
        {
            if (!GameInProgress && !CurrentGame.AllDistancesComplete())
                return;

            var p = CurrentGame.ToJson();
            UserData.Instance.SavePartlyFinishedRound(p);
        }

        public static void LoadPartlyFinishedRound()
        {
            if (PreviousRoundNotFinished)
                return;

            var p = (JObject) UserData.Instance.GetPartlyFinishedRound();

            if (p == null || !p.Properties().Any())
                return;

            PreviousRoundNotFinished = true;
            PreviousGame = new Round(p);
        }

        public static void ClearPartialRound()
        {
            GameInProgress = false;
            PreviousGame = null;
        }
    }
}