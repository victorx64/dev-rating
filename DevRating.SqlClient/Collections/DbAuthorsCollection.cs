using System.Data;
using DevRating.SqlClient.Entities;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlClient.Collections
{
    internal sealed class DbAuthorsCollection : AuthorsCollection
    {
        private readonly IDbTransaction _transaction;

        public DbAuthorsCollection(IDbTransaction transaction)
        {
            _transaction = transaction;
        }

        public IdentifiableAuthor NewAuthor(string email)
        {
            using var command = _transaction.Connection.CreateCommand();
            command.Transaction = _transaction;

            command.CommandText = @"
                INSERT INTO [dbo].[Author]
                       ([Email])
                OUTPUT [Inserted].[Id]
                VALUES
                       (@Email)";

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 50) {Value = email});

            return new DbAuthor(_transaction, (int) command.ExecuteScalar());
        }

        public bool Exist(string email)
        {
            using var command = _transaction.Connection.CreateCommand();
            command.Transaction = _transaction;

            command.CommandText = "SELECT [Id] FROM [dbo].[Author] WHERE [Email] = @Email";

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) {Value = email});

            using var reader = command.ExecuteReader();

            return reader.Read();
        }

        public IdentifiableAuthor Author(string email)
        {
            using var command = _transaction.Connection.CreateCommand();
            command.Transaction = _transaction;

            command.CommandText = "SELECT [Id] FROM [dbo].[Author] WHERE [Email] = @Email";

            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar) {Value = email});

            using var reader = command.ExecuteReader();

            reader.Read();

            return new DbAuthor(_transaction, (int) reader["Id"]);
        }
    }
}