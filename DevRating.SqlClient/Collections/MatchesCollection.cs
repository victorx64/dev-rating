using DevRating.SqlClient.Entities;

namespace DevRating.SqlClient.Collections
{
    internal interface MatchesCollection
    {
        Match NewMatch(int first, int second, string commit, string repository, uint count);
    }
}