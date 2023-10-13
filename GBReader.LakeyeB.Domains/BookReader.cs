namespace GBReader.LakeyeB.Domains

{
    /// <summary>
    /// Classe permettant de synchroniser les données domains entre les presenters
    /// Pas d'objet pages car une page n'est qu'un int donnant un string, donc un dictionnaire
    /// </summary>
    public class BookReader : IBookReader
    {
        private List<Book> _books = new();
        private Dictionary<int, string> _currentPages = new();
        private List<Choice> _currentChoices = new();
        private UserSession? _currentSession;
        
        // ======================= PARTIE GET ===============================================
        public List<Book> GetBooks() => new(_books);
        public Dictionary<int, string> GetPages() => new(_currentPages);
        public UserSession? GetCurrentSession() => _currentSession;
        public string GetCurrentBook() => _currentSession!.Isbn;
        public string GetCurrentPageText() => _currentPages[_currentSession!.CurrentPage];

        public List<Choice> GetCurrentPageChoices()
        {
            List<Choice> currentPageChoices = new();
            foreach (Choice choice in _currentChoices)
            {
                if (choice.PageFrom == _currentSession!.CurrentPage)
                {
                    currentPageChoices.Add(choice);
                }
            }

            return currentPageChoices;
        }
        
        // ======================= PARTIE "LOGIQUE" ===============================================
        public void CreateNewSession(string isbn) => _currentSession = UserSession.CreateNewSession(isbn);
        public void UpdateSession(int page) => _currentSession!.UpdateSession(page);
        
        
        // ======================= PARTIE SET ===============================================
        public void SetBooks(List<Book> books) => _books = new List<Book>(books);
        public void SetPages(Dictionary<int, string> pages) => _currentPages = new Dictionary<int, string>(pages);
        public void SetChoices(List<Choice> choices) => _currentChoices = new List<Choice>(choices);
        public void SetCurrentSession(UserSession session) => _currentSession = session;
        

        // ======================= PARTIE RESET ===============================================
        public void ResetCurrentSession()
        {
            _currentSession = null;
            ResetChoices();
            ResetPages();
        } 
        private void ResetPages() => _currentPages = new Dictionary<int, string>();
        private void ResetChoices() => _currentChoices = new List<Choice>();
    }
}