using GBReader.LakeyeB.Domains;

namespace GBReader.LakeyeB.Presentations.ViewModel
{
    public record UserSessionViewModel
    {
        public DateTime FirstOpen { get; }
        public DateTime LastSave { get; }
        public int CurrentPage { get; }
        public string Isbn { get; }

        public UserSessionViewModel(UserSession session)
        {
            FirstOpen = session.FirstOpen;
            LastSave = session.LastSave;
            CurrentPage = session.CurrentPage;
            Isbn = session.Isbn;
        }
    }
}