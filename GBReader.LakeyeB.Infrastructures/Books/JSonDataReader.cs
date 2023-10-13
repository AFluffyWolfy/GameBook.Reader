using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Repositories;

namespace GBReader.LakeyeB.Infrastructures.Books
{
    /// <summary>
    /// Classe dorénavant inutile (n'utilisant plus JSon pour la lecture de livres)
    /// </summary>
    internal class JSonDataReader /* : IBookRepository*/
    {
        /*private static readonly string DirPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\XXXX";
        private static readonly string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\XXXX\\XXXX.json";
        public List<Book> GetBooks()
        {
            var books = new List<DtoBook>();
            CheckDirectory();

            try
            {
                string lines = File.ReadAllText(FilePath);
                books = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DtoBook>>(lines);
            } catch (FileNotFoundException e)
            {
                File.Create(FilePath);
            }

            return BookMapping.ToBook(books);
        }

        private void CheckDirectory()
        {
            if (Directory.Exists(DirPath))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(DirPath);

            }
            catch(FileNotFoundException e)
            {
                // ERREUR DE PATH
                throw new FileNotFoundException("ERREUR DE PATH" + e.Message);
            } 
            catch(IOException e)
            {
                throw new IOException("UNKNOWN ERROR" + e.Message);
            }
        }
    }
*/
    }
}
