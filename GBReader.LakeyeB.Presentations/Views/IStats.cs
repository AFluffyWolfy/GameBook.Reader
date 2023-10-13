using GBReader.LakeyeB.Presentations.Notifications;
using GBReader.LakeyeB.Presentations.ViewModel;

namespace GBReader.LakeyeB.Presentations.Views
{
    public interface IStats
    {
        event EventHandler<EventArgs> StatsDisplayRequested;
        event EventHandler<EventArgs>? BackToMenuRequested; 
        public void GoTo(string view);
        public void Push(NotificationSeverity serverity, string title, string message);
        public void DisplaySessionsStat(List<UserSessionViewModel> sessions);
    }
}