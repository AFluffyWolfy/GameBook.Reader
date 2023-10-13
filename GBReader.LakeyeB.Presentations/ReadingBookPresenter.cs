using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Presentations.Events;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.ViewModel;
using GBReader.LakeyeB.Presentations.Views;
using GBReader.LakeyeB.Repositories;
using GBReader.LakeyeB.Repositories.Books;
using GBReader.LakeyeB.Repositories.Session;

namespace GBReader.LakeyeB.Presentations
{
    public class ReadingBookPresenter
    {
        private readonly IReadingBook _view;
        private readonly IBookReader _domain;
        private readonly IBookRepository _bookRepo;
        private readonly ISessionRepository _sessionRepo;
        
        public ReadingBookPresenter(IReadingBook view, IBookReader domain, IBookRepository bookRepo, ISessionRepository sessionRepo)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            Subscribing();

            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _bookRepo = bookRepo ?? throw new ArgumentNullException(nameof(bookRepo));
            _sessionRepo = sessionRepo ?? throw new ArgumentNullException(nameof(sessionRepo));
        }

        private void Subscribing()
        {
            _view.DisplayPageRequested += OnPageDisplayRequested;
            _view.DisplayChoicesRequested += OnChoicesDisplayRequested;
            _view.ChoiceSelected += OnChoiceSelected;
            _view.BackToMenuRequested += OnBackToMenuRequested;
            _view.BackToBeginningRequested += OnBackToBeginningRequested;
        }

        private void OnPageDisplayRequested(object? sender, EventArgs e) => _view.DisplayPage(_domain.GetCurrentPageText());

        private void OnChoicesDisplayRequested(object? sender, EventArgs e)
        {
            List<Choice> currentPageChoices = _domain.GetCurrentPageChoices();
            _view.DisplayChoices(ViewModelMapping.ToChoicesViewModel(currentPageChoices));
        }
        
        private void OnChoiceSelected(object? sender, ChoiceSelectedEventArgs e)
        {
            _domain.UpdateSession(e.PageTo);
            
            _view.DisplayPage(_domain.GetCurrentPageText());
            
            List<Choice> currentPageChoices = _domain.GetCurrentPageChoices();
            _view.DisplayChoices(ViewModelMapping.ToChoicesViewModel(currentPageChoices));
        }
        
        private void OnBackToMenuRequested(object? sender, BackToMenuEventArgs e)
        {
            if (e.IsTerminalPage)
            {
                try
                {
                    _sessionRepo.RemoveSession(_domain.GetCurrentSession()!);
                    _domain.ResetCurrentSession();
                    _view.GoTo("MainMenu");
                }
                catch (RepoException ex)
                {
                    _view.Push(NotificationSeverity.Error, ex.Message, "Réessayez à nouveau.");
                }
            }
            else
            {
                try
                {
                    _sessionRepo.SaveSession(_domain.GetCurrentSession()!);
                    _domain.ResetCurrentSession();
                    _view.GoTo("MainMenu");
                }
                catch (RepoException ex)
                {
                    _view.Push(NotificationSeverity.Error, ex.Message, "Réessayez à nouveau.");
                }
            }
            
        }
        
        private void OnBackToBeginningRequested(object? sender, EventArgs e)
        {
            _domain.UpdateSession(1);
            
            _view.DisplayPage(_domain.GetCurrentPageText());
            
            List<Choice> currentPageChoices = _domain.GetCurrentPageChoices();
            _view.DisplayChoices(ViewModelMapping.ToChoicesViewModel(currentPageChoices));
        }
        
        private void OnUpdateSessionBeforeClosingRequested(object? sender, EventArgs e)
        {
            try
            {
                _sessionRepo.SaveSession(_domain.GetCurrentSession()!);
            }
            catch
            {
                _view.Push(NotificationSeverity.Error, "Impossible de sauvegarder la session", "Une erreur est survenue lors de la sauvegarde de votre session !");
                // Si la sauvegarde en repo échoue, je n'empêche pas la fermeture de l'app
                // Je n'ai pas réussi à gérer ce cas, finissant dans une boucle infinie avec l'arg
                // Cancel = true de l'event Closing
            }
        }
    }
}