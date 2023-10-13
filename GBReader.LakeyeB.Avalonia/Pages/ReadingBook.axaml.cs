using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using GBReader.LakeyeB.Avalonia.Controls;
using GBReader.LakeyeB.Presentations.Events;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.Routes;
using GBReader.LakeyeB.Presentations.ViewModel;
using GBReader.LakeyeB.Presentations.Views;

namespace GBReader.LakeyeB.Avalonia.Pages
{
    public partial class ReadingBook : UserControl, IReadingBook, IBrowseToViews, IShowNotifications, IPage
    {
        private IBrowseToViews? _router;
        private IShowNotifications? _notificationChannel;
        private List<ChoiceViewModel> _choices = new List<ChoiceViewModel>();
        
        public ReadingBook()
        {
            InitializeComponent();
            Menu.Click += OnGoBackToMenu;
        }

        public void SetRouterAndNotifications(IBrowseToViews router, IShowNotifications notificationChannel)
        {
            SetRouter(router);
            SetNotifications(notificationChannel);
        }

        private void SetRouter(IBrowseToViews router) => _router = router ?? throw new ArgumentNullException(nameof(router));
        private void SetNotifications(IShowNotifications notificationChannel) =>
            _notificationChannel = notificationChannel ?? throw new ArgumentNullException(nameof(notificationChannel));

        public void DisplayPage(string pageText) => Page.Text = pageText;

        public void DisplayChoices(List<ChoiceViewModel> choices)
        {
            _choices = choices;
            Choices.Children.Clear();
            
            if (choices.Count == 0)
            {
                Menu.IsVisible = false;
                DisplayNoChoices();
            }
            else
            {
                Menu.IsVisible = true;
                foreach (var choice in _choices)
                {
                    ChoiceButton newChoice = new ChoiceButton();
                    newChoice.InitializeChoice(choice.Text, choice.PageTo);
                    newChoice.ChoiceSelected += OnChoiceSelected;
                    Choices.Children.Add(newChoice);
                }
            }
            
        }

        private void DisplayNoChoices()
        {
            Button buttonMenu = new();
            buttonMenu.Click += OnGoBackToMenuFromTerminalPage;
            buttonMenu.Content = "Retour à l'accueil";
            buttonMenu.FontFamily = "Arial Rounded MT";

            Button buttonBeginning = new();
            buttonBeginning.Click += OnGoBackToBeginning;
            buttonBeginning.Content = "Relire du début";
            buttonBeginning.FontFamily = "Arial Rounded MT";

            Choices.Children.Add(buttonBeginning);
            Choices.Children.Add(buttonMenu);
        }

        private void OnChoiceSelected(object? sender, ChoiceSelectedEventArgs e) => 
            ChoiceSelected?.Invoke(this, e);

        private void OnGoBackToBeginning(object? sender, RoutedEventArgs e) => 
            BackToBeginningRequested?.Invoke(this, EventArgs.Empty);

        private void OnGoBackToMenu(object? sender, RoutedEventArgs e) => 
            BackToMenuRequested?.Invoke(this, new BackToMenuEventArgs(false));

        private void OnGoBackToMenuFromTerminalPage(object? sender, RoutedEventArgs e) => 
            BackToMenuRequested?.Invoke(this, new BackToMenuEventArgs(true));

        public void GoTo(string view) => _router?.GoTo(view);

        public void Push(NotificationSeverity serverity, string title, string message) =>
            _notificationChannel?.Push(serverity, title, message);

        public void OnEnter()
        {
            DisplayPageRequested?.Invoke(this, EventArgs.Empty);
            DisplayChoicesRequested?.Invoke(this, EventArgs.Empty);
        }

        // Le nom d'event "BackToMenu" n'est pas très parlant dans ce contexte,
        // mais c'est pour éviter d'en créer un avec les mêmes propriétés alors que j'ai besoin de la même exécution
        public void OnQuit() =>
            BackToMenuRequested?.Invoke(this,
                _choices.Count == 0 ? new BackToMenuEventArgs(true) : new BackToMenuEventArgs(false));
        
        public event EventHandler<EventArgs>? DisplayPageRequested;
        public event EventHandler<EventArgs>? DisplayChoicesRequested;
        public event EventHandler<ChoiceSelectedEventArgs>? ChoiceSelected;
        public event EventHandler<EventArgs>? BackToBeginningRequested;
        public event EventHandler<BackToMenuEventArgs>? BackToMenuRequested;
    }
}
