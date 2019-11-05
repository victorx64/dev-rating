using System.Collections.Generic;
using System.Data;
using DevRating.Domain;
using DevRating.SqlClient.Collections;
using Microsoft.Data.SqlClient;

namespace DevRating.SqlClient
{
    public class SqlAuthorsRepository : AuthorsRepository
    {
        private readonly IDbConnection _connection;

        public SqlAuthorsRepository(string connection)
            : this(new SqlConnection(connection))
        {
        }

        public SqlAuthorsRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        
        public IEnumerable<Author> TopAuthors()
        {
            _connection.Open();

            var transaction = _connection.BeginTransaction();

            try
            {
                var authors = new SqlAuthorsCollection(transaction);

                transaction.Commit();

                return authors.TopAuthors();
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}