using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.ViewModel;
using GBReader.LakeyeB.Presentations.Views;
using GBReader.LakeyeB.Repositories;
using GBReader.LakeyeB.Repositories.Session;

namespace GBReader.LakeyeB.Presentations
{
    public class StatsPresenter
    {
        private readonly IStats _view;
        private readonly IBookReader _domain;
        private readonly ISessionRepository _sessionRepo;
        
        public StatsPresenter(IStats view, IBookReader domain, ISessionRepository sessionRepo)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            Subscribing();

            _domain = domain ?? throw new ArgumentNullException(nameof(domain));
            _sessionRepo = sessionRepo ?? throw new ArgumentNullException(nameof(sessionRepo));
        }

        private void Subscribing()
        {
            _view.StatsDisplayRequested += OnStatsDisplayRequested;
            _view.BackToMenuRequested += OnBackToMenu;
        }

        private void OnBackToMenu(object? sender, EventArgs e) => _view.GoTo("MainMenu");

        private void OnStatsDisplayRequested(object? sender, EventArgs e)
        {
            try
            {
                List<UserSession> sessions = _sessionRepo.GetUserSessions();
                _view.DisplaySessionsStat(ViewModelMapping.ToSessionsViewModel(sessions));
            }
            catch (RepoException ex)
            {
                _view.Push(NotificationSeverity.Error, ex.Message,
                    "Veuillez recharger la page et/ou l'application si le problème persiste.");
            }
        }
    }
}