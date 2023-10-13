using System.Data;
using GBReader.LakeyeB.Domains;
using GBReader.LakeyeB.Repositories;
using GBReader.LakeyeB.Repositories.Books;
using MySql.Data.MySqlClient;

namespace GBReader.LakeyeB.Infrastructures.Books
{
    /// <summary>
    /// Permet d'intéragir avec la BDD contenant les livres.
    /// Possède une SqlConnexionFactory pour créer ses propres connexions à chaques requêtes,
    /// et ainsi les fermées.
    /// </summary>
    public class SqlDatabaseReader : IBookRepository
    {
        private readonly SqlConnexionFactory _conFactory;

        public SqlDatabaseReader(SqlConnexionFactory conFactory)
        {
            _conFactory = conFactory;
        }

        public List<Book> GetBooks()
        {
            try
            {
                using var con = _conFactory.NewCon();

                List<Book> books = new List<Book>();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT a.Name, b.Title, b.Isbn, b.Summary FROM aib2__Book b "
                                      + "JOIN aib2__Author a on b.IDAuthor = a.IDAuthor "
                                      + "WHERE isPublished = 1";
                    cmd.ExecuteNonQuery();
                    using (var reader = cmd.ExecuteReader())
                    {
                        FillBooksList(reader, books);
                    }
                }

                con.Close();

                return books;
            }
            catch (Exception exception)
            {
                throw new RepoException("Impossible d'obtenir les livres", exception);
            }
            
        }

        public Dictionary<int, string> GetPages(string isbn)
        {
            try
            {
                using var con = _conFactory.NewCon();

                Dictionary<int, string> pages = new();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT p.PageNumber, p.Text FROM aib2__Page p WHERE p.ISBN = @isbn";

                    MySqlParameter paramIsbn = new();
                    paramIsbn.ParameterName = "@isbn";
                    paramIsbn.DbType = DbType.String;
                    paramIsbn.Value = isbn;
                    cmd.Parameters.Add(paramIsbn);

                    cmd.ExecuteNonQuery();
                    using (var reader = cmd.ExecuteReader())
                    {
                        FillPagesList(reader, pages);
                    }
                }
                con.Close();

                return pages;
            }
            catch (Exception e)
            {
                throw new RepoException($"Impossible d'obtenir les pages du livre {isbn}", e);
            }
        }

        public List<Choice> GetChoices(string isbn)
        {
            try
            {
                using var con = _conFactory.NewCon();

                List<Choice> choices = new();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT " +
                                      "pto.PageNumber AS PageFrom, pfrom.PageNumber AS PageTo, ch2.Text " +
                                      "FROM aib2__Page p2 " +
                                      "JOIN aib2__Choice ch2 ON ch2.IDPageFrom = p2.IDPage " +
                                      "JOIN aib2__Page pfrom ON pfrom.IDPage = ch2.IDPageTo " +
                                      "JOIN aib2__Page pto ON pto.IDPage = ch2.IDPageFrom " +
                                      "WHERE p2.isbn = @isbn";

                    MySqlParameter paramIsbn = new();
                    paramIsbn.ParameterName = "@isbn";
                    paramIsbn.DbType = DbType.String;
                    paramIsbn.Value = isbn;
                    cmd.Parameters.Add(paramIsbn);

                    cmd.ExecuteNonQuery();
                    using (var reader = cmd.ExecuteReader())
                    {
                        FillChoicesList(reader, choices);
                    }
                }

                con.Close();

                return choices;
            }
            catch (InvalidOperationException e)
            {
                throw new RepoException($"Impossible d'accéder aux choix du livre {isbn}", e);
            }
            catch (Exception e)
            {
                throw new RepoException($"Impossible d'accéder aux choix du livre {isbn}", e);
            }
        }

        private void FillBooksList(IDataReader reader, List<Book> books)
        {
            while (reader.Read())
            {
                books.Add(new Book(
                    (string)reader[0],
                    (string)reader[1],
                    (string)reader[2],
                    (string)reader[3]));
            }
        }
        
        private void FillPagesList(IDataReader reader, Dictionary<int, string> pages)
        {
            while (reader.Read())
            {
                pages.Add((int)reader[0], (string)reader[1]);
            }
        }
        
        private void FillChoicesList(IDataReader reader, List<Choice> choices)
        {
            while (reader.Read())
            {
                choices.Add(new Choice((string)reader[2], (int)reader[0], (int)reader[1]));
            }
        }
        
    }
}
