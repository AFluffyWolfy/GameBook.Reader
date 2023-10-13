using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Repositories.Session
{
    public static class SessionMapping
    {
        public static List<UserSession> ToUserSessions(List<DtoSession>? dtoSessions)
        {
            List<UserSession> sessions = new();
            if (dtoSessions != null)
            {
                foreach (DtoSession dtoSession in dtoSessions)
                {
                    sessions.Add(new UserSession(dtoSession.FirstOpen, dtoSession.LastSave, dtoSession.CurrentPage, dtoSession.Isbn));
                }
            }
            return sessions;
        }

        public static DtoSession? ToDtoSession(UserSession? userSession)
        {
            if (userSession != null)
            {
                return new DtoSession(userSession.FirstOpen, userSession.LastSave, userSession.CurrentPage,
                    userSession.Isbn);
            }

            return null;
        }
    }
}