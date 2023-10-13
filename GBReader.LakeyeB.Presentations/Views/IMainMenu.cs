using GBReader.LakeyeB.Presentations.Events;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.ViewModel;

namespace GBReader.LakeyeB.Presentations.Views
{
    public interface IMainMenu
    {
        public void DisplayBooks(List<BookViewModel> books);
        public void DisplaySearchedBooks(List<BookViewModel> books);
        void DisplayFatalError();
        event EventHandler<BookSearchEventArgs> SearchBookRequested;
        event EventHandler<EventArgs> DisplayBooksRequested;
        event EventHandler<BookReadEventArgs> ReadBookRequested;
        event EventHandler<EventArgs>? GoToStatsRequested;
        void GoTo(string viewName);
        void Push(NotificationSeverity notificationSeverity, string messageHeader, string messageText);
    }
}
