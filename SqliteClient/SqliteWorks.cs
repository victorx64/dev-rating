using System.Data;
using DevRating.Database;
using DevRating.Domain;
using Microsoft.Data.Sqlite;

namespace DevRating.SqliteClient
{
    internal sealed class SqliteWorks : Works
    {
        private readonly IDbConnection _connection;

        public SqliteWorks(IDbConnection connection)
        {
            _connection = connection;
        }

        public DbWork Work(Diff diff)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id 
                FROM Work 
                WHERE Repository = @Repository 
                AND StartCommit = @StartCommit
                AND EndCommit = @EndCommit";

            command.Parameters.Add(new SqliteParameter("@Repository", SqliteType.Text) {Value = diff.Key()});
            command.Parameters.Add(new SqliteParameter("@StartCommit", SqliteType.Text, 50)
                {Value = diff.StartCommit()});
            command.Parameters.Add(new SqliteParameter("@EndCommit", SqliteType.Text, 50) {Value = diff.EndCommit()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqliteDbWork(_connection, reader["Id"]);
        }

        public bool Exist(Diff diff)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id 
                FROM Work 
                WHERE Repository = @Repository 
                AND StartCommit = @StartCommit
                AND EndCommit = @EndCommit";

            command.Parameters.Add(new SqliteParameter("@Repository", SqliteType.Text) {Value = diff.Key()});
            command.Parameters.Add(new SqliteParameter("@StartCommit", SqliteType.Text, 50)
                {Value = diff.StartCommit()});
            command.Parameters.Add(new SqliteParameter("@EndCommit", SqliteType.Text, 50) {Value = diff.EndCommit()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public DbWork Insert(string repository, string start, string end, DbObject author, double reward,
            DbObject rating)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Work
                    (Repository
                    ,StartCommit
                    ,EndCommit
                    ,AuthorId
                    ,Reward
                    ,UsedRatingId)
                VALUES
                    (@Repository
                    ,@StartCommit
                    ,@EndCommit
                    ,@AuthorId
                    ,@Reward
                    ,@UsedRatingId);
                SELECT last_insert_rowid();";

            command.Parameters.Add(new SqliteParameter("@Repository", SqliteType.Text) {Value = repository});
            command.Parameters.Add(new SqliteParameter("@StartCommit", SqliteType.Text, 50) {Value = start});
            command.Parameters.Add(new SqliteParameter("@EndCommit", SqliteType.Text, 50) {Value = end});
            command.Parameters.Add(new SqliteParameter("@AuthorId", SqliteType.Integer) {Value = author.Id()});
            command.Parameters.Add(new SqliteParameter("@Reward", SqlDbType.Real) {Value = reward});
            command.Parameters.Add(new SqliteParameter("@UsedRatingId", SqliteType.Integer) {Value = rating.Id()});

            var id = command.ExecuteScalar();

            return new SqliteDbWork(_connection, id);
        }

        public DbWork Insert(string repository, string start, string end, DbObject author, double reward)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Work
                    (Repository
                    ,StartCommit
                    ,EndCommit
                    ,AuthorId
                    ,Reward
                    ,UsedRatingId)
                VALUES
                    (@Repository
                    ,@StartCommit
                    ,@EndCommit
                    ,@AuthorId
                    ,@Reward
                    ,NULL);
                SELECT last_insert_rowid();";

            command.Parameters.Add(new SqliteParameter("@Repository", SqliteType.Text) {Value = repository});
            command.Parameters.Add(new SqliteParameter("@StartCommit", SqliteType.Text, 50) {Value = start});
            command.Parameters.Add(new SqliteParameter("@EndCommit", SqliteType.Text, 50) {Value = end});
            command.Parameters.Add(new SqliteParameter("@AuthorId", SqliteType.Integer) {Value = author.Id()});
            command.Parameters.Add(new SqliteParameter("@Reward", SqlDbType.Real) {Value = reward});

            var id = command.ExecuteScalar();

            return new SqliteDbWork(_connection, id);
        }
    }
}