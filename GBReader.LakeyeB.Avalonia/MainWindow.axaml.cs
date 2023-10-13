using System;
using Avalonia.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls.Notifications;
using GBReader.LakeyeB.Avalonia.Pages;
using GBReader.LakeyeB.Presentations.Routes;
using GBReader.LakeyeB.Presentations.Notifications;

namespace GBReader.LakeyeB.Avalonia
{
    /// <summary>
    /// Classe inspirée (comprendre fortement copiée) du code de Monsieur Hendrikx sur la branche
    /// solution pour le PigOrCow. Les changements apportés sont les suivants
    ///
    /// Ajout d'une interface IPage, le dictionnaire _pages étant dorénavant <string, IPage> et
    /// les changements que cela implique dans le reste du code
    /// (cet ajout permet d'appeller des OnEnter() et OnQuit() sur toutes les vues)
    ///
    /// Abonnement à l'event Closing pour gérer la fermeture de l'application pendant
    /// la lecture d'un livre
    ///
    /// </summary>
    public partial class MainWindow : Window, IBrowseToViews, IShowNotifications
    {
        private readonly WindowNotificationManager _notificationManager;
        private readonly IDictionary<string, IPage> _pages = new Dictionary<string, IPage>();

        public MainWindow()
        {
            InitializeComponent();
            _notificationManager = new WindowNotificationManager(this)
            {
                Position = NotificationPosition.BottomRight
            };
            this.Closing += OnCloseRequested;
        }

        private void OnCloseRequested(object? sender, CancelEventArgs e)
        {
            foreach (var view in _pages.Values)
            {
                if (Content == view)
                {
                    view.OnQuit();
                }
            }
        }

        internal void RegisterPage(string viewName, UserControl view)
        {
            _pages[viewName] = (IPage)view;
            if (Content == null)
            {
                Content = view;
            }
        }

        private void Close(object? sender, EventArgs e) => this.Close();

        public void GoTo(string viewName)
        {
            Content = (UserControl)_pages[viewName];
            _pages[viewName].OnEnter();
        }

        public void Push(NotificationSeverity severity, string title, string message)
        {
            var notification = new Notification(title, message, severity switch
            {
                NotificationSeverity.Info => NotificationType.Information,
                NotificationSeverity.Warning => NotificationType.Warning,
                NotificationSeverity.Success => NotificationType.Success,
                _ => NotificationType.Error,
            });

            if (this.IsVisible)
            {
                _notificationManager.Show(notification);
            }
        }
    }
}
