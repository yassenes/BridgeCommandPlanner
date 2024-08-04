using System.Data.SqlClient;

namespace BridgeCommandPlanner.SQL
{
    public class DbConnection
    {
        private SqlConnectionStringBuilder builder_;
        public DbConnection(string DataSource, string UserID, string Password, string InitialCatalog)
        {
            builder_ = new SqlConnectionStringBuilder();
            builder_.DataSource = DataSource;
            builder_.UserID = UserID;
            builder_.Password = Password;
            builder_.InitialCatalog = InitialCatalog;
            builder_.MultipleActiveResultSets = true;
            builder_.ConnectRetryCount = 254;
            builder_.ConnectTimeout = 5;
        }

        public SqlConnection Get()
        {
            return new SqlConnection(builder_.ConnectionString);
        }
    }
}
