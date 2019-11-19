using System.Data;
using DevRating.Database;

namespace DevRating.SqlServerClient
{
    public sealed class SqlServerEntities : Entities
    {
        private readonly Works _works;
        private readonly Authors _authors;
        private readonly Ratings _ratings;
        private readonly IDbConnection _connection;

        public SqlServerEntities(IDbConnection connection)
            : this(connection,
                new SqlServerWorks(connection),
                new SqlServerAuthors(connection),
                new SqlServerRatings(connection))
        {
        }

        public SqlServerEntities(IDbConnection connection, Works works, Authors authors, Ratings ratings)
        {
            _connection = connection;
            _works = works;
            _authors = authors;
            _ratings = ratings;
        }

        public IDbConnection Connection()
        {
            return _connection;
        }

        public Works Works()
        {
            return _works;
        }

        public Authors Authors()
        {
            return _authors;
        }

        public Ratings Ratings()
        {
            return _ratings;
        }
    }
}