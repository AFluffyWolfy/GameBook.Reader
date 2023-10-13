using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Infrastructures.Session;
using GBReader.LakeyeB.Repositories;

namespace GBReader.LakeyeB.Infrastructures.Tests
{
    public class Tests
    {
        private static readonly UserSession Session =
            new UserSession(DateTime.UnixEpoch, DateTime.UnixEpoch, 1, "2-210129-01-X");

        private static readonly string DirPath =
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName + "\\FileTests";

        /// <summary>
        /// Méthode de sécurité au cas où un problème aurait lieu avec le dossier TestsEvent contenant
        /// les ressources pour les tests
        /// </summary>
        [OneTimeSetUp]
        public void SetUpOnce()
        {
            if (!Directory.Exists(DirPath))
            {
                Directory.CreateDirectory(DirPath);
                CreateTestsRessourcesFiles();
            } else if (!Directory.EnumerateFileSystemEntries(DirPath).Any())
            {
                CreateTestsRessourcesFiles();
            }
        }

        private static void CreateTestsRessourcesFiles()
        {
            CreateFile(DirPath + "\\missingData.json");
            WriteIntoJson(DirPath + "\\missingData.json",
                "[{,\"LastSave\":\"2022-12-22T09:08:50.3353028+01:00\",\"CurrentPage\":1,\"Isbn\":\"2-210129-00-1\"}]");

            CreateFile(DirPath + "\\existingSessionsToGet.json");
            WriteIntoJson(DirPath + "\\existingSessionsToGet.json",
                "[{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":1,\"Isbn\":\"2-210129-01-X\"},{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":2,\"Isbn\":\"2-210129-00-1\"}]");
            
            CreateFile(DirPath + "\\checkSessionExists.json");
            WriteIntoJson(DirPath + "\\checkSessionExists.json", "[{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":1,\"Isbn\":\"2-210129-01-X\"}]"
                );
        }

        /// <summary>
        /// If a directory path does not exists, JsonSessionHandler should create it along with its file.
        /// </summary>
        [Test]
        public void DirectoryDoesNotExists()
        {
            // GIVEN : dir that does not exists
            string tempDirPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                                 "\\ue36-tests-dirnotexists";
            const string tempFile = "dir-not-exists-test";
            string tempFilePath = tempDirPath + "\\" + tempFile + ".json";
            
            // WHEN : creating object
            JsonSessionHandler repo = new JsonSessionHandler(tempDirPath, tempFile);

            // THEN : the dir was created with its file
            try
            {
                Assert.That(Directory.Exists(tempDirPath));
                Assert.That(File.Exists(tempFilePath));
            }
            finally
            {
                File.Delete(tempFilePath);
                Directory.Delete(tempDirPath);
            }
        }

