using System.Data;

namespace DevRating.SqlClient
{
    internal sealed class DbMatch : Match
    {
        private readonly IDbConnection _connection;
        private readonly int _id;

        public DbMatch(IDbConnection connection, int id)
        {
            _connection = connection;
            _id = id;
        }

        public int Id()
        {
            return _id;
        }
    }
}