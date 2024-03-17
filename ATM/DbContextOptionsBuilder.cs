using Microsoft.Data.SqlClient;

namespace ATM
{
    internal class DbContextOptionsBuilder<T>
    {
        public DbContextOptionsBuilder()
        {
        }

        internal object UseSqlServer(SqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}