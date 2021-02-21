namespace TheScoreBook.models
{
    public enum EScore
    { 
        X = 11, TEN = 10, NINE = 9, EIGHT = 8, SEVEN = 7, SIX = 6, FIVE = 5, FOUR = 4, THREE = 3, TWO = 2, ONE = 1, M = 0
    }

    public static class EScoreHelpers
    {
        public static string ToUserString(this EScore s)
            => s switch
            {
                EScore.M => "M",
                EScore.X => "X",
                _ => ((int)s).ToString()
            };
        
        public static int GetRealValue(this EScore s)
        {
            return s switch
            {
                EScore.X => 10,
                _ => (int)s
            };
        }
    }
}