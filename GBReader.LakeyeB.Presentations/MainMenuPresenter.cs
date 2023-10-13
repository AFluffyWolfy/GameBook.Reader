using GBReader.LakeyeB.Presentations.Views;
using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.Events;
using GBReader.LakeyeB.Presentations.ViewModel;
using GBReader.LakeyeB.Repositories;
using GBReader.LakeyeB.Repositories.Books;
using GBReader.LakeyeB.Repositories.Session;

namespace GBReader.LakeyeB.Presentations
{
    public class MainMenuPresenter
    {
        private readonly IMainMenu _view;
        private readonly IBookReader _domain;
        private readonly IBookRepository _bookRepo;
        private readonly ISessionRepository _sessionRepo;

        public MainMenuPresenter(IMainMenu view, IBookReader domain, IBookRepository bookRepo, ISessionRepository sessionRepo)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            Subscribing();

            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
            _sessionRepo = sessionRepo ?? throw new ArgumentNullException(nameof(sessionRepo));
        }

        private void Subscribing()
        {
            _view.SearchBookRequested += this.OnSearchRequested;
            _view.DisplayBooksRequested += this.OnBooksDisplayRequested;
            _view.ReadBookRequested += this.OnReadBookRequested;
            _view.GoToStatsRequested += this.OnGoToStatsRequested;
        }

        private void OnBooksDisplayRequested(object? sender, EventArgs args) => 
            _view.DisplayBooks(GetBooksRepo());

        private void OnSearchRequested(object? sender, BookSearchEventArgs args)
        {
            List<Book> books = GetBooksDomain();
            bool isIsbnEmpty = string.IsNullOrWhiteSpace(args.Isbn);
            bool isTitleEmpty = string.IsNullOrWhiteSpace(args.Title);

            for (int i = books.Count - 1; i >= 0; i--)
            {
                ActualBookSearchSorting(args, books, i, isTitleEmpty, isIsbnEmpty, args.Isbn, args.Title);
            }

            _view.DisplaySearchedBooks(ViewModelMapping.ToBookViewModel(books));
        }

        private static void ActualBookSearchSorting(BookSearchEventArgs args, List<Book> books, int i, bool isTitleEmpty,
            bool isIsbnEmpty, string argsIsbn, string argsTitle)
        {
            Book currentBook = books.ElementAt(i);
            string currentBookTitle = currentBook.Title.ToUpper();
            string currentBookIsbn = currentBook.Isbn.ToUpper();
            
            switch (isTitleEmpty)
            {
                // Cas où on a le titre mais pas l'ISBN
                case false when isIsbnEmpty && !currentBookTitle.Contains(argsTitle):
                    books.Remove(currentBook);
                    return;
                // Cas où on a le titre ET l'ISBN
                case false when !isIsbnEmpty && (!currentBookTitle.Contains(argsTitle) ||
                                               currentBookIsbn != argsIsbn):
                    books.Remove(currentBook);
                    return;
                // Cas où on a l'ISBN mais pas le titre
                case true when !isIsbnEmpty && currentBookIsbn != argsIsbn:
                    books.Remove(currentBook);
                    return;
            }
        }

        private void OnReadBookRequested(object? sender, BookReadEventArgs args)
        {
            bool errorHappened = SetBookUserSession(args);

            if (errorHappened == false)
            {
                errorHappened = SetBookPages();

                errorHappened = SetBookChoices();
            }

            if (errorHappened == false)
            {
                _view.GoTo("ReadingBook");
            }
        }

        private bool SetBookChoices()
        {
            try
            {
                List<Choice> choices = _bookRepo.GetChoices(_domain.GetCurrentBook());
                _domain.SetChoices(choices);
            }
            catch (RepoException e)
            {
                _view.Push(NotificationSeverity.Error, e.Message,
                    "Réessayez à nouveau ou redémarrer l'application si le problème persiste.");
                return true;
            }

            return false;
        }

        private bool SetBookPages()
        {
            try
            {
                Dictionary<int, string> pages = _bookRepo.GetPages(_domain.GetCurrentBook());
                _domain.SetPages(pages);
            }
            catch (RepoException e)
            {
                _view.Push(NotificationSeverity.Error, e.Message,
                    "Réessayez à nouveau ou redémarrer l'application si le problème persiste.");
                return true;
            }

            return false;
        }

        private bool SetBookUserSession(BookReadEventArgs args)
        {
            try
            {
                UserSession? session = _sessionRepo.CheckSessionExists(args.Isbn);

                if (session == null)
                {
                    _domain.CreateNewSession(args.Isbn);
                    _sessionRepo.SaveSession(_domain.GetCurrentSession()!);
                }
                else
                {
                    _domain.SetCurrentSession(session);
                }
            }
            catch (RepoException e)
            {
                _domain.ResetCurrentSession();
                _view.Push(NotificationSeverity.Warning, e.Message,
                    "Une erreur est survenue lors de la lecture du livre.");
                return true;
            }

            return false;
        }
        
        private void OnGoToStatsRequested(object? sender, EventArgs e) => _view.GoTo("Stats");

        private List<BookViewModel> GetBooksRepo()
        {
            List<BookViewModel> booksViewModel = new();
            try
            {
                List<Book> books = _bookRepo.GetBooks();
                booksViewModel = ViewModelMapping.ToBookViewModel(books);
                SetDomainBooks(books);
            }
            catch (RepoException e)
            {
                //La notif ne s'affiche pas lors du début de l'appli donc TextBlock fatalError dans MainMenu
                _view.Push(NotificationSeverity.Error, e.Message, "Impossible d'accéder aux livres. Veuillez redémarrer l'application.");
                _view.DisplayFatalError();
            }

            return booksViewModel;
        }

        private List<Book> GetBooksDomain() => _domain.GetBooks();

        private void SetDomainBooks(List<Book> books) => _domain.SetBooks(books);
    }
}
