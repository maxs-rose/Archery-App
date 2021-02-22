namespace TheScoreBook.localisation
{
    public class Language
    {
        public string Name { get; }
        public string CI;

        public Language(string name, string ci)
        {
            Name = name;
            CI = ci;
        }
    }
}