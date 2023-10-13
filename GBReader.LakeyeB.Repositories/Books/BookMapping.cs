using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Repositories.Books
{
    public class BookMapping
    {
        public static List<Book> ToBook(List<DtoBook> dtoBooks)
        {
            List<Book> books = new();
            foreach (DtoBook dtoBook in dtoBooks)
            {
                books.Add(new Book(dtoBook.Author, dtoBook.Title, dtoBook.Isbn, dtoBook.Summary));
            }
            return books;
        }
    }
}
