using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Repositories.Session
{
    public interface ISessionRepository
    {
        UserSession? CheckSessionExists(string isbn);
        public void RemoveSession(UserSession currentSession);
        void SaveSession(UserSession getCurrentSession);
        List<UserSession> GetUserSessions();
    }
}