using Avalonia.Controls;
using Avalonia.Interactivity;
using GBReader.LakeyeB.Presentations.Events;
using System;
using System.Collections.Generic;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.Routes;
using GBReader.LakeyeB.Presentations.ViewModel;
using GBReader.LakeyeB.Presentations.Views;

namespace GBReader.LakeyeB.Avalonia.Pages
{
    public partial class MainMenu : UserControl, IMainMenu, IBrowseToViews, IShowNotifications, IPage
    {
    private IBrowseToViews? _router;
    private IShowNotifications? _notificationChannel;
    private readonly Controls.BooksPanel _booksView;

    public MainMenu()
    {
        InitializeComponent();

        _booksView = new Controls.BooksPanel();
        BooksPanel.Children.Add(_booksView);
        Subscribing();
    }

    public void SetRouterAndNotifications(IBrowseToViews router, IShowNotifications notificationChannel)
    {
        SetRouter(router);
        SetNotifications(notificationChannel);
    }

    private void SetRouter(IBrowseToViews router) =>
        _router = router ?? throw new ArgumentNullException(nameof(router));

    private void SetNotifications(IShowNotifications notificationChannel) =>
        _notificationChannel = notificationChannel ?? throw new ArgumentNullException(nameof(notificationChannel));

    private void Subscribing()
    {
        _booksView.ReadBookRequested += OnReadBookRequested;
        StatsButton.Click += OnGoToStats;
    }

    private void Search_OnClick(object? sender, RoutedEventArgs args)
    {
        string title = bookTitle.Text == null ? string.Empty : bookTitle.Text.ToUpper();
        string isbn = bookIsbn.Text == null ? string.Empty : bookIsbn.Text.ToUpper();
        var bookSearchArgs = new BookSearchEventArgs(title, isbn);
        SearchBookRequested?.Invoke(this, bookSearchArgs);
    }

    private void ResetSearch_OnClick(object? sender, RoutedEventArgs args) =>
        DisplayBooksRequested?.Invoke(this, EventArgs.Empty);

    public void DisplayBooks(List<BookViewModel> books)
    {
        _booksView.ReceiveBooks(books);
        _booksView.DisplayBooks();
    }

    public void DisplaySearchedBooks(List<BookViewModel> books)
    {
        _booksView.ReceiveBooks(books);
        _booksView.DisplayBooksSearch();
    }

    public void DisplayFatalError() => FatalError.IsVisible = true;

    private void OnReadBookRequested(object? sender, BookReadEventArgs args) => ReadBookRequested?.Invoke(this, args);
    private void OnGoToStats(object? sender, RoutedEventArgs e) => GoToStatsRequested?.Invoke(this, EventArgs.Empty);

    public void GoTo(string viewName) => _router!.GoTo(viewName);

    public void Push(NotificationSeverity notificationSeverity, string messageHeader, string messageText) =>
        _notificationChannel!.Push(notificationSeverity, messageHeader, messageText);

    public void OnEnter()
    {
        FatalError.IsVisible = false;
        DisplayBooksRequested?.Invoke(this, EventArgs.Empty);
    }

    public void OnQuit()
    {
        
    }
    
    public event EventHandler<BookSearchEventArgs>? SearchBookRequested;
    public event EventHandler<EventArgs>? DisplayBooksRequested;
    public event EventHandler<BookReadEventArgs>? ReadBookRequested;
    public event EventHandler<EventArgs>? GoToStatsRequested;
    }
}
