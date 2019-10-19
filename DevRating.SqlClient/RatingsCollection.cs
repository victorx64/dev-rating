namespace DevRating.SqlClient
{
    internal interface RatingsCollection
    {
        Rating NewRating(int author, double value, int last, int match);
        Rating LastRatingOf(int author);
    }
}