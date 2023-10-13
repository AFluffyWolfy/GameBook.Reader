namespace GBReader.LakeyeB.Repositories.Session
{
    public record DtoSession(DateTime FirstOpen, DateTime LastSave, int CurrentPage, string Isbn);
}