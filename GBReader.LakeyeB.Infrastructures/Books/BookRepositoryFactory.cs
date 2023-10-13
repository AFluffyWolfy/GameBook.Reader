using GBReader.LakeyeB.Repositories.Books;

namespace GBReader.LakeyeB.Infrastructures.Books
{
    /// <summary>
    /// Simple factory pour aisément implémenter de nouvelles infrastructures
    /// </summary>
    public static class BookRepositoryFactory
    {
        public static IBookRepository? CreateRepository(string infrastructure)
        {
            switch (infrastructure.ToLower())
            {
                case "json":
                    return null;
                case "sql":
                    SqlConnexionFactory connexionFactory = new SqlConnexionFactory(
                        "Server=XXXX;Port=XXXX;Database=XXXX;Uid=XXXX;Pwd=XXXX;");
                    SqlDatabaseReader repo = new SqlDatabaseReader(connexionFactory);
                    return repo;
                default:
                    return null;
            }
        }
    }
}