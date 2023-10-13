using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.Routes;
using GBReader.LakeyeB.Presentations.ViewModel;
using GBReader.LakeyeB.Presentations.Views;

namespace GBReader.LakeyeB.Avalonia.Pages
{
    public partial class Stats : UserControl, IStats, IBrowseToViews, IShowNotifications, IPage
    {
        private IBrowseToViews? _router;
        private IShowNotifications? _notificationChannel;

        public Stats()
        {
            InitializeComponent();
            Menu.Click += OnBackToMenuClick;
        }

        public void SetRouterAndNotifications(IBrowseToViews router, IShowNotifications notificationChannel)
        {
            SetRouter(router);
            SetNotifications(notificationChannel);
        }

        private void SetRouter(IBrowseToViews router) => _router = router ?? throw new ArgumentNullException(nameof(router));
        private void SetNotifications(IShowNotifications notificationChannel) =>
            _notificationChannel = notificationChannel ?? throw new ArgumentNullException(nameof(notificationChannel));

        public void DisplaySessionsStat(List<UserSessionViewModel> sessions)
        {
            Books.Children.Clear();
            if (sessions.Count == 0)
            {
                BookNumber.Text = "Pas de sessions en cours";
            }
            else
            {
                BookNumber.Text = $"Vous avez {sessions.Count} sessions actives :";

                foreach (var session in sessions)
                {
                    WrapPanel book = new WrapPanel();
                    book.Background = Brush.Parse("LightSkyBlue");
                    
                    TextBlock firstOpen = new TextBlock();
                    firstOpen.Text = $"La session pour le livre \"{session.Isbn}\" à été créée le {session.FirstOpen:F}";
                    
                    TextBlock lastSave = new TextBlock();
                    lastSave.Text = $"La dernière sauvegarde pour cette session à été effectué le {session.LastSave:F}\n Vous étiez à la page numéro {session.CurrentPage}";
                    
                    book.Children.Add(firstOpen);
                    book.Children.Add(lastSave);
                    
                    Books.Children.Add(book);
                }
                
            }
        }
        
        private void OnBackToMenuClick(object? sender, RoutedEventArgs e) => 
            BackToMenuRequested?.Invoke(this, EventArgs.Empty);

        public void GoTo(string view) => _router?.GoTo(view);

        public void Push(NotificationSeverity serverity, string title, string message) =>
            _notificationChannel?.Push(serverity, title, message);

        public void OnEnter() => StatsDisplayRequested?.Invoke(this, EventArgs.Empty);

        public void OnQuit()
        { 

        }
        
        public event EventHandler<EventArgs>? StatsDisplayRequested; 
        public event EventHandler<EventArgs>? BackToMenuRequested; 
    }
}
