using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Presentations.ViewModel
{
    public record BookViewModel
    {
        public string Author { get; }
        public string Title { get; }
        public string Isbn { get; }
        public string Summary { get; }
        
        public BookViewModel(Book book)
        {
            Author = book.Author;
            Title = book.Title;
            Isbn = book.Isbn;
            Summary = book.Summary;
        }
    }
}