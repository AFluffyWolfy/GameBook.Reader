using GBReader.LakeyeB.Presentations.Events;
using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.ViewModel;

namespace GBReader.LakeyeB.Presentations.Views
{
    public interface IReadingBook
    {
        event EventHandler<EventArgs> DisplayPageRequested;
        event EventHandler<EventArgs> DisplayChoicesRequested;
        event EventHandler<ChoiceSelectedEventArgs> ChoiceSelected;
        event EventHandler<EventArgs> BackToBeginningRequested;
        event EventHandler<BackToMenuEventArgs> BackToMenuRequested;
        public void GoTo(string view);
        public void Push(NotificationSeverity serverity, string title, string message);
        void DisplayPage(string pageText);
        void DisplayChoices(List<ChoiceViewModel> choices);
    }
}
