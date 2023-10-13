using GBReader.LakeyeB.Repositories;
using GBReader.LakeyeB.Repositories.Session;

namespace GBReader.LakeyeB.Infrastructures.Session
{
    /// <summary>
    /// Simple factory pour aisément implémenter de nouvelles infrastructures
    /// </summary>
    public static class SessionRepositoryFactory
    {
        public static ISessionRepository? CreateRepository(string infrastructure) => infrastructure.ToLower() switch
        {
            "json" => new JsonSessionHandler(),
            _ => null,
        };
    }
}