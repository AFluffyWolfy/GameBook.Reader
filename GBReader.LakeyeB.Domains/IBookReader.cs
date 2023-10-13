namespace GBReader.LakeyeB.Domains
{
    public interface IBookReader
    {
        List<Book> GetBooks();
        Dictionary<int, string> GetPages();
        string GetCurrentBook();
        UserSession? GetCurrentSession();
        string GetCurrentPageText();
        List<Choice> GetCurrentPageChoices();
        public void UpdateSession(int page);
        void SetBooks(List<Book> books);
        void SetPages(Dictionary<int, string> pages);
        void SetChoices(List<Choice> choices);
        void CreateNewSession(string isbn);
        void SetCurrentSession(UserSession session);
        void ResetCurrentSession();
    }
}
