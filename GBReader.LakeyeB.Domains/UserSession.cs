namespace GBReader.LakeyeB.Domains
{
    /// <summary>
    /// Une session est un DateTime de première ouverture, un DateTime définissant la dernière sauvegarde,
    /// le numéro de la page actuel, le tout relié à un livre (un isbn)
    /// </summary>
    public class UserSession
    {
        public DateTime FirstOpen { get; }
        public DateTime LastSave { get; private set; }
        public int CurrentPage { get; private set; }
        public string Isbn { get; }

        public UserSession(DateTime firstOpen, DateTime lastSave, int currentPage, string isbn)
        {
            FirstOpen = firstOpen;
            LastSave = lastSave;
            CurrentPage = currentPage;
            Isbn = isbn;
        }
        
        private UserSession(int currentPage, string isbn)
        {
            FirstOpen = DateTime.Now;
            LastSave = DateTime.Now;
            CurrentPage = currentPage;
            Isbn = isbn;
        }

        public static UserSession CreateNewSession(string isbn) => new UserSession(1, isbn);

        public void UpdateSession(int currentPage)
        {
            LastSave = DateTime.Now;
            CurrentPage = currentPage;
        }
    }
}