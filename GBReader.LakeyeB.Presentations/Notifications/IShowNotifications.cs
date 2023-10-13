namespace GBReader.LakeyeB.Presentations.Notifications;

public interface IShowNotifications
{
    void Push(NotificationSeverity serverity, string title, string message);
}