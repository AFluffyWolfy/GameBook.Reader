using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GBReader.LakeyeB.Avalonia.Pages;
using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Infrastructures.Books;
using GBReader.LakeyeB.Infrastructures.Session;
using GBReader.LakeyeB.Presentations;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Repositories;
using GBReader.LakeyeB.Repositories.Books;
using GBReader.LakeyeB.Repositories.Session;

namespace GBReader.LakeyeB.Avalonia
{
    public partial class App : Application
    {
        private MainWindow? _mainWindow;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _mainWindow = new MainWindow();
                desktop.MainWindow = _mainWindow;

                CreateStuff();
            }

            base.OnFrameworkInitializationCompleted();
        }
        
        private void CreateStuff()
        {
            IBookRepository? bookRepoDb = null;
            ISessionRepository? sessionRepoJson = null;
            try
            {
                bookRepoDb = BookRepositoryFactory.CreateRepository("sql");
                sessionRepoJson = SessionRepositoryFactory.CreateRepository("json");
            }
            catch (RepoException e)
            {
                _mainWindow!.Push(NotificationSeverity.Error, e.Message, "Redémarrer l'application");
            }

            IBookReader domain = new BookReader();

            var mainMenuView = new MainMenu();
            mainMenuView.SetRouterAndNotifications(_mainWindow!, _mainWindow!);
            var mainMenuPresenter = new MainMenuPresenter(mainMenuView, domain, bookRepoDb!, sessionRepoJson!);

            var readingBookView = new ReadingBook();
            readingBookView.SetRouterAndNotifications(_mainWindow!, _mainWindow!);
            var readingBookPresenter = new ReadingBookPresenter(readingBookView, domain, bookRepoDb!, sessionRepoJson!);

            var statsView = new Stats();
            statsView.SetRouterAndNotifications(_mainWindow!, _mainWindow!);
            var statsPresenter = new StatsPresenter(statsView, domain, sessionRepoJson!);

            _mainWindow!.RegisterPage("MainMenu", mainMenuView);
            _mainWindow!.RegisterPage("ReadingBook", readingBookView);
            _mainWindow!.RegisterPage("Stats", statsView);
            
            // Instruction pour ne pas Get les livres dans le presenter MainMenu à la création de l'app
            _mainWindow.GoTo("MainMenu");
        }
    }
}
