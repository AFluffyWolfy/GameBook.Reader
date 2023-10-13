using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Repositories;
using GBReader.LakeyeB.Repositories.Session;
using Newtonsoft.Json;

namespace GBReader.LakeyeB.Infrastructures.Session
{
    /// <summary>
    /// Permet d'intéragir avec un Json pour gérer les sessions de lectures de livres.
    /// </summary>
    public class JsonSessionHandler : ISessionRepository
    {
        private static string _dirPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\XXXX";
        private static string _filePath = _dirPath + "\\XXXX-session.json";

        public JsonSessionHandler()
        {
            CheckFileAndDirectory();
        }

        /// <summary>
        /// Constructeur pour les tests Json, servant à remplacer DirPath et FilePath
        /// Je pars du principe que ce constructeur n'existe pas pour d'autres utilisations
        /// et ne vérifie donc pas des cas erronés de directoryPath ou fileName
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        public JsonSessionHandler(string directoryPath, string fileName)
        {
            _dirPath = Path.GetFullPath(directoryPath);
            _filePath = Path.Combine(_dirPath, fileName + ".json");
            CheckFileAndDirectory();
        }

        /// <summary>
        /// Vérifie si une session pour le livre "isbn" existe.
        /// Si oui, retourne la session sous forme UserSession
        /// Si non, retourne null
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        /// <exception cref="RepoException"></exception>
        public UserSession? CheckSessionExists(string isbn)
        {
            try
            {
                List<UserSession> sessions = SessionMapping.ToUserSessions(GetDtoUserSessions());

                foreach (var session in sessions)
                {
                    if (session.Isbn.Equals(isbn))
                    {
                        return session;
                    }
                }

                return null;
            }
            catch (RepoException e)
            {
                throw new RepoException(e.Message);
            }

        }

        public void RemoveSession(UserSession currentSession)
        {
            try
            {
                DtoSession currentSessionDto = SessionMapping.ToDtoSession(currentSession)!;
                List<DtoSession> sessions = GetDtoUserSessions();

                bool foundAndRemoved = SearchAndRemoveSession(currentSessionDto, sessions);

                if (foundAndRemoved)
                {
                    WriteSessionsIntoJson(sessions);
                }
                else
                {
                    throw new RepoException("Impossible de supprimer la session : elle n'existe pas");
                }
            }
            catch (Exception e)
            {
                if (e is RepoException) throw new RepoException(e.Message);
                if (e is JsonException)
                    throw new RepoException("Impossible de convertir les sessions dans le fichier JSon");
                if (e is ArgumentException or IOException)
                    throw new RepoException("Impossible de supprimer la session");
            }
        }

        public void SaveSession(UserSession currentSession)
        {
            try
            {
                DtoSession currentSessionDto = SessionMapping.ToDtoSession(currentSession)!;
                List<DtoSession> sessions = GetDtoUserSessions();

                SearchAndRemoveSession(currentSessionDto, sessions);
                sessions.Add(currentSessionDto);

                WriteSessionsIntoJson(sessions);
            }
            catch (Exception e)
            {
                if (e is RepoException) throw new RepoException(e.Message);
                if (e is JsonException)
                    throw new RepoException("Impossible de convertir les sessions dans le fichier JSon");
                if (e is ArgumentException or IOException)
                    throw new RepoException("Impossible de sauvegarder la session");

            }
        }

        public List<UserSession> GetUserSessions()
        {
            List<DtoSession> dtoSessions = GetDtoUserSessions();
            return SessionMapping.ToUserSessions(dtoSessions);
        }

        private static bool SearchAndRemoveSession(DtoSession currentSession, List<DtoSession> sessions)
        {
            for (int i = sessions.Count - 1; i >= 0; i--)
            {
                if (sessions[i].Isbn.Equals(currentSession.Isbn))
                {
                    sessions.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private static void WriteSessionsIntoJson(List<DtoSession> sessions)
        {
            string jsonString = JsonConvert.SerializeObject(sessions);

            using var fileStream = new FileStream(_filePath, FileMode.Truncate, FileAccess.Write);
            using StreamWriter writer = new StreamWriter(fileStream);
            writer.Write(jsonString);
        }

        private List<DtoSession> GetDtoUserSessions()
        {
            List<DtoSession> sessions;

            try
            {
                sessions = ReadJson();
            }
            catch (JsonException e)
            {
                throw new RepoException("Impossible d'obtenir les sessions", e);
            }

            return sessions;
        }

        private static List<DtoSession> ReadJson()
        {
            List<DtoSession>? sessions;
            using (StreamReader reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                sessions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DtoSession>>(json);
            }

            return sessions == null ? new List<DtoSession>() : sessions;
        }

        private void CheckFileAndDirectory()
        {
            CheckDirectory();
            CheckFile();
        }

        private void CheckDirectory()
        {
            try
            {
                if (Directory.Exists(_dirPath))
                {
                    return;
                }

                Directory.CreateDirectory(_dirPath);
            }
            catch (Exception e)
            {
                throw new RepoException("Impossible de créer le json", e);
            }
        }

        private void CheckFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    return;
                }

                using var createFile = File.Create(_filePath);
                createFile.Close();
            }
            catch (Exception e)
            {
                throw new RepoException("Impossible de créer le json", e);
            }
        }
    }
}