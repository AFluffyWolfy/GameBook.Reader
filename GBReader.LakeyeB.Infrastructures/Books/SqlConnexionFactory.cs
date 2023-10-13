using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace GBReader.LakeyeB.Infrastructures.Books
{
    /// <summary>
    /// Permet de fabriquer des connexions à une BDD
    /// (code presque similaire à celui fourni pas Monsieur Ludwig)
    /// </summary>
    public class SqlConnexionFactory
    {
        private readonly MySqlClientFactory _factory;
        private readonly string _connectionString;
        
        public SqlConnexionFactory(string connectionString)
        {
            try
            {
                _factory = MySqlClientFactory.Instance;
                _connectionString = connectionString;
            }
            catch (ArgumentException ex)
            {
                throw new ProviderNotFoundException($"Unable to load provider MySqlClient", ex);
            }
        }

        public IDbConnection NewCon()
        {
            IDbConnection con;
            try
            {
                con = _factory.CreateConnection();
                con.ConnectionString = _connectionString;
                con.Open();
            }  
            catch (ArgumentException ex)
            {
                throw new InvalidConnectionStringException(ex);
            } 
            catch (SqlException ex)
            {
                throw new UnableToConnectException(ex);
            }

            return con;
        }
        
    }

    public class UnableToConnectException : Exception
    {
        public UnableToConnectException(Exception sqlException)
            : base("Unable to establish connection to db", sqlException)
        { }
    }

    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(string s, Exception argumentException)
            :base(s, argumentException)
        {
        }
    }

    public class InvalidConnectionStringException : Exception
    {
        public InvalidConnectionStringException(Exception argumentException)
            : base("Unable to use this connection string", argumentException)
        {
        }
    }
}