        /// <summary>
        /// A wrong format cannot happen because JsonSessionHandler adds .json at the end for the tests constructor
        /// In the parameter-less constructor, the filePath is hard coded as a static readonly string as
        /// a .json file
        /// </summary>
        [Test]
        public void BadFileFormat()
        {
            //GIVEN : A wrong format name
            const string tempFile = "wrong-format-cannot-happen.png";
            string tempFilePath = DirPath + "\\" + tempFile + ".json";
            
            // WHEN : creating object
            JsonSessionHandler repo = new JsonSessionHandler(DirPath, tempFile);

            // THEN : the file was created with .json at the end
            try
            {
                Assert.That(File.Exists(tempFilePath));
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        /// <summary>
        /// Test case is the Json having one session, which is missing the first parameter "FirstSave"
        /// </summary>
        [Test]
        public void MissingDataInJson()
        {
            //GIVEN : a json mising some data
            JsonSessionHandler repo = new JsonSessionHandler(DirPath, "missingData");
            
            // WHEN : calling the methode GetUserSessions
            // THEN : a RepoException is thrown
            Assert.Throws<RepoException>(() => repo.GetUserSessions());
        }

        /// <summary>
        /// Test is to establish if function GetSessions works as intended
        /// </summary>
        [Test]
        public void GetSessionsWorks()
        {
            // GIVEN : a Json with two sessions correctly encoded
            JsonSessionHandler repo = new(DirPath, "existingSessionsToGet");

            // WHEN : Calling function GetUserSessions
            List<UserSession> sessions = repo.GetUserSessions();
            
            // THEN : What I got is what is inside the Json in the right format and object
            Assert.That(sessions.Count == 2);
            Assert.That(sessions[0].Isbn == "2-210129-01-X" && sessions[0].CurrentPage == 1 &&
                        sessions[1].Isbn == "2-210129-00-1" && sessions[1].CurrentPage == 2);
        }

        /// <summary>
        /// Save a session into an empty json file and assert it was indeed saved 
        /// </summary>
        [Test]
        public void SaveSessionInEmptyJson()
        {
            // GIVEN : an empty json
            string tempFilePath = DirPath + "\\saveSessionEmpty.json";
            CreateFile(tempFilePath);
            try
            {
                JsonSessionHandler repo = new JsonSessionHandler(DirPath, "saveSessionEmpty");

                // WHEN : Saving session into the Json
                repo.SaveSession(Session);

                // THEN : The session appears in the json, in the good format
                List<UserSession> sessions = repo.GetUserSessions();

                Assert.That(sessions[0].FirstOpen == DateTime.UnixEpoch &&
                            sessions[0].LastSave == DateTime.UnixEpoch &&
                            sessions[0].Isbn == "2-210129-01-X");
            }
            finally
            {
                File.Delete(tempFilePath);
            }
            
            
        }

        /// <summary>
        /// Save a session into a json holding already two differents sessions
        /// Assert that the session was added and the others sessions were unaffected
        /// </summary>
        [Test]
        public void SaveSessionIntoJsonWithTwoSessions()
        {
            // GIVEN : a json with two sessions having different isbn than the current one
            string tempFilePath = DirPath + "\\saveSessionExistingSessions.json";
            CreateFile(tempFilePath);
            UserSession toSave = new UserSession(DateTime.UnixEpoch, DateTime.UnixEpoch, 3, "2-210129-02-8");

            try
            {
                WriteIntoJson(tempFilePath,
                    "[{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":1,\"Isbn\":\"2-210129-00-1\"},{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":2,\"Isbn\":\"2-210129-01-X\"}]");
                JsonSessionHandler repo = new(DirPath, "saveSessionExistingSessions");

                // WHEN : Saving the session into the Json
                repo.SaveSession(toSave);

                // THEN : The two pre-existing sessions are still there and the new session was added after them
                List<UserSession> sessions = repo.GetUserSessions();
                Assert.That(sessions.Count == 3);
                Assert.That(sessions[0].Isbn == "2-210129-00-1" && sessions[0].CurrentPage == 1 &&
                            sessions[1].Isbn == "2-210129-01-X" && sessions[1].CurrentPage == 2 &&
                            sessions[2].Isbn == "2-210129-02-8" && sessions[2].CurrentPage == 3);
            }
            finally
            {
                File.Delete(tempFilePath);
            }

            
        }

        /// <summary>
        /// Save a session which already exists in the file (so update it, basically)
        /// Assert that the Json was indeed updated with the newer session
        /// </summary>
        [Test]
        public void SaveSessionAlreadyExistsInJson()
        {
            //GIVEN : A Json having a session with the same ISBN as the current one
            string tempFilePath = DirPath + "\\saveSessionExisting.json";
            CreateFile(tempFilePath);
            UserSession toSave = new UserSession(DateTime.UnixEpoch, DateTime.UnixEpoch.Add(TimeSpan.FromDays(1)),
                2, "2-210129-01-X");

            try
            {
                WriteIntoJson(tempFilePath,
                    "[{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":1,\"Isbn\":\"2-210129-01-X\"}]");

                JsonSessionHandler repo = new(DirPath, "saveSessionExisting");

                // WHEN : Savng the session into the Json
                repo.SaveSession(toSave);

                // THEN : The LastSave and CurrentPage of this session in the Json have been updated
                List<UserSession> sessions = repo.GetUserSessions();
                Assert.That(sessions.Count == 1 &&
                            sessions[0].LastSave == DateTime.UnixEpoch.Add(TimeSpan.FromDays(1)) &&
                            sessions[0].CurrentPage == 2);
            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        /// <summary>
        /// Assert that this function works as intended
        /// </summary>
        [Test]
        public void CheckSessionExistsWorks()
        {
            // GIVEN : a json with one session
            JsonSessionHandler repo = new(DirPath, "checkSessionExists");
            
            // WHEN 1) trying to get an ISBN that matches
            //      2) trying to get an ISBN that doesn't exists in the json
            UserSession? existingSession = repo.CheckSessionExists("2-210129-01-X");
            UserSession? nonExistingSession = repo.CheckSessionExists("a");

            // THEN 1) Gets the session linked to that ISBN
            //      2) Gets null
            Assert.That(existingSession?.Isbn == "2-210129-01-X" && existingSession.CurrentPage == 1 && existingSession.FirstOpen == DateTime.UnixEpoch);
            Assert.That(nonExistingSession, Is.EqualTo(null));
        }

        /// <summary>
        /// Assert that this function works as intended, hence removes a session from the json
        /// </summary>
        [Test]
        public void RemoveSessionWorks()
        {
            // GIVEN : a json with two sessions
            string tempFilePath = DirPath + "\\removeSession.json";
            CreateFile(tempFilePath);

            try
            {
                WriteIntoJson(tempFilePath, "[{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":1,\"Isbn\":\"2-210129-00-1\"}," +
                                            "{\"FirstOpen\":\"1970-01-01T00:00:00Z\",\"LastSave\":\"1970-01-01T00:00:00Z\",\"CurrentPage\":2,\"Isbn\":\"2-210129-01-X\"}]");

                JsonSessionHandler repo = new JsonSessionHandler(DirPath, "removeSession");
                
                // WHEN : 1) Removing a session matching the ISBN of one of the Json sessions
                repo.RemoveSession(Session);
                
                // THEN : 1) The sessions is removed from the Json
                List<UserSession> sessions = repo.GetUserSessions();
                Assert.That(sessions.Count == 1 && sessions[0].Isbn == "2-210129-00-1");
                
                // WHEN : 2) Removing a session that doesn't match the ISBN of one of the Json sessions
                // THEN : 2) RepoException is thrown stating that the session doesn't exists
                Assert.Throws<RepoException>((() => repo.RemoveSession(Session)));


            }
            finally
            {
                File.Delete(tempFilePath);
            }
        }

        private static void CreateFile(string tempFilePath)
        {
            using var createFile = File.Create(tempFilePath);
            createFile.Close();
        }

        private static void WriteIntoJson(string tempFilePath, string content)
        {
            using (StreamWriter writer = new StreamWriter(tempFilePath))
            {
                writer.Write(content);
                writer.Close();
            }
        }
    }
}