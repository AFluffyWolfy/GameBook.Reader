namespace GBReader.LakeyeB.Repositories
{
    public class RepoException : Exception
    {
        public RepoException() {}
        public RepoException(string s) : base(s)
        {
        }

        public RepoException(string s, Exception e) : base(s, e)
        {
        }
    }
}