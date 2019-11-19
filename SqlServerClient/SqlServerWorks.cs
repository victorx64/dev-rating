using System.Data;
using DevRating.Database;
using DevRating.Domain;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlServerClient
{
    internal sealed class SqlServerWorks : Works
    {
        private readonly IDbConnection _connection;

        public SqlServerWorks(IDbConnection connection)
        {
            _connection = connection;
        }

        public DbWork Work(WorkKey key)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id 
                FROM Work 
                WHERE Repository = @Repository 
                AND StartCommit = @StartCommit
                AND EndCommit = @EndCommit";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = key.Repository()});
            command.Parameters.Add(new SqlParameter("@StartCommit", SqlDbType.NVarChar, 50)
                {Value = key.StartCommit()});
            command.Parameters.Add(new SqlParameter("@EndCommit", SqlDbType.NVarChar, 50) {Value = key.EndCommit()});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new SqlServerDbWork(_connection, (int) reader["Id"]);
        }

        public bool Exist(WorkKey key)
        {
            using var command = _connection.CreateCommand();

            command.CommandText = @"
                SELECT Id 
                FROM Work 
                WHERE Repository = @Repository 
                AND StartCommit = @StartCommit
                AND EndCommit = @EndCommit";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = key.Repository()});
            command.Parameters.Add(new SqlParameter("@StartCommit", SqlDbType.NVarChar, 50)
                {Value = key.StartCommit()});
            command.Parameters.Add(new SqlParameter("@EndCommit", SqlDbType.NVarChar, 50) {Value = key.EndCommit()});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public DbWork Insert(string repository, string start, string end, DbObject author,
            double reward, DbObject rating)
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
                OUTPUT Inserted.Id
                VALUES
                    (@Repository
                    ,@StartCommit
                    ,@EndCommit
                    ,@AuthorId
                    ,@Reward
                    ,@UsedRatingId)";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = repository});
            command.Parameters.Add(new SqlParameter("@StartCommit", SqlDbType.NVarChar, 50) {Value = start});
            command.Parameters.Add(new SqlParameter("@EndCommit", SqlDbType.NVarChar, 50) {Value = end});
            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author.Id()});
            command.Parameters.Add(new SqlParameter("@Reward", SqlDbType.Real) {Value = reward});
            command.Parameters.Add(new SqlParameter("@UsedRatingId", SqlDbType.Int) {Value = rating.Id()});

            var id = (int) command.ExecuteScalar();

            return new SqlServerDbWork(_connection, id);
        }

        public DbWork Insert(string repository, string start, string end, DbObject author,
            double reward)
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
                OUTPUT Inserted.Id
                VALUES
                    (@Repository
                    ,@StartCommit
                    ,@EndCommit
                    ,@AuthorId
                    ,@Reward
                    ,NULL)";

            command.Parameters.Add(new SqlParameter("@Repository", SqlDbType.NVarChar) {Value = repository});
            command.Parameters.Add(new SqlParameter("@StartCommit", SqlDbType.NVarChar, 50) {Value = start});
            command.Parameters.Add(new SqlParameter("@EndCommit", SqlDbType.NVarChar, 50) {Value = end});
            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author.Id()});
            command.Parameters.Add(new SqlParameter("@Reward", SqlDbType.Real) {Value = reward});

            var id = (int) command.ExecuteScalar();

            return new SqlServerDbWork(_connection, id);
        }
    }
}