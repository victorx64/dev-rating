using System.Data;
using DevRating.SqlClient.Entities;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlClient.Collections
{
    internal sealed class DbRatingsCollection : RatingsCollection
    {
        private readonly IDbTransaction _transaction;

        public DbRatingsCollection(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public SqlRating NewRating(int author, double value, int match)
        {
            using var command = _transaction.Connection.CreateCommand();
            command.Transaction = _transaction;

            command.CommandText = @"
                INSERT INTO [dbo].[Rating]
                       ([Rating]
                       ,[LastRatingId]
                       ,[MatchId]
                       ,[AuthorId])
                OUTPUT [Inserted].[Id]
                VALUES
                       (@Rating
                       ,NULL
                       ,@MatchId
                       ,@AuthorId)";

            command.Parameters.Add(new SqlParameter("@Rating", SqlDbType.Real) {Value = value});
            command.Parameters.Add(new SqlParameter("@MatchId", SqlDbType.Int) {Value = match});
            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author});

            return new SqlRating(_transaction, (int) command.ExecuteScalar());
        }

        public SqlRating NewRating(int author, double value, int last, int match)
        {
            using var command = _transaction.Connection.CreateCommand();
            command.Transaction = _transaction;

            command.CommandText = @"
                INSERT INTO [dbo].[Rating]
                       ([Rating]
                       ,[LastRatingId]
                       ,[MatchId]
                       ,[AuthorId])
                OUTPUT [Inserted].[Id]
                VALUES
                       (@Rating
                       ,@LastRatingId
                       ,@MatchId
                       ,@AuthorId)";

            command.Parameters.Add(new SqlParameter("@Rating", SqlDbType.Real) {Value = value});
            command.Parameters.Add(new SqlParameter("@LastRatingId", SqlDbType.Int) {Value = last});
            command.Parameters.Add(new SqlParameter("@MatchId", SqlDbType.Int) {Value = match});
            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author});

            return new SqlRating(_transaction, (int) command.ExecuteScalar());
        }

        public bool HasRating(int author)
        {
            using var command = _transaction.Connection.CreateCommand();
            command.Transaction = _transaction;

            command.CommandText = "SELECT TOP (1) [Id] FROM [dbo].[Rating] WHERE [AuthorId] = @AuthorId ORDER BY [Id] DESC";

            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public SqlRating LastRatingOf(int author)
        {
            using var command = _transaction.Connection.CreateCommand();
            command.Transaction = _transaction;

            command.CommandText = "SELECT TOP (1) [Id] FROM [dbo].[Rating] WHERE [AuthorId] = @AuthorId ORDER BY [Id] DESC";

            command.Parameters.Add(new SqlParameter("@AuthorId", SqlDbType.Int) {Value = author});

            using var reader = command.ExecuteReader();

            reader.Read();
            
            return new SqlRating(_transaction, (int) reader["Id"]);
        }
    }
}