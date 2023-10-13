using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Repositories.Books
{
    public interface IBookRepository
    {
        List<Book> GetBooks();
        Dictionary<int, string> GetPages(string isbn);
        List<Choice> GetChoices(string isbn);
    }
}
