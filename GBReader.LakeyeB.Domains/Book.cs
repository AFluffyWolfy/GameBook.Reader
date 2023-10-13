namespace GBReader.LakeyeB.Domains
{
    public class Book
    {
        public string Author { get; }
        public string Title { get; }
        public string Isbn { get; }
        public string Summary { get; }

        public Book(string author, string title, string isbn, string summary)
        {
            Author = author;
            Title = title;
            Isbn = isbn;
            Summary = summary;
        }
    }
}